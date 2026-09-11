namespace Loupedeck.SafariPlugin.Adjustments
{
    using System;
    using Loupedeck.SafariPlugin.Helpers;

    /// <summary>
    /// Precision roller adjustment for the MX Dialpad Roller Wheel (rotatePages: 1).
    /// Native stepped scrolling up and down via AppleScript System Events.
    /// </summary>
    public class SafariScrollAdjustment : PluginDynamicAdjustment
    {
        private Int32 _accumulatedTicks = 0;
        private DateTime _lastTickTime = DateTime.MinValue;
        private DateTime _lastExecutionTime = DateTime.MinValue;
        private Int32 _isExecuting = 0;

        private const Int32 CooldownMs = 40;
        private const Int32 IdleTimeoutMs = 250;

        public SafariScrollAdjustment()
            : base(displayName: "Precise Page Scroll", description: "Smoothly scrolls the current Safari webpage up or down", groupName: "Safari Adjustments", hasReset: false)
        {
            this.AddParameter("gentle", "Gentle (1 arrow line / tick)", "Sensitivity");
            this.AddParameter("normal", "Normal (3 arrow lines / tick)", "Sensitivity");
            this.AddParameter("fast", "Fast (6 arrow lines / tick)", "Sensitivity");
            this.AddParameter("page", "Page by Page (Page Up / Down)", "Sensitivity");
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            var now = DateTime.UtcNow;

            if ((now - this._lastTickTime).TotalMilliseconds > IdleTimeoutMs)
            {
                this._accumulatedTicks = 0;
            }
            this._lastTickTime = now;

            if ((this._accumulatedTicks > 0 && diff < 0) || (this._accumulatedTicks < 0 && diff > 0))
            {
                this._accumulatedTicks = 0;
            }

            this._accumulatedTicks += diff;

            if (Math.Abs(this._accumulatedTicks) < 1)
            {
                return;
            }

            if ((now - this._lastExecutionTime).TotalMilliseconds < CooldownMs)
            {
                return;
            }

            if (System.Threading.Interlocked.CompareExchange(ref this._isExecuting, 1, 0) != 0)
            {
                return;
            }

            var dir = this._accumulatedTicks > 0 ? 1 : -1;
            this._accumulatedTicks = 0;
            this._lastExecutionTime = now;

            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    SafariAppleScript.Scroll(dir, actionParameter);
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"[SafariScroll] Error scrolling: {ex.Message}");
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref this._isExecuting, 0);
                }
            });
        }

        protected override String GetAdjustmentValue(String actionParameter) => "Scroll";
    }
}
