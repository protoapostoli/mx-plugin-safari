import { AdjustmentAction, AdjustmentActionExecuteEvent } from '@logitech/plugin-sdk';
import { exec } from 'child_process';

/**
 * PrecisionRollerAdjustment - Controls the roller wheel on the MX Creative Dialpad (Control ID 1)
 * Ideal for stepped volume control, brush size adjustments, or vertical scrolling.
 */
export class PrecisionRollerAdjustment extends AdjustmentAction {
  readonly name = 'mx_roller_volume';
  readonly displayName = 'Roller System Volume';
  readonly description = 'Adjusts macOS system output volume using the Dialpad roller wheel';
  readonly groupName = 'MX Dialpad Adjustments';
  readonly hasReset = false;

  execute(event: AdjustmentActionExecuteEvent) {
    const diff = event.tick;
    console.log(`[PrecisionRollerAdjustment] Roller moved by ${diff} ticks`);

    // diff > 0 -> Volume Up, diff < 0 -> Volume Down
    const change = diff > 0 ? '+ 2' : '- 2';
    const script = `set volume output volume ((output volume of (get volume settings)) ${change})`;
    exec(`osascript -e '${script}'`, (err) => {
      if (err) {
        console.error('[PrecisionRollerAdjustment] Failed to adjust volume:', err);
      }
    });
  }
}

export class RollerScrollAdjustment extends AdjustmentAction {
  readonly name = 'mx_roller_scroll';
  readonly displayName = 'Roller Precision Scroll';
  readonly description = 'Sends vertical scroll steps using the Dialpad roller wheel';
  readonly groupName = 'MX Dialpad Adjustments';
  readonly hasReset = false;

  execute(event: AdjustmentActionExecuteEvent) {
    const ticks = event.tick;
    // Positive tick scrolls down, negative tick scrolls up
    const scrollLines = ticks * 3;
    exec(`osascript -e 'tell application "System Events" to scroll down by ${scrollLines}' 2>/dev/null || true`);
  }
}
