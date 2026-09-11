# Hardware Mapping: Logitech MX Creative Console

This document provides the hardware mapping reference for the **MX Creative Keypad** and **MX Creative Dialpad** when programming custom plugins and configuring profiles with the Logi Actions SDK.

---

## 1. MX Creative Keypad (`Loupedeck70`)

The Keypad features **9 full-color dynamic LCD display keys** arranged in a 3×3 grid, plus **2 mechanical paging buttons** below the grid.

### LCD Key Grid (3×3)

```
+---------------+---------------+---------------+
|               |               |               |
|  Control 0    |  Control 1    |  Control 2    |
|  (Top Left)   | (Top Center)  |  (Top Right)  |
|               |               |               |
+---------------+---------------+---------------+
|               |               |               |
|  Control 3    |  Control 4    |  Control 5    |
| (Middle Left) |   (Center)    | (Middle Right)|
|               |               |               |
+---------------+---------------+---------------+
|               |               |               |
|  Control 6    |  Control 7    |  Control 8    |
| (Bottom Left) |(Bottom Center)|(Bottom Right) |
|               |               |               |
+---------------+---------------+---------------+
           [ < Prev Page ]     [ Next Page > ]
```

### Control Specifications:
- **Control IDs**: `0` through `8` in `pressPages`.
- **Display Resolution**: 80 × 80 pixels per LCD key.
- **Image Format**: PNG (24-bit RGB or 32-bit RGBA) or SVG vector graphics.
- **Dynamic Updates**:
  - In C# SDK: Supported dynamically using `BitmapBuilder` (drawing text, shapes, gauges, or bitmaps) and calling `this.ActionImageChanged()`.
  - In Node.js SDK: Icons configured via `assets.yml` and icon templates (`.ict`).
- **Paging Keys**: Used by Logi Plugin Service to paginate between pages within the active workspace.

---

## 2. MX Creative Dialpad (`Loupedeck71`)

The Dialpad features a **large motorized aluminum dial**, a **stepped roller wheel**, and **4 round tactile action buttons**.

```
       +------------------------------------+
       |                                    |
       |             [ ROLLER ]             |
       |         (Rotate Control 1)         |
       |                                    |
       |     (Button 0)        (Button 1)   |
       |      Top Left          Top Right   |
       |                                    |
       |           /-----------\            |
       |          /             \           |
       |         |   ALUMINUM    |          |
       |         |     DIAL      |          |
       |         |  (Rotate 0)   |          |
       |          \             /           |
       |           \-----------/            |
       |                                    |
       |     (Button 2)        (Button 3)   |
       |     Bottom Left      Bottom Right  |
       |                                    |
       +------------------------------------+
```

### Rotary Controls (`rotatePages`):
- **Control ID 0 (Aluminum Dial)**:
  - High-resolution rotary encoder.
  - Generates positive `diff`/`event.tick` on clockwise rotation, negative on counter-clockwise.
  - Supports dial press (reset command in adjustments where `hasReset = true`).
  - Configurable in `LoupedeckSettings.ini`:
    - `Extended/ClicksMultiplier71_1`: sensitivity multiplier.
    - `Extended/ReverseClicks71_1`: invert rotation direction.
- **Control ID 1 (Roller Wheel)**:
  - Tactile stepped roller on the top edge.
  - Generates discrete step ticks (`diff = +1 / -1`).
  - Ideal for vertical scrolling, stepped volume, or brush size changes.

### Tactile Buttons (`pressPages`):
- **Control ID 0**: Top-left button (default: Undo).
- **Control ID 1**: Top-right button (default: Redo).
- **Control ID 2**: Bottom-left button (default: Custom / App Action).
- **Control ID 3**: Bottom-right button (default: Show Radial Actions Ring).

---

## 3. Loupedeck Settings & Identifiers

In `~/Library/Application Support/Logi/LogiPluginService/LoupedeckSettings.ini`:

```ini
CurrentApplication/Loupedeck70=@_defaultmac
CurrentApplication/Loupedeck71=@_defaultmac
CurrentApplication/Loupedeck72=@_defaultmac

Extended/ClicksMultiplier71_1=-10
Extended/DisplayBrightness70=40
Extended/ReverseClicks71_1=False

Loupedeck/DeveloperMode=True
```
