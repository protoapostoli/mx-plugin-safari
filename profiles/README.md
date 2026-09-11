# Profiles Guide: Logitech MX Creative Console

This folder provides tools, templates, and references for customizing and exporting profiles for the **MX Creative Keypad** and **MX Creative Dialpad**.

---

## Device Model Identifiers

Logitech Logi Options+ and Loupedeck use internal hardware codes for MX devices:

| Device | Internal ID | Controls Available |
| :--- | :--- | :--- |
| **MX Creative Keypad** | `Loupedeck70` | 9 Color LCD Buttons (`controlId: 0..8`) + 2 Page Buttons |
| **MX Creative Dialpad** | `Loupedeck71` | 1 Aluminum Dial (`rotatePages` `0`), 1 Roller (`rotatePages` `1`), 4 Tactile Buttons (`pressPages` `0..3`) |
| **Actions Ring** | `Loupedeck72` | Universal 8-button on-screen radial menu |

---

## Profile Archive Format (`.lp5`)

Profiles are packaged as standard `.zip` archives with the `.lp5` extension. When unpacked, a profile directory contains:

```
MyProfile/
├── ApplicationIcon.png       # App icon displayed in Options+
├── ApplicationInfo.json      # Maps profile to app bundle (e.g. com.apple.Terminal)
├── ProfileInfo.json          # Main control assignments (pressAction, rotateAction)
└── metadata/
    ├── LoupedeckPackage.yaml # Metadata (profile name, version, type: Profile5)
    └── ProfilePreview.json   # Graphical preview layout
```

---

## Working with Profiles

Use the built-in `mxdev` or `tools/profile_tool.py` CLI:

### 1. Inspect an existing profile on your Mac
```bash
mxdev profile inspect "$HOME/Library/Application Support/Logi/LogiPluginService/Applications/Loupedeck70/@_defaultmac/Profiles/C79DC52F8D1D4799B00377EF67F21A1B"
```

### 2. Extract an `.lp5` package to inspect/edit JSON:
```bash
mxdev profile extract path/to/profile.lp5 ./extracted_profile
```

### 3. Repack a directory into an `.lp5` file:
```bash
mxdev profile pack ./extracted_profile ./CustomProfile70.lp5
```

### 4. Exporting & Importing in Logi Options+
1. Open **Logi Options+**.
2. Select your MX Creative Keypad or Dialpad.
3. Open the profile dropdown at the top.
4. Click the three dots `...` next to any profile and select **Export profile**.
5. To load a custom profile, select **Add profile** -> **Import profile** and select your `.lp5` file.
