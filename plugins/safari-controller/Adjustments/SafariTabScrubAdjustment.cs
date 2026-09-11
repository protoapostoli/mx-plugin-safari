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
        public SafariTabScrubAdjustment()
            : base(displayName: "Tab Scrub / Switcher", description: "Rotate dial to cycle tabs. Press dial to close tab.", groupName: "Safari Adjustments", hasReset: true)
        {
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            if (diff > 0)
            {
                // Clockwise -> Next Tab
                for (var i = 0; i < Math.Min(diff, 5); i++)
                {
                    SafariAppleScript.NextTab();
                }
            }
            else if (diff < 0)
            {
                // Counter-Clockwise -> Previous Tab
                for (var i = 0; i < Math.Min(-diff, 5); i++)
                {
                    SafariAppleScript.PreviousTab();
                }
            }

            PluginLog.Info($"[SafariTabScrub] Scrubbed tabs by {diff}");
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
