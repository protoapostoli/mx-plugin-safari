namespace Loupedeck.CustomPlugin
{
    using System;

    /// <summary>
    /// Custom dynamic adjustment mapped to MX Dialpad Dial (controlId: 0) or Roller (controlId: 1).
    /// </summary>
    public class CustomAdjustment : PluginDynamicAdjustment
    {
        private Int32 _value = 50;

        public CustomAdjustment()
            : base(displayName: "Custom Step Adjustment", description: "Stepped adjustment for Dial or Roller", groupName: "Custom Adjustments", hasReset: true)
        {
        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            this._value = Math.Clamp(this._value + diff, 0, 100);
            this.AdjustmentValueChanged();
            PluginLog.Info($"Adjustment rotated by {diff}. New value: {this._value}");
        }

        protected override void RunCommand(String actionParameter)
        {
            // Reset command (dial press)
            this._value = 50;
            this.AdjustmentValueChanged();
            PluginLog.Info("Adjustment reset to 50");
        }

        protected override String GetAdjustmentValue(String actionParameter) => $"{this._value}%";
    }
}
