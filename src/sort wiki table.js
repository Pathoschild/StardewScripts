/*

When run on https://stardewvalleywiki.com/Modding:Mod_compatibility?action=edit&section=3, this
script logs two plaintext lists: the mod names in their current order, and the mod names when
sorted alphabetically ignoring case and non-alphanumeric characters.

The two lists can be compared using a tool like https://www.diffchecker.com/diff to find mods which
aren't sorted correctly.

*/
(function() {
    /**
     * Parse a mod name into a comparable value.
     * @param name The mod name to format.
     */
    const getComparable = function(name) {
        // sort by main mod name
        name = name.split(',')[0];

        // reverse HTML encoding
        {
            const element = document.createElement("div");
            element.innerHTML = name;
            name = element.innerText;
        }

        // get case-insensitive alphanumeric string
        return name
            .toLowerCase()
            .replace(/[^a-z0-9]/gm, '');
    };

    const compareModNames = function(a, b) {
        const comparableA = getComparable(a);
        const comparableB = getComparable(b);

        // special case: if they have a common prefix, list shorter one first
        if (comparableA.startsWith(comparableB) || comparableB.startsWith(comparableA))
            return comparableA - comparableB;

        // else sort alphanumerically
        if (comparableA < comparableB)
            return -1;
        else if (comparableA > comparableB)
            return 1;
        return 0;
    }

    const modNames = $("#mod-list").find("tr.mod").get().map(row => $(row).attr("data-name"));

    const sortedNames = [...modNames];
    sortedNames.sort(compareModNames);

    console.log({ current: modNames.join("\n"), sorted: sortedNames.join("\n") });
})();
