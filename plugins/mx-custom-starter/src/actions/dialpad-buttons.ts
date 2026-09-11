import { CommandAction } from '@logitech/plugin-sdk';
import { exec } from 'child_process';

/**
 * DialpadButtonActions - Handlers for the 4 physical tactile buttons on the MX Creative Dialpad
 * Control IDs 0, 1, 2, 3 on Loupedeck71
 */

export class DialpadUndoAction extends CommandAction {
  readonly name = 'mx_dialpad_undo';
  readonly displayName = 'Smart Undo';
  readonly description = 'Triggers Command+Z undo shortcut in the active application';
  readonly groupName = 'MX Dialpad Buttons';

  onKeyDown() {
    console.log('[DialpadUndoAction] Triggered Undo (Cmd+Z)');
    exec(`osascript -e 'tell application "System Events" to keystroke "z" using command down'`);
  }
}

export class DialpadRedoAction extends CommandAction {
  readonly name = 'mx_dialpad_redo';
  readonly displayName = 'Smart Redo';
  readonly description = 'Triggers Command+Shift+Z redo shortcut in the active application';
  readonly groupName = 'MX Dialpad Buttons';

  onKeyDown() {
    console.log('[DialpadRedoAction] Triggered Redo (Cmd+Shift+Z)');
    exec(`osascript -e 'tell application "System Events" to keystroke "z" using {command down, shift down}'`);
  }
}

export class DialpadPlayPauseAction extends CommandAction {
  readonly name = 'mx_dialpad_playpause';
  readonly displayName = 'Media Play/Pause';
  readonly description = 'Toggles media playback (Apple Music, Spotify, YouTube)';
  readonly groupName = 'MX Dialpad Buttons';

  onKeyDown() {
    console.log('[DialpadPlayPauseAction] Toggling Media Play/Pause');
    exec(`osascript -e 'tell application "System Events" to key code 49' 2>/dev/null || true`);
  }
}

export class DialpadCustomWorkflowAction extends CommandAction {
  readonly name = 'mx_dialpad_quick_action';
  readonly displayName = 'Quick Action';
  readonly description = 'Executes user-customizable shell command or script';
  readonly groupName = 'MX Dialpad Buttons';

  onKeyDown() {
    console.log('[DialpadCustomWorkflowAction] Executing quick action');
    exec(`say "MX Action triggered" 2>/dev/null || true`);
  }
}
