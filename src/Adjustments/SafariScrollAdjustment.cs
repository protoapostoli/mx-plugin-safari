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
        public SafariScrollAdjustment()
            : base(displayName: "Precise Page Scroll", description: "Smoothly scrolls the current Safari webpage up or down", groupName: "Safari Adjustments", hasReset: false)
        {
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            SafariAppleScript.Scroll(diff);
        }

        protected override String GetAdjustmentValue(String actionParameter) => "Scroll";
    }
}
