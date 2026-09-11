namespace Loupedeck.SafariPlugin.Adjustments
{
    using System;
    using Loupedeck.SafariPlugin.Helpers;

    /// <summary>
    /// Rotary adjustment for the MX Dialpad Aluminum Dial (rotatePages: 0).
    /// Rotating clockwise switches to next tab; counter-clockwise switches to previous tab.
    /// Pressing the dial closes the current tab.
    /// </summary>
    public class SafariTabScrubAdjustment : PluginDynamicAdjustment
    {
        private Int32 _accumulatedTicks = 0;
        private DateTime _lastTickTime = DateTime.MinValue;
        private DateTime _lastExecutionTime = DateTime.MinValue;
        private Int32 _isExecuting = 0;

        private const Int32 CooldownMs = 120;
        private const Int32 IdleTimeoutMs = 350;

        public SafariTabScrubAdjustment()
            : base(displayName: "Tab Scrub / Switcher", description: "Rotate dial to cycle tabs. Press dial to close tab.", groupName: "Safari Adjustments", hasReset: true)
        {
            this.AddParameter("ultra_low", "Ultra Low (~8 ticks / tab)", "Sensitivity");
            this.AddParameter("low", "Low (~5 ticks / tab)", "Sensitivity");
            this.AddParameter("medium", "Medium (~3 ticks / tab)", "Sensitivity");
            this.AddParameter("direct", "Direct (1 tick / tab)", "Sensitivity");
        }

        private Int32 GetThreshold(String actionParameter)
        {
            return actionParameter switch
            {
                "direct" => 1,
                "medium" => 3,
                "low" => 5,
                _ => 8 // Default to "ultra_low" (~8 ticks per tab)
            };
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            var now = DateTime.UtcNow;

            // Reset accumulator if user was idle
            if ((now - this._lastTickTime).TotalMilliseconds > IdleTimeoutMs)
            {
                this._accumulatedTicks = 0;
            }
            this._lastTickTime = now;

            // Reset accumulator if reversing direction
            if ((this._accumulatedTicks > 0 && diff < 0) || (this._accumulatedTicks < 0 && diff > 0))
            {
                this._accumulatedTicks = 0;
            }

            this._accumulatedTicks += diff;
            var threshold = this.GetThreshold(actionParameter);

            // Check if threshold reached
            if (Math.Abs(this._accumulatedTicks) < threshold)
            {
                return;
            }

            // Check cooldown to prevent queue backlog
            if ((now - this._lastExecutionTime).TotalMilliseconds < CooldownMs)
            {
                return;
            }

            // Concurrency guard: skip if an AppleScript process is already in flight
            if (Interlocked.CompareExchange(ref this._isExecuting, 1, 0) != 0)
            {
                return;
            }

            var isNext = this._accumulatedTicks > 0;
            this._accumulatedTicks = 0;
            this._lastExecutionTime = now;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    if (isNext)
                    {
                        SafariAppleScript.NextTab();
                    }
                    else
                    {
                        SafariAppleScript.PreviousTab();
                    }
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"[SafariTabScrub] Error switching tab: {ex.Message}");
                }
                finally
                {
                    Interlocked.Exchange(ref this._isExecuting, 0);
                }
            });
        }

        protected override void RunCommand(String actionParameter)
        {
            // Dial press action: Close current tab
            SafariAppleScript.CloseActiveTab();
            PluginLog.Info("[SafariTabScrub] Dial pressed: closed active tab.");
        }

        protected override String GetAdjustmentValue(String actionParameter) => "Tabs";
    }
}
