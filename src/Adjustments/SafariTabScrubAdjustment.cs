namespace Loupedeck.SafariPlugin.Adjustments
{
    using System;
    using Loupedeck.SafariPlugin.Helpers;

    /// <summary>
    /// Rotary adjustment for the MX Dialpad Aluminum Dial (rotatePages: 0).
    /// Rotating clockwise switches to next tab; counter-clockwise switches to previous tab.
    /// Pressing the dial closes the current tab.
    /// </summary>
    public abstract class SafariTabScrubBaseAdjustment : PluginDynamicAdjustment
    {
        private readonly Int32 _thresholdTicks;
        private Int32 _accumulatedTicks = 0;
        private DateTime _lastTickTime = DateTime.MinValue;
        private DateTime _lastExecutionTime = DateTime.MinValue;
        private Int32 _isExecuting = 0;

        private const Int32 CooldownMs = 120;
        private const Int32 IdleTimeoutMs = 350;

        protected SafariTabScrubBaseAdjustment(String displayName, String description, Int32 thresholdTicks)
            : base(displayName: displayName, description: description, groupName: "Safari Adjustments", hasReset: true)
        {
            this._thresholdTicks = Math.Max(1, thresholdTicks);
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

            // Check if threshold reached
            if (Math.Abs(this._accumulatedTicks) < this._thresholdTicks)
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
            SafariAppleScript.CloseActiveTab();
            PluginLog.Info("[SafariTabScrub] Dial pressed: closed active tab.");
        }

        protected override String GetAdjustmentValue(String actionParameter) => "Tabs";
    }

    /// <summary>
    /// Default Tab Scrub adjustment (Ultra Low ~20 ticks).
    /// </summary>
    public class SafariTabScrubAdjustment : SafariTabScrubBaseAdjustment
    {
        public SafariTabScrubAdjustment()
            : base(
                displayName: "Tab Scrub / Switcher",
                description: "Rotate dial to cycle tabs (~20 ticks/tab, anti-spin). Press dial to close tab.",
                thresholdTicks: 20)
        {
        }
    }

    public class SafariTabScrubUltraLowAdjustment : SafariTabScrubBaseAdjustment
    {
        public SafariTabScrubUltraLowAdjustment()
            : base(
                displayName: "Tab Scrub (Ultra Low)",
                description: "Requires ~20 encoder ticks to switch 1 tab. Maximum dampening, eliminates runaway spin.",
                thresholdTicks: 20)
        {
        }
    }

    public class SafariTabScrubVeryLowAdjustment : SafariTabScrubBaseAdjustment
    {
        public SafariTabScrubVeryLowAdjustment()
            : base(
                displayName: "Tab Scrub (Very Low)",
                description: "Requires ~8 encoder ticks to switch 1 tab. Heavy resistance for deliberate tab browsing.",
                thresholdTicks: 8)
        {
        }
    }

    public class SafariTabScrubLowAdjustment : SafariTabScrubBaseAdjustment
    {
        public SafariTabScrubLowAdjustment()
            : base(
                displayName: "Tab Scrub (Low)",
                description: "Requires ~5 encoder ticks to switch 1 tab. Moderate resistance.",
                thresholdTicks: 5)
        {
        }
    }

    public class SafariTabScrubMediumAdjustment : SafariTabScrubBaseAdjustment
    {
        public SafariTabScrubMediumAdjustment()
            : base(
                displayName: "Tab Scrub (Medium)",
                description: "Requires ~3 encoder ticks to switch 1 tab. Light resistance.",
                thresholdTicks: 3)
        {
        }
    }

    public class SafariTabScrubDirectAdjustment : SafariTabScrubBaseAdjustment
    {
        public SafariTabScrubDirectAdjustment()
            : base(
                displayName: "Tab Scrub (Direct)",
                description: "Direct 1:1 hardware pass-through (1 tick per tab).",
                thresholdTicks: 1)
        {
        }
    }
}
