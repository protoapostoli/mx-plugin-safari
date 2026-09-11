namespace Loupedeck.SafariPlugin.Adjustments
{
    using System;
    using Loupedeck.SafariPlugin.Helpers;

    /// <summary>
    /// Alternate rotary adjustment for the MX Dialpad Aluminum Dial.
    /// Zooms page in / out. Pressing the dial resets zoom to 100%.
    /// </summary>
    public class SafariZoomAdjustment : PluginDynamicAdjustment
    {
        public SafariZoomAdjustment()
            : base(displayName: "Safari Page Zoom", description: "Rotate dial to zoom in/out. Press dial to reset zoom to 100%.", groupName: "Safari Adjustments", hasReset: true)
        {
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            if (diff > 0)
            {
                SafariAppleScript.ZoomIn();
            }
            else if (diff < 0)
            {
                SafariAppleScript.ZoomOut();
            }
        }

        protected override void RunCommand(String actionParameter)
        {
            SafariAppleScript.ResetZoom();
            PluginLog.Info("[SafariZoom] Zoom reset to 100%.");
        }

        protected override String GetAdjustmentValue(String actionParameter) => "Zoom";
    }
}
