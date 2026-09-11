namespace Loupedeck.SafariPlugin.Adjustments
{
    using System;
    using Loupedeck.SafariPlugin.Helpers;

    /// <summary>
    /// Precision roller adjustment for the MX Dialpad Roller Wheel (rotatePages: 1).
    /// Stepped smooth scrolling up and down via injected JavaScript.
    /// </summary>
    public class SafariScrollAdjustment : PluginDynamicAdjustment
    {
        private const Int32 ScrollStepPixels = 75;

        public SafariScrollAdjustment()
            : base(displayName: "Precise Page Scroll", description: "Smoothly scrolls the current Safari webpage up or down", groupName: "Safari Adjustments", hasReset: false)
        {
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            var pixels = diff * ScrollStepPixels;
            SafariAppleScript.ScrollBy(pixels);
        }

        protected override String GetAdjustmentValue(String actionParameter) => "Scroll";
    }
}
