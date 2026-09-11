# Logitech MX Creative Console Development Environment

A development environment for customizing profiles, actions, and plugins for the **Logitech MX Creative Console** (both the **MX Keypad** and **MX Dialpad**) using the **Logi Actions SDK**.

---

## Hardware Overview & Control Mapping

| Device | Model ID | Controls |
| :--- | :--- | :--- |
| **MX Creative Keypad** | `Loupedeck70` | 9 Color LCD keys (`0..8` in 3×3 grid, 80×80px) + 2 Paging Buttons |
| **MX Creative Dialpad** | `Loupedeck71` | Large Precision Aluminum Dial (`0`), Roller Wheel (`1`), 4 Tactile Buttons (`0..3`) |
| **Actions Ring** | `Loupedeck72` | Universal 8-button on-screen radial overlay |

See [docs/HARDWARE_MAPPING.md](docs/HARDWARE_MAPPING.md) for full pinouts and diagrams.

---

## Quickstart

### 1. Activate Environment
No global installs required! The environment automatically hooks into the standalone Node 22 runtime bundled inside your existing Logi Plugin Service:

```bash
source tools/dev-env.sh
```

This adds `./bin` to your `$PATH`, providing `node`, `npm`, `npx`, and the `mxdev` CLI.

### 2. Check Device & Service Status
```bash
mxdev status
```

### 3. Explore the Starter Plugin
The starter plugin in [plugins/mx-custom-starter](plugins/mx-custom-starter) is wired for both the Keypad and Dialpad:
- **Keypad LCD Buttons**: System notifications, terminal launcher, counter reset.
- **Dialpad Buttons**: Smart Undo/Redo, media controls.
- **Aluminum Dial**: Continuous rotary counter & scrub controller.
- **Roller Wheel**: Volume & stepped scrolling.

To link the starter plugin into Logi Plugin Service:
```bash
mxdev link plugins/mx-custom-starter
```

### 4. Live Debugging
Enable Developer Mode to inspect `console.log()` and diagnostic messages in real-time:
```bash
mxdev dev-mode on
mxdev restart
mxdev logs
```

---

## Project Structure

```
.
├── bin/                          # CLI wrappers for node, npm, npx, and mxdev
├── plugins/
│   ├── mx-custom-starter/        # Runnable starter plugin for Keypad & Dialpad
│   └── templates/                # Ready-to-copy templates for Node.js and C# (.NET 8)
├── profiles/
│   ├── keypad-70/                # Keypad reference layouts
│   ├── dialpad-71/               # Dialpad reference layouts
│   └── README.md                 # Profile guide
├── tools/                        # Helper scripts & profile inspection tools
├── docs/                         # Comprehensive documentation
│   ├── ARCHITECTURE.md           # System and IPC architecture
│   ├── HARDWARE_MAPPING.md       # Hardware control IDs & layouts
│   ├── NODEJS_SDK_GUIDE.md       # Node.js/TypeScript SDK development
│   ├── CSHARP_SDK_GUIDE.md       # C# SDK & dynamic LCD drawing
│   └── PROFILES_GUIDE.md         # Profiles (.lp5) & virtual plugins
└── Makefile                      # Convenient make tasks (make status, make logs, etc.)
```

---

## Useful Commands

```bash
mxdev status             # Check LogiPluginService, installed plugins, and active devices
mxdev list               # List all installed/linked plugins
mxdev link <plugin>      # Link a plugin to LogiPluginService
mxdev unlink <plugin>    # Unlink a plugin
mxdev logs [plugin]      # Tail logs in real time
mxdev restart            # Gracefully restart LogiPluginService
mxdev dev-mode [on|off]  # Toggle DeveloperMode in LoupedeckSettings.ini
mxdev profile list       # List active device profiles on this Mac
mxdev profile inspect    # Inspect an .lp5 archive or profile folder
```
