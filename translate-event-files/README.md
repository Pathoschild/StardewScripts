[← back to repo](../)

This script pulls text strings out of Stardew Valley events and generates translatable versions
using Content Patcher's `{{i18n}}` token.

# Usage
To use this script:

1. Open the script file in [LINQPad](https://www.linqpad.net).
2. Change the settings under 'configuration' near the top.
3. Click the execute (▶) button to generate the translated event.

# Event file format
The script automatically recognizes the three common formats:

1. Raw event files loaded using `"Action": "Load"`:
   ```json
   {
       "event A": "script",
       "event B": "script",
       ...
   }
   ```

2. A file loaded via `"Action": "Include"`:
   ```json
   {
       "Changes": [
           {
               "Action": "EditData",
                "Target": "Data/Events/Town",
                "Entries": {
                    "event A": "script",
                    "event B": "script",
                    ...
                }
            },
            ...
        ]
   }
   ```
3. A Legacy file loaded using `"Action": "EditData"` with `FromFile`:
   ```json
   {
       "Entries": {
           "event A": "script",
           "event B": "script",
           ...
       }
   }
   ```

If you have events in a different format, you can copy & paste them into the raw event file format
to use this script.
