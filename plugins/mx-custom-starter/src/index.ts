import { PluginSDK, LoggerLevel } from '@logitech/plugin-sdk';
import { SystemInfoAction, DevWorkflowAction, CounterResetAction } from './actions/keypad-buttons.js';
import {
  DialpadUndoAction,
  DialpadRedoAction,
  DialpadPlayPauseAction,
  DialpadCustomWorkflowAction
} from './actions/dialpad-buttons.js';
import { PrecisionDialAdjustment, TimelineScrubAdjustment } from './adjustments/dial-adjustments.js';
import { PrecisionRollerAdjustment, RollerScrollAdjustment } from './adjustments/roller-adjustments.js';

async function main() {
  console.log('========================================================');
  console.log('  Starting MX Custom Starter Plugin for MX Creative Console');
  console.log('========================================================');

  // Initialize the Logi Actions PluginSDK
  const sdk = new PluginSDK({ logLevel: LoggerLevel.INFO });

  // 1. Shared state instances
  const dialCounter = new PrecisionDialAdjustment();
  const counterReset = new CounterResetAction(() => dialCounter.resetCounter());

  // 2. Register MX Keypad LCD actions (Loupedeck70: controls 0..8)
  sdk.registerAction(new SystemInfoAction());
  sdk.registerAction(new DevWorkflowAction());
  sdk.registerAction(counterReset);

  // 3. Register MX Dialpad button actions (Loupedeck71: controls 0..3)
  sdk.registerAction(new DialpadUndoAction());
  sdk.registerAction(new DialpadRedoAction());
  sdk.registerAction(new DialpadPlayPauseAction());
  sdk.registerAction(new DialpadCustomWorkflowAction());

  // 4. Register MX Dialpad rotary adjustments (Loupedeck71: dial = 0, roller = 1)
  sdk.registerAction(dialCounter);
  sdk.registerAction(new TimelineScrubAdjustment());
  sdk.registerAction(new PrecisionRollerAdjustment());
  sdk.registerAction(new RollerScrollAdjustment());

  // 5. Connect to Logi Plugin Service via WebSocket IPC
  try {
    await sdk.connect();
    console.log('[MX Custom Starter] Successfully connected to Logi Plugin Service!');
  } catch (error) {
    console.error('[MX Custom Starter] Failed to connect to Logi Plugin Service:', error);
  }
}

main().catch((err) => {
  console.error('[MX Custom Starter] Unhandled fatal error:', err);
  process.exit(1);
});
