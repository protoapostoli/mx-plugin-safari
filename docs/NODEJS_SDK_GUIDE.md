# Node.js / TypeScript SDK Guide

The **Node.js Actions SDK** allows developing custom plugins using modern JavaScript or TypeScript, with hot reloading and access to the complete npm ecosystem.

---

## 1. Quickstart

Inside this workspace, the bundled Node.js 22 runtime is ready in `./bin/`. Activate the environment:

```bash
source tools/dev-env.sh
```

Navigate to your plugin directory:

```bash
cd plugins/mx-custom-starter
```

Install dependencies and build:

```bash
npm install
npm run build
```

Link to Logi Plugin Service:

```bash
npm run link
```

Watch mode with hot reloading:

```bash
npm run watch
```

---

## 2. Defining Command Actions (LCD Keys & Buttons)

Command actions respond to button presses on the **Keypad LCD grid** or **Dialpad tactile buttons**:

```typescript
import { CommandAction } from '@logitech/plugin-sdk';
import { exec } from 'child_process';

export class MyKeypadAction extends CommandAction {
  readonly name = 'my_custom_key';
  readonly displayName = 'Custom Key';
  readonly description = 'Triggers an automated script';
  readonly groupName = 'My Commands';

  onKeyDown() {
    console.log('Key pressed!');
    exec('open -a "Google Chrome" "https://github.com"');
  }
}
```

---

## 3. Defining Adjustment Actions (Dials & Rollers)

Adjustment actions respond to rotation of the **Aluminum Dial** or **Roller Wheel**:

```typescript
import { AdjustmentAction, AdjustmentActionExecuteEvent } from '@logitech/plugin-sdk';

export class MyDialAdjustment extends AdjustmentAction {
  readonly name = 'my_dial_adjustment';
  readonly displayName = 'Fine Adjuster';
  readonly description = 'Adjusts values based on dial ticks';
  readonly groupName = 'My Adjustments';
  readonly hasReset = true; // Enables dial press reset

  private value = 0;

  execute(event: AdjustmentActionExecuteEvent) {
    const ticks = event.tick;
    this.value += ticks;
    console.log(`Dial moved by ${ticks} ticks. New total: ${this.value}`);
  }
}
```

---

## 4. Main Entry Point

```typescript
import { PluginSDK, LoggerLevel } from '@logitech/plugin-sdk';
import { MyKeypadAction } from './actions/my-keypad-action.js';
import { MyDialAdjustment } from './adjustments/my-dial-adjustment.js';

const sdk = new PluginSDK({ logLevel: LoggerLevel.INFO });

// Register all actions BEFORE connecting
sdk.registerAction(new MyKeypadAction());
sdk.registerAction(new MyDialAdjustment());

// Connect to Logi Plugin Service
await sdk.connect();
console.log('Plugin connected and ready!');
```

---

## 5. Live Debugging

To see `console.log()` output in real-time:

1. Enable Developer Mode:
   ```bash
   mxdev dev-mode on
   mxdev restart
   ```
2. Watch logs:
   ```bash
   mxdev logs
   ```
