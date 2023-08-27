/*

See documentation at https://github.com/Pathoschild/StardewScripts.

*/
(function() {
    /*********
    ** Read Discord data
    *********/
    // An export of every Discord user who has the 'Mod Author' role, in the form `id: username`.
    const discordUsersById = { /* TODO */ };


    /*********
    ** Read GitHub data
    *********/
    // The GitHub users with special permission to access the decompiled repo without being listed on the wiki.
    const usersWithSpecialRepoPermission = [ /* TODO */ ].map(p => p.toLowerCase());

    // The GitHub users with access to the decompiled code. Run this script on each page of the project's
    // collaborators page to generate the list:
    /*
        (function() {
            const users = [];

            for (const row of document.querySelectorAll("#repository-access-table .js-repo-access-entry")) {
                const username = row.querySelector("a")?.href?.match(/^https:\/\/github.com\/(.+)/)?.[1];
                if (username)
                    users.push(username);
                else
                    console.warn("Can't get username from row.", row);
            }

            console.log("\"" + users.join("\",\"") + "\"");
        })();
    */
    const usersWithMainRepoAccess = [ /* TODO */ ].map(p => p.toLowerCase());
    const usersWithAndroidRepoAccess = [ /* TODO */ ].map(p => p.toLowerCase());


    /*********
    ** Read wiki data
    *********/
    // extract data from wiki table
    const modders = [];
    for (const row of document.querySelectorAll("#modders-table tbody tr")) {
        // read basic info
        const displayName = row.dataset.displayName;
        const discordName = row.dataset.discordName;
        const discordId = row.dataset.discordId;
        const nexusId = row.dataset.nexusId;
        const githubId = row.dataset.githubId;
        const redditName = row.dataset.redditName;
        const customAuthorUrl = row.dataset.customAuthorUrl;
        const modTypes = Array.from(row.querySelectorAll("td:nth-of-type(6) small")).map(p => p.innerText);

        // extract sample mod links
        const modLinks = [];
        {
            const linksCell = row.querySelector("td:nth-of-type(2)");

            for (const element of linksCell.childNodes) {
                // mod link
                if (element.localName == "a") {
                    modLinks.push({ name: element.textContent, url: element.href, appendAfter: "" });
                    continue;
                }

                // extra text after a mod link (like "(co-author)")
                const text = element.textContent.replace(/[,\s\n]+$/g, "").trim();
                if (text) {
                    if (!modLinks.length)
                    {
                        console.warn(`Ignored extra text '${text}' in mod links column because it appears before any link.`);
                        continue;
                    }

                    modLinks[modLinks.length - 1].appendAfter += text;
                }
            }
        }

        // add model
        modders.push({ displayName, discordName, discordId, nexusId, githubId, redditName, customAuthorUrl, modLinks, modTypes });
    }

    // sort by name
    modders.sort((a, b) => {
        const left = a.displayName.toLowerCase().replace(/[^a-z0-9]/g, "");
        const right = b.displayName.toLowerCase().replace(/[^a-z0-9]/g, "");

        if (left < right)
            return -1;
        if (left > right)
            return 1;
        return 0;
    });


    /*********
    ** Update & validate Discord data
    *********/
    {
        const wikiEntriesByDiscordId = {};

        for (const modder of modders) {
            // no Discord ID
            if (!modder.discordId) {
                console.warn(`The wiki entry with display name '${modder.displayName}' doesn't have the required Discord ID.`);
                continue;
            }

            // duplicate ID
            if (wikiEntriesByDiscordId[modder.discordId]) {
                console.warn(`The wiki entry with display name '${modder.displayName}' has Discord ID '${modder.discordId}', which is already assigned to the '${wikiEntriesByDiscordId[modder.discordId].displayName}' entry.`);
                continue;
            }

            // not in Discord data
            const discordName = discordUsersById[modder.discordId];
            if (!discordName) {
                console.warn(`The wiki entry with display name '${modder.displayName}' and Discord ID '${modder.discordId}' doesn't match a Discord account with the 'Mod Author' role.`);
                continue;
            }

            // update data
            modder.discordName = discordName;
            wikiEntriesByDiscordId[modder.discordId] = modder;
        }

        for (const discordId of Object.keys(discordUsersById)) {
            if (!wikiEntriesByDiscordId[discordId])
                console.warn(`Discord user ${discordId} (${discordUsersById[discordId]}) doesn't have a wiki entry.`);
        }
    }


    /*********
    ** Validate GitHub repo access
    *********/
    {
        // get GitHub IDs on wiki list
        const githubIds = {};
        for (const modder of modders) {
            if (modder.githubId)
                githubIds[modder.githubId.toLowerCase()] = true;
        }

        // warn for invalid IDs on access lists
        for (const id of usersWithMainRepoAccess) {
            if (!githubIds[id] && !usersWithSpecialRepoPermission.includes(id))
                console.warn(`Unknown GitHub user '${id}' has access to the main Stardew Valley decompile.`);
        }
        for (const id of usersWithAndroidRepoAccess) {
            if (!githubIds[id] && !usersWithSpecialRepoPermission.includes(id))
                console.warn(`Unknown GitHub user '${id}' has access to the Android Stardew Valley decompile.`);
        }
    }


    /*********
    ** Export updated wiki text
    *********/
    {
        // get mod link formatter
        function formatModLink(link) {
            if (!link)
                return "";

            // Chucklefish
            {
                const match = link.url.match(/^https:\/\/community\.playstarbound\.com\/resources\/(\d+)$/);
                if (match)
                    return `{{Chucklefish mod|${match[1]}|${link.name}}} ${link.appendAfter}`.trim();
            }

            // GitHub
            {
                const match = link.url.match(/^https:\/\/github\.com\/(.+)$/);
                if (match)
                    return `{{github|${match[1]}|${link.name}}} ${link.appendAfter}`.trim();
            }

            // ModDrop
            {
                const match = link.url.match(/^https:\/\/www\.moddrop\.com\/stardew-valley\/mods\/(\d+)$/);
                if (match)
                    return `{{ModDrop mod|${match[1]}|${link.name}}} ${link.appendAfter}`.trim();
            }

            // Nexus
            {
                const match = link.url.match(/^https:\/\/www\.nexusmods\.com\/stardewvalley\/mods\/(\d+)$/);
                if (match)
                    return `{{Nexus mod|${match[1]}|${link.name}}} ${link.appendAfter}`.trim();
            }

            // arbitrary link
            return `[${link.url} ${link.name}] ${link.appendAfter}`.trim();
        }

        // get author link formatter
        function formatAuthorLink(link) {
            if (!link)
                return "";

            // Chucklefish
            {
                const match = link.match(/^\[https:\/\/community\.playstarbound\.com\/resources\/authors\/(\d+) (.+)\]$/);
                if (match)
                    return `{{Chucklefish author|${match[1]}|${match[2]}}}`;
            }

            // ModDrop
            {
                const match = link.match(/^\[https:\/\/www\.moddrop\.com\/stardew-valley\/profile\/(\d+)\/mods (.+)\]$/);
                if (match)
                    return `{{ModDrop author|${match[1]}|${match[2]}}}`;
            }

            // arbitrary link
            return link;
        }

        // build new wikitext
        let wikitext = "";
        for (const modder of modders) {
            wikitext +=
                "{{/entry\n"
                + ` |display name = ${modder.displayName}\n`
                + ` |discord name = ${modder.discordName}\n`
                + ` |discord id   = ${modder.discordId}\n`
                + ` |nexus id     = ${modder.nexusId}\n`
                + ` |github id    = ${modder.githubId}\n`
                + ` |reddit name  = ${modder.redditName}\n`
                + ` |mod types    = ${modder.modTypes.join(", ")}\n`
                + ` |sample mod 1 = ${formatModLink(modder.modLinks[0])}\n`
                + ` |sample mod 2 = ${formatModLink(modder.modLinks[1])}\n`
                + ` |sample mod 3 = ${formatModLink(modder.modLinks[2])}\n`;

                if (modder.customAuthorUrl)
                    wikitext += ` |author url   = ${formatAuthorLink(modder.customAuthorUrl)}\n`;

                wikitext += "}}\n";
        }

        wikitext = wikitext.replace(/ +$/gm, "");

        console.log(wikitext);
    }
})();
