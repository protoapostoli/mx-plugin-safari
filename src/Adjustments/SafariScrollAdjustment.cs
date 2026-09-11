namespace Loupedeck.SafariPlugin.Adjustments
{
    using System;
    using Loupedeck.SafariPlugin.Helpers;

    /// <summary>
    /// Precision roller adjustment for the MX Dialpad Roller Wheel (rotatePages: 1).
    /// Native stepped scrolling up and down via AppleScript System Events.
    /// </summary>
    public abstract class SafariScrollBaseAdjustment : PluginDynamicAdjustment
    {
        private readonly String _mode;
        private Int32 _accumulatedTicks = 0;
        private DateTime _lastTickTime = DateTime.MinValue;
        private DateTime _lastExecutionTime = DateTime.MinValue;
        private Int32 _isExecuting = 0;

        private const Int32 CooldownMs = 40;
        private const Int32 IdleTimeoutMs = 250;

        protected SafariScrollBaseAdjustment(String displayName, String description, String mode)
            : base(displayName: displayName, description: description, groupName: "Safari Adjustments", hasReset: false)
        {
            this._mode = mode ?? "gentle";
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
                    SafariAppleScript.Scroll(dir, this._mode);
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

    /// <summary>
    /// Default Precise Page Scroll (Gentle).
    /// </summary>
    public class SafariScrollAdjustment : SafariScrollBaseAdjustment
    {
        public SafariScrollAdjustment()
            : base(
                displayName: "Precise Page Scroll",
                description: "Smoothly scrolls the current Safari webpage up or down (Gentle speed).",
                mode: "gentle")
        {
        }
    }

    public class SafariScrollGentleAdjustment : SafariScrollBaseAdjustment
    {
        public SafariScrollGentleAdjustment()
            : base(
                displayName: "Page Scroll (Gentle)",
                description: "Scrolls 1 arrow line per tick. Ideal for slow, detailed reading.",
                mode: "gentle")
        {
        }
    }

    public class SafariScrollNormalAdjustment : SafariScrollBaseAdjustment
    {
        public SafariScrollNormalAdjustment()
            : base(
                displayName: "Page Scroll (Normal)",
                description: "Scrolls 3 arrow lines per tick. Balanced navigation speed.",
                mode: "normal")
        {
        }
    }

    public class SafariScrollFastAdjustment : SafariScrollBaseAdjustment
    {
        public SafariScrollFastAdjustment()
            : base(
                displayName: "Page Scroll (Fast)",
                description: "Scrolls 6 arrow lines per tick. Rapid skimming through long pages.",
                mode: "fast")
        {
        }
    }

    public class SafariScrollPageAdjustment : SafariScrollBaseAdjustment
    {
        public SafariScrollPageAdjustment()
            : base(
                displayName: "Page Scroll (Page by Page)",
                description: "Triggers Page Up / Page Down per tick for rapid jumping.",
                mode: "page")
        {
        }
    }
}
