import { AdjustmentAction, AdjustmentActionExecuteEvent } from '@logitech/plugin-sdk';
import { exec } from 'child_process';

/**
 * PrecisionDialAdjustment - Controls the large smooth aluminum dial on the MX Creative Dialpad (Control ID 0)
 * Handles continuous clockwise (tick > 0) and counter-clockwise (tick < 0) rotation.
 */
export class PrecisionDialAdjustment extends AdjustmentAction {
  readonly name = 'mx_dial_counter';
  readonly displayName = 'Precision Dial Counter';
  readonly description = 'Tracks rotation ticks from the MX Dialpad dial with reset support';
  readonly groupName = 'MX Dialpad Adjustments';
  readonly hasReset = true;

  private counter: number = 0;

  constructor() {
    super();
  }

  getCounter(): number {
    return this.counter;
  }

  resetCounter(): void {
    this.counter = 0;
    console.log('[PrecisionDialAdjustment] Counter reset to 0');
  }

  execute(event: AdjustmentActionExecuteEvent) {
    const diff = event.tick;
    this.counter += diff;
    console.log(`[PrecisionDialAdjustment] Dial rotated by ${diff} ticks. Current total: ${this.counter}`);
  }
}

/**
 * TimelineScrubAdjustment - Precision timeline scrubbing or zoom
 */
export class TimelineScrubAdjustment extends AdjustmentAction {
  readonly name = 'mx_dial_scrub';
  readonly displayName = 'Timeline Scrub / Arrow Step';
  readonly description = 'Sends left/right arrow keystrokes based on dial rotation direction';
  readonly groupName = 'MX Dialpad Adjustments';
  readonly hasReset = false;

  execute(event: AdjustmentActionExecuteEvent) {
    const ticks = event.tick;
    if (ticks > 0) {
      // Rotate clockwise -> Right Arrow (code 124)
      exec(`osascript -e 'tell application "System Events" to key code 124' 2>/dev/null || true`);
    } else if (ticks < 0) {
      // Rotate counter-clockwise -> Left Arrow (code 123)
      exec(`osascript -e 'tell application "System Events" to key code 123' 2>/dev/null || true`);
    }
  }
}
