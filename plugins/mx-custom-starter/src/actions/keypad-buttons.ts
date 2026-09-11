import { CommandAction } from '@logitech/plugin-sdk';
import { exec } from 'child_process';

/**
 * KeypadLCDAction - Represents an action mapped to one of the 9 LCD keys on the MX Keypad.
 * LCD Key buttons trigger onKeyDown when pressed.
 */
export class SystemInfoAction extends CommandAction {
  readonly name = 'mx_keypad_sysinfo';
  readonly displayName = 'System Info';
  readonly description = 'Triggers a macOS notification with current system date/time and uptime';
  readonly groupName = 'MX Keypad Controls';

  onKeyDown() {
    console.log('[SystemInfoAction] Pressed on MX Keypad!');
    const applescript = `display notification "MX Keypad action triggered successfully!" with title "MX Creative Console" subtitle "System Info"`;
    exec(`osascript -e '${applescript}'`, (err) => {
      if (err) {
        console.error('[SystemInfoAction] Error executing notification:', err);
      }
    });
  }
}

/**
 * DevWorkflowAction - Quick developer workflow trigger
 */
export class DevWorkflowAction extends CommandAction {
  readonly name = 'mx_keypad_dev_workflow';
  readonly displayName = 'Launch Dev Terminal';
  readonly description = 'Opens Terminal in the project directory';
  readonly groupName = 'MX Keypad Controls';

  onKeyDown() {
    console.log('[DevWorkflowAction] Opening development workspace...');
    const projectDir = process.cwd();
    exec(`open -a Terminal "${projectDir}"`, (err) => {
      if (err) {
        console.error('[DevWorkflowAction] Error opening Terminal:', err);
      }
    });
  }
}

/**
 * CounterResetAction - Resets the shared rotation counter
 */
export class CounterResetAction extends CommandAction {
  readonly name = 'mx_keypad_counter_reset';
  readonly displayName = 'Reset Dial Counter';
  readonly description = 'Resets the value of the custom precision dial counter';
  readonly groupName = 'MX Keypad Controls';

  private resetCallback?: () => void;

  constructor(resetCallback?: () => void) {
    super();
    this.resetCallback = resetCallback;
  }

  onKeyDown() {
    console.log('[CounterResetAction] Counter reset requested');
    if (this.resetCallback) {
      this.resetCallback();
    }
  }
}
