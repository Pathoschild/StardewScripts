/*

See documentation at https://github.com/Pathoschild/StardewScripts.

*/
(function() {
    let statuses = { "ok": 0, "workaround": 0, "soon": 0, "broken": 0 };
    let total = 0;
    $(".mod").each(function(i, entry) {
        entry = $(entry);

        var name = entry.data("name");
        var status = entry.data("status");
        var isOpenSource = !!entry.data("github") || !!entry.data("custom-source");

        switch (status)
        {
            case "ok":
            case "optional":
                statuses.ok++;
                total++;
                break;

            case "unofficial":
            case "workaround":
                statuses.workaround++;
                total++;
                break;

            case "broken":
                if (isOpenSource)
                    statuses.soon++;
                else
                    statuses.broken++;
                total++;
                break;

            default:
                console.log(`ignored mod '${name}', invalid status '${status}'.`);
                break;
        }
    });

    console.log(`${Math.round(((statuses.ok + statuses.workaround) / total) * 1000) / 10}% compatible`);
    console.log(
        "{{/barchart\n"
        + `  |ok         = ${statuses.ok}\n`
        + `  |workaround = ${statuses.workaround}\n`
        + `  |soon       = ${statuses.soon}\n`
        + `  |broken     = ${statuses.broken}\n`
        + `  |total      = ${total}\n`
        + "}}"
    );
})();
