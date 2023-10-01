/*

See documentation at https://github.com/Pathoschild/StardewScripts.

*/
javascript:(function() {
    /*******************
    ** Config data types
    *******************/
    /**
     * The overall type for a notification.
     */
    const NotificationType = Object.freeze({
        /**
         * A request from another user to add them as a friend.
         */
        FriendRequest: "friend_request",

        /**
         * New comments added to the mod's 'posts' tab.
         */
        ModComments: "comments",

        /**
         * New issues or issue replies added to the mod's 'bugs' tab.
         */
        ModIssues: "issues",

        /**
         * New mod downloads added to the mod's 'files' tab.
         */
        ModIssues: "issues",

        /**
         * A mod-related notification that's not covered by another type (e.g. mod page edits).
         */
        ModOther: "mod_other",

        /**
         * New games added to Nexus Mods.
         */
        NewGames: "new_games",

        /**
         * An announcement or blog post from Nexus Mods.
         */
        News: "news",

        /**
         * An invalid or unknown notification type.
         */
        Unknown: "unknown",

        /**
         * A Donation Points notification.
         */
        Wallet: "wallet"
    });


    /*******************
    ** Configuration
    *******************/
    /**
     * Whether to mark filtered comments as read. If false, they'll be highlighted so you can mark
     * them as read instead.
     */
    const markAsRead = true;

    /**
     * Configures which notifications you want to ignore. These will be either marked as read &
     * hidden, or highlighted, depending on the 'markAsRead' setting.
     */
    const ignoreList = {
        /**
         * The notification types to ignore.
         */
        types: {
            [NotificationType.FriendRequest]: true,
            [NotificationType.ModComments]: false,
            [NotificationType.ModIssues]: false,
            [NotificationType.ModUpdates]: true,
            [NotificationType.ModOther]: false,
            [NotificationType.NewGames]: false,
            [NotificationType.News]: false,
            [NotificationType.Unknown]: false,
            [NotificationType.Wallet]: false
        },

        /**
         * The mod IDs to ignore. The values are just for reference, the code will only use the keys.
         */
        mods: {
            /* entoarox */
            2270: "Advanced Location Loader",
            2272: "Custom Paths",
            2269: "Entoarox Framework",
            2271: "Extended Minecart",
            2277: "Faster Paths",
            2276: "Furniture Anywhere",
            2274: "More Animals",
            /*2273: "Seasonal Immersion",*/
            2278: "Shop Expanded",
            2268: "Stardew Patcher",
            /*2275: "XNB Loader",*/

            /* Jierishi */
            7194: "Pierre Sells Tree Fertilizer",

            /* omegasis */
            435: "Advanced Save Backup",
            492: "Billboard Anywhere",
            520: "Happy Birthday",
            433: "Night Owl",
            444: "Save Anywhere",
            425: "Stardew Symphony",

            /* spacechase0 */
            3283: "Animal Social Menu",
            3379: "Another Hunger Mod",
            14448: "A Quality Mod",
            14570: "Backstory Questions Framework",
            5096: "Better Meteorites ",
            2012: "Better Shop Menu",
            1845: "Bigger Backpack",
            7530: "Bigger Craftables",
            17550: "Blahaj Blast",
            5099: "Bug Net",
            7636: "Capstone Professions",
            1333: "Carry Chest",
            3905: "Combat Level Damage Scaler",
            3101: "Console Code",
            /*10545: "Content Code", */
            3853: "Content Patcher Animations",
            522: "Cooking Skill",
            12801: "Controller Radial Keybindings",
            1255: "Custom Critters",
            3849: "Custom NPC Fixes",
            1099: "Customize Exterior",
            12706: "Dimensional Pockets",
            7635: "Displays",
            9365: "Dynamic Game Assets",
            509: "Experience Bars",
            1493: "Extended Reach",
            1229: "Flower Color Picker",
            6131: "Flower Rain",
            /*5098: "Generic Mod Config Menu",*/
            6577: "Hybrid Crop Engine",
            1720: "Json Assets",
            1844: "Jump Over",
            7437: "Junimos Accept Cash",
            14262: "Literally Can't Even",
            521: "Luck Skill",
            2007: "Magic",
            7831: "Mana Bar",
            17436: "MaYo Rain",
            10673: "Monsters - The Framework",
            10612: "Moon Misadventures",
            2757: "More Buildings",
            5263: "More Giant Crops",
            1702: "More Grass Starter",
            2054: "More Rings",
            7436: "MultiFertilizer",
            10786: "New Game Plus",
            1315: "Object Time Left",
            13852: "Giant Omelet",
            7684: "Preexisting Relationship",
            3378: "Profit Calculator",
            7455: "Pyromancer's Journey",
            9386: "Realtime Minimap",
            12029: "Responsive MP Knockback",
            605: "Rush Orders",
            9386: "Realtime Minimap",
            14263: "Sharing is Caring",
            14267: "Sizable Fish",
            14452: "Skill Training",
            1152: "Sleepy Eye",
            1348: "SpaceCore",
            2755: "Spenny",
            14984: "Spenny Deluxe",
            17326: "Spennying Wheel",
            7532: "Statue of Generosity",
            /*9418: "Super Hopper",*/
            6688: "Surfing Festival",
            5062: "Theft of the Winter Star",
            500: "Three-Heart Dance Partner",
            5097: "Throwable Axe",
            17139: "Unhinged Mayo Jar",
    
            /* TehPers */
            866: "Teh's Fishing Overhaul"
        }
    };


    /*******************
    ** Script
    *******************/
    const tracked = {};

    /**
     * A parsed representation of a notification element in the list.
     * @property {jQuery} element The underlying HTML element.
     * @property {boolean} isRead Whether the notification is marked as read.
     * @property {NotificationType} type The notification type.
     * @property {number | null} modId The Nexus mod ID, if this is a mod notification.
     * @property {string} modName The name of the mod for which the notification was raised, if applicable.
     */
    const NotificationRow = class {
        /**
         * Construct an instance.
         * @param {jQuery} element The underlying HTML element.
         */
        constructor(element) {
            this.element = element = $(element);
            this.isRead = element.is(".notification-read");

            /* parse notification URL */
            const rawTarget = element.find(".notification-link:first").attr("href");
            if (rawTarget && rawTarget !== "null")
            {
                const target = new URL(rawTarget);
                const tab = target.searchParams.get("tab");
                const path = target.pathname.replace(/^\/+|\/+$/g, "").split("/");
                if (path[1] == "mods")
                {
                    // mod ID
                    this.modId = parseInt(path[2]);
                    if (isNaN(this.modId))
                        this.modId = null;

                    // type
                    switch (tab)
                    {
                        case "bugs":
                            this.type = NotificationType.ModIssues;
                            break;

                        case "files":
                            this.type = NotificationType.ModUpdates;
                            break;

                        case "posts":
                            this.type = NotificationType.ModComments;
                            break;

                        default:
                            this.type = NotificationType.ModOther;
                            break;
                    }
                }
                else {
                    switch (path[0])
                    {
                        case "modrewards":
                            this.type = NotificationType.Wallet;
                            break;

                        case "users":
                            this.type = tab == "friends" ? NotificationType.FriendRequest : NotificationType.Unknown;
                            break;

                        case "news":
                            this.type = NotificationType.News;
                            break;

                        case "games":
                            this.type = NotificationType.NewGames;
                            break;

                        default:
                            this.type = NotificationType.Unknown;
                            break;
                    }
                }
            }
            else
                this.type = NotificationType.Unknown;

            /* get notification type from text if needed */
            if (this.type == NotificationType.Unknown) {
                const text = element.find(".notification-row").text();

                if (text.includes("mods are now available")) {
                    this.type = NotificationType.NewGames;
                }
            }

            /* extract mod text */
            const row = element.find(".notification-row");
            const rowHtml = row.html();
            const rowMatch = rowHtml.match("<span[^<>]*>.+?</span> ([a-z ]+) <span[^<>]*>(.+?)</span>"); /* span for user or count, text for action, then span for mod name */
            this.modName = rowMatch?.[2] ?? null;
        }

        hide() {
            this.element.attr("style", "display: none");
        }

        markAsRead() {
            this.isRead = true;
            this.element.find(".notification-indicator-box button").click();
        }

        highlight() {
            this.element.attr("style", "border: 3px solid red");
        }
    }

    /* clean notifications */
    for (let notif of $(".notification-wrapper:visible"))
    {
        /* parse notification */
        notif = new NotificationRow(notif);
        const duplicateKey = `${notif.type}:${notif.modId ?? notif.modName}`;

        /* hide if read */
        if (notif.isRead)
        {
            notif.hide();
            continue;
        }

        /* log unknown type */
        if (notif.type == NotificationType.Unknown)
        {
            console.warn("Unknown notification type.", notif);
        }

        /* mark as read */
        if (!notif.isRead)
        {
            const isIgnored =
                notif.modId in ignoreList.mods
                || ignoreList.types[notif.type]
                || false;
            const isDuplicate = duplicateKey in tracked;

            tracked[duplicateKey] = true;

            if (isIgnored || isDuplicate)
            {
                if (markAsRead) {
                    notif.markAsRead();
                }
                else {
                    notif.highlight();
                }
            }
        }

        /* hide read */
        if (notif.isRead)
        {
            notif.hide();
        }
    }
})();void(0);
