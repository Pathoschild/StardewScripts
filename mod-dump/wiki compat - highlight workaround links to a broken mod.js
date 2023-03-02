/*

See documentation at https://github.com/Pathoschild/StardewScripts.

*/
(function() {
    let isFirst = true;

    for (let modRow of document.querySelectorAll(".mod")) {
        /* get summary element */
        const summary = modRow.querySelector(".mod-beta-summary") ?? modRow.querySelector(".mod-summary");
        if (!summary)
        {
            console.error(`Can't get summary element for mod ID '${link.id}'.`);
            continue;
        }

        /* handle anchor links */
        for (let link of summary.querySelectorAll("a[href^='#']")) {
            /* get link */
            const href = link.getAttribute("href");
            if (href == '#') {
            	continue;
            }

            /* get target row */
            const targetRow = document.querySelector(`[id="${href.substr(1)}"]`);
            if (!targetRow)
            {
                console.error(`Invalid target '${link.href}'.`);
                continue;
            }

            /* get status */
            const status = targetRow.getAttribute("data-beta-status") ?? targetRow.getAttribute("data-status");
            if (!status)
            {
                console.error(`Can't read status for '${link.href}' target.`);
                continue;
            }

            /* highlight links to a broken mod */
            if (status === "broken" || status === "abandoned" || status == "obsolete" || status === "unknown")
            {
                link.setAttribute("style", "border: 5px solid red;");

                if (isFirst) {
                    window.scrollBy(0, link.getBoundingClientRect().top - 30);
                    isFirst = false;
                }
            }
        }
    }
})();
