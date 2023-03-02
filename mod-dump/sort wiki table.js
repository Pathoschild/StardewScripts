/*

See documentation at https://github.com/Pathoschild/StardewScripts.

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

    // get mod names
    const modNames = $("#mod-list").find("tr.mod").get().map(row => $(row).attr("data-name"));

    // get sorted list
    const sortedNames = [...modNames];
    sortedNames.sort(compareModNames);

    // list incorrectly sorted values
    let unsorted = [];
    for (let i = 0; i < modNames.length; i++)
    {
        if (modNames[i] != sortedNames[i])
            unsorted.push({ position: i + 1, current: modNames[i], sorted: sortedNames[i] });
    }

    // log summary
    if (!unsorted.length)
        console.log("List is correctly sorted!");
    else
        console.warn("Found unsorted values:", unsorted);
})();
