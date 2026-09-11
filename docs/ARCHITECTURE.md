# Architecture: Logi Actions SDK & Logi Plugin Service

This document describes how Logitech MX Creative Console hardware, host applications, Logi Plugin Service, and custom plugins interact.

---

## 1. System Overview

```
+------------------------------------+------------------------------------+
|       MX Creative Keypad           |        MX Creative Dialpad         |
|      (Loupedeck70: LCDs 0..8)      |      (Loupedeck71: Dial & Roller)  |
+------------------------------------+------------------------------------+
                 \                                  /
                  \                                /
                   v                              v
            +--------------------------------------------+
            |      Logitech Options+ Host App            |
            +--------------------------------------------+
                                  |
                                  v
            +--------------------------------------------+
            |      Logi Plugin Service (LPS)             |
            |   (/Applications/Utilities/...app)        |
            +--------------------------------------------+
                     /                          \
       (In-Process DLLs)              (WebSocket IPC)
                    /                            \
                   v                              v
         +--------------------+        +--------------------+
         |   C# Native DLL    |        |  Node.js / TS App  |
         | (PluginApi.dll)    |        | (@logitech/sdk)    |
         +--------------------+        +--------------------+
```

---

## 2. Core Components

### 1. Logi Plugin Service (LPS)
- Background daemon included with Logi Options+ that manages device communications, profiles, and plugins.
- macOS Location: `/Applications/Utilities/LogiPluginService.app/Contents/MonoBundle/`
- User Data & Plugins Directory: `~/Library/Application Support/Logi/LogiPluginService/`
- Plugin Discovery:
  - Scans `Plugins/` subdirectory.
  - Supports physical folders, `.link` text files (pointing to a build directory), or symbolic links.

### 2. Action Types
Every capability on the device is an **Action**:
1. **Command (`CommandAction` / `PluginDynamicCommand`)**:
   - Triggers once when a button is pressed or released.
   - Used on Keypad LCD keys and Dialpad tactile buttons.
2. **Adjustment (`AdjustmentAction` / `PluginDynamicAdjustment`)**:
   - Responds to continuous or incremental rotation.
   - Provides a `diff` or `event.tick` indicating direction and velocity.
   - Used on the Aluminum Dial and Roller Wheel.

### 3. Communication Protocol (Node.js SDK)
- Node plugins run as standalone Node.js processes.
- When Logi Plugin Service starts, it launches the Node host (`node22`).
- The plugin connects back to Logi Plugin Service over a local WebSocket server.
- Registration occurs upon connection:
  1. Plugin registers all `CommandAction` and `AdjustmentAction` instances.
  2. `await pluginSDK.connect();` completes the handshake.
  3. LPS routes device presses and dial rotations directly to the registered class callbacks.
