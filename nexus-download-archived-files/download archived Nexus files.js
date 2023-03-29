/*

See documentation at https://github.com/Pathoschild/StardewScripts.

*/
javascript:(function() {
    const gameId = $("#section").attr("data-game-id");
    const downloadElements = $("#file-container-archived-files [data-id]");

    for (let element of downloadElements) {
        element = $(element);
        const description = element.find(".files-description");
        const fileId = element.attr("data-id");

        const html = `
            <div class="tabbed-block">
                <ul class="accordion-downloads clearfix">
                    <li>
                        <a class="btn inline-flex popup-btn-ajax" href="/Core/Libs/Common/Widgets/DownloadPopUp?id=${fileId}&nmm=1&game_id=${gameId}">
                            <svg title="" class="icon icon-nmm">
                                <use xlink:href="https://www.nexusmods.com/assets/images/icons/icons.svg#icon-nmm"></use>
                            </svg>
                            <span class="flex-label">Mod manager download</span>
                        </a>
                    </li>
                    <li>
                    </li>
                    <li>
                        <a class="btn inline-flex popup-btn-ajax" href="/Core/Libs/Common/Widgets/DownloadPopUp?id=${fileId}&game_id=${gameId}">
                            <svg title="" class="icon icon-manual">
                                <use xlink:href="https://www.nexusmods.com/assets/images/icons/icons.svg#icon-manual"></use>
                            </svg>
                            <span class="flex-label">Manual download</span>
                        </a>
                    </li>
                </ul>
            </div>
        `;

        $(html).insertAfter(description);
    }
})();void(0);
