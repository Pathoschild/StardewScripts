[← back to repo](../)

This script can be run on Nexus to clean up messy notification lists. It will...

* hide all but the latest new-comments notification for each mod:
  ```
  3 new comments on ExampleMod (2 hours ago)  <-- keep this one, hide the rest
  2 new comments on ExampleMod (1 day ago)
  3 new comments on ExampleMod (2 days ago)
  5 new comments on ExampleMod (3 days ago)
  ```
* optionally hide notifications of specific types (e.g. friend requests);
* optionally hide notifications from specific mods;
* optionally mark hidden notifications as read.

# Usage
To use this script:

1. Optionally configure the settings near the top of the script.
2. Click the notification icon on Nexus.
3. Click 'Load more' until all unread notifications are visible.
4. Run the script as a bookmarklet or in the JavaScript console.
