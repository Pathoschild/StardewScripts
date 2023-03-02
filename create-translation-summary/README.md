[← back to repo](../)

This script generates a translation summary for a repository's README.md by scanning the
translation files in the repository. This makes it much easier for translators to see what's left
to translate.

# Usage
To use this script:

1. Open the script file in [LINQPad](https://www.linqpad.net).
2. Change the settings under 'configure' at the top.
3. Click the execute (▶) button to generate the translation summary.

# Assumptions
The script makes these assumptions:

1. A folder containing `manifest.json` and `i18n/default.json` is a mod.
2. If there are any `i18n` folders under that, they're treated as part of that mod too.
3. A translation file is incomplete if it's missing any keys defined in `default.json`, or if it
   has a comment in the form `// TODO`.
