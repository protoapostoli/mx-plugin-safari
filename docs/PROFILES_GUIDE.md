# Profiles Guide (.lp5 & Virtual Plugins)

This guide covers how device profiles, application targeting, and virtual shortcut plugins work on the Logitech MX Creative Console.

---

## 1. How Profiles Work

A **Profile** binds actions (from plugins or system defaults) to the physical hardware controls of a specific device:
- Keypad profiles bind actions to `pressPages` (`controlId: 0..8`).
- Dialpad profiles bind actions to `rotatePages` (`controlId: 0` for dial, `1` for roller) and `pressPages` (`controlId: 0..3`).

---

## 2. Default Profile Naming Conventions

When packaging default profiles inside a plugin for automatic installation:

| Target Device | File Name |
| :--- | :--- |
| **Logitech MX Creative Keypad** | `DefaultProfile70.lp5` |
| **Logitech MX Creative Dialpad** | `DefaultProfile71.lp5` |
| **Logitech Actions Ring** | `DefaultProfile72.lp5` |
| Loupedeck CT | `DefaultProfile20.lp5` |
| Loupedeck Live | `DefaultProfile30.lp5` |
| Loupedeck Live S | `DefaultProfile50.lp5` |

For OS-specific profiles:
- Mac: `DefaultProfile70mac.lp5`
- Win: `DefaultProfile70win.lp5`

---

## 3. Virtual Plugins (`virtual/actions.json`)

If you want to create custom application controls with shortcuts and icons without compiling any code, you can create a **Virtual Plugin** (such as the official Excel and Word plugins).

Folder structure:
```
MyVirtualPlugin/
├── metadata/
│   ├── Icon256x256.png
│   └── LoupedeckPackage.yaml
├── virtual/
│   └── actions.json
├── actionicons/
│   └── MyAction.svg
└── profiles/
    ├── DefaultProfile70.lp5
    └── DefaultProfile71.lp5
```

In `metadata/LoupedeckPackage.yaml`:
```yaml
type: plugin4
name: MyVirtualPlugin
displayName: My App Controls
version: 1.0.0
pluginType: virtual
applicationPatterns:
  bundleNamePattern: com.mycompany.myapp
```

In `virtual/actions.json`:
```json
{
  "languages": ["en-US"],
  "primaryOperatingSystem": "Mac",
  "groupNames": ["Editing", "Navigation"],
  "commands": [
    {
      "name": "FormatBold",
      "displayName": "Bold Text",
      "description": "Applies bold formatting",
      "groupName": "Editing",
      "shortcuts": {
        "en-US": "Cmd+B"
      }
    }
  ]
}
```
Logi Plugin Service will automatically generate the actions, show them in Logi Options+, and trigger the shortcut keys when pressed!
