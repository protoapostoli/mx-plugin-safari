# Safari Controller for Logitech MX Creative Console

[![macOS](https://img.shields.io/badge/Platform-macOS-black?logo=apple)](https://apple.com)
[![Hardware](https://img.shields.io/badge/Hardware-Logitech%20MX%20Creative%20Console-blue)](https://www.logitech.com)
[![Runtime](https://img.shields.io/badge/Runtime-.NET%2010%20%7C%20C%23-purple)](https://dotnet.microsoft.com)
[![Safari](https://img.shields.io/badge/Browser-Safari-006CFF?logo=safari)](https://apple.com/safari)

A dedicated native plugin and pre-configured application profile for the **Logitech MX Creative Console** (**MX Keypad** & **MX Dialpad**) that transforms your console into an interactive command deck for **Safari**.

![Safari Controller Keypad Layout](src/package/metadata/Icon256x256.png)

---

## ✨ Key Features

- 🔤 **Giant Marquee Active Tab Display**: The center LCD key displays the active Safari tab title in giant, vertically centered text with no clutter or border boxes. When titles exceed the key width, text scrolls smoothly in a readable loop.
- 📋 **Tap-to-Copy URL**: Tap the center key anytime to copy the active webpage URL straight to your clipboard with visual green confirmation.
- 🎛 **Precision Tab Scrubbing**: Rotate the aluminum dial to swiftly scrub and cycle between open Safari tabs.
- 📜 **Smooth Roller Scrolling**: Roll the tactile wheel for natural vertical scrolling up and down long webpages.
- 🧹 **Clean Reader / De-Clutter**: Instantly strip newsletter popups, cookie consent overlays, paywall modals, and sticky headers with a single keypress.
- ⚡ **Native Safari Shortcuts**: Dedicated keys for Tab Duplication, Reader Mode (`Cmd+Shift+R`), Reopen Closed Tab (`Cmd+Shift+T`), History Navigation, and Page Reload.

---

## 🎮 Hardware Layout & Controls

### MX Keypad (3×3 LCD Keys)

```
+--------------------+--------------------+--------------------+
|     [Key 0]        |     [Key 1]        |     [Key 2]        |
|     New Tab        |    Close Tab       |   Duplicate Tab    |
+--------------------+--------------------+--------------------+
|     [Key 3]        |     [Key 4]        |     [Key 5]        |
|  Clean Reader      |  ACTIVE TAB TITLE  |    Toggle Native   |
|   (De-Clutter)     |  (Giant Marquee)   |     Reader Mode    |
+--------------------+--------------------+--------------------+
|     [Key 6]        |     [Key 7]        |     [Key 8]        |
|   History Back     |    Reload Page     |   History Forward  |
+--------------------+--------------------+--------------------+
```

| Key | Control | Action | Description |
| :---: | :--- | :--- | :--- |
| **0** | `New Tab` | Press | Open a new tab (`Cmd+T`) |
| **1** | `Close Tab` | Press | Close current tab (`Cmd+W`) |
| **2** | `Duplicate Tab` | Press | Duplicate active tab in background |
| **3** | `Clean Reader` | Press | Strips modals, cookie banners & sticky overlays |
| **4** | **Active Tab Display** | **Dynamic Key** | **Giant scrolling tab title; Tap to copy URL** |
| **5** | `Toggle Reader Mode` | Press | Toggle native Safari Reader (`Cmd+Shift+R`) |
| **6** | `History Back` | Press | Navigate back in history (`Cmd+[`) |
| **7** | `Reload Page` | Press | Reload active page (`Cmd+R`) |
| **8** | `History Forward` | Press | Navigate forward in history (`Cmd+]`) |

### MX Dialpad

| Control | Action | Description |
| :--- | :--- | :--- |
| **Aluminum Dial** | Rotate Left / Right | Scrub & switch between open tabs |
| **Roller Wheel** | Roll Up / Down | Smooth vertical webpage scrolling |
| **Top-Left Button (0)** | Press | Reopen last closed tab (`Cmd+Shift+T`) |
| **Top-Right Button (1)** | Press | Open new tab (`Cmd+T`) |
| **Bottom-Left Button (2)** | Press | Clean Reader / De-Clutter script |
| **Bottom-Right Button (3)** | Press | Actions Ring Overlay |

---

## 🚀 1-Step Quick Installation

### Prerequisites
- **macOS** 13+ (Apple Silicon or Intel)
- **Logitech Options+** with your MX Creative Console configured
- **Apple Safari**
- **.NET SDK** *(If not already installed, the installer will automatically download it for you)*

### Install Command
Open your terminal and run:

```bash
git clone https://github.com/protoapostoli/mx-plugin-safari.git
cd mx-plugin-safari
./install.sh
```

That's it! The script will:
1. Compile the plugin for your system.
2. Package the binaries and pre-configured application profiles.
3. Deploy directly into `~/Library/Application Support/Logi/LogiPluginService/Plugins/Safari/`.
4. Clear stale icon caches and restart the Logitech background service.

---

## 🔐 First-Time Permissions & Settings

### 1. macOS Automation Permission
When you first bring Safari into focus, macOS will prompt you to allow `LogiPluginService` to control Safari via AppleScript:
1. Click **Allow** on the macOS system alert.
2. If you missed or clicked "Don't Allow", enable it manually:
   - Go to **System Settings > Privacy & Security > Automation**.
   - Under **LogiPluginService**, ensure **Safari** is toggled **ON**.

### 2. Safari: Allow JavaScript from Apple Events (Required for Clean Reader)
Safari blocks external applications from running JavaScript inside tabs by default. To enable the **Clean Reader (De-Clutter)** feature and smooth DOM scrolling:
1. Open **Safari** and go to **Settings...** (`Cmd+,`).
2. Click the **Advanced** tab and check **"Show features for web developers"** (at the bottom).
3. In the macOS top menu bar, click the newly visible **Develop** menu.
4. Click **"Allow JavaScript from Apple Events"** (authenticate with Touch ID / password if prompted).
Once enabled, Clean Reader will instantly strip modals, overlays, and clutter on command!

---

## 🛠 Development & Customization

### Rebuilding After Changes
If you modify any source files in `src/`, apply updates with a single command:

```bash
./tools/build-and-restart.sh
```

### Customizing the Active Tab Display
Open [`src/Actions/SafariActiveTabKey.cs`](src/Actions/SafariActiveTabKey.cs) to tweak display constants:

```csharp
public const Int32 FontSize = 26;          // Font point size (24-28)
public const Int32 VisibleCharCount = 5;   // Characters visible across the 80x80 key
public const Int32 ScrollIntervalMs = 230; // Milliseconds per character step
public const Int32 PauseTicks = 6;         // Delay before scrolling starts (~1.4s)
```

### Real-Time Debug Logs
To watch the active tab poller and marquee updates in real time:

```bash
tail -f /tmp/safari_keypad.log
```

---

## 📂 Project Structure

```
mx-plugin-safari/
├── src/                               # Plugin C# source code & metadata
│   ├── Actions/                       # Dynamic Keypad LCD actions (Active tab marquee, buttons)
│   ├── Adjustments/                   # Dialpad controls (Tab scrubbing, roller scroll)
│   ├── Helpers/                       # Safari AppleScript automation bridge
│   ├── package/                       # Packaged icons & .lp5 application profiles
│   ├── SafariApplication.cs           # Client application binding
│   ├── SafariPlugin.cs                # Plugin lifecycle & tab polling loop
│   └── SafariPlugin.csproj            # .NET project definition
├── tools/                             # Build & packaging utilities
│   ├── assemble_safari_plugin.py      # Packages plugin distribution directory
│   ├── build-and-restart.sh           # Quick rebuild & restart script
│   └── generate_safari_profiles.py    # Generates .lp5 profiles from JSON definitions
├── install.sh                         # 1-step installer for end users
└── README.md                          # Documentation
```

---

## 📄 License

MIT License. Feel free to use, modify, and distribute.
