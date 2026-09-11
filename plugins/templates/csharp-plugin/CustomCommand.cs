namespace Loupedeck.CustomPlugin
{
    using System;

    /// <summary>
    /// Custom dynamic command mapped to MX Keypad LCD keys or Dialpad buttons.
    /// Demonstrates dynamic bitmap image generation on LCD keys via BitmapBuilder.
    /// </summary>
    public class CustomCommand : PluginDynamicCommand
    {
        private Int32 _pressCount = 0;

        public CustomCommand()
            : base(displayName: "Custom Counter Button", description: "Displays dynamic press count on Keypad LCD", groupName: "Custom Actions")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            this._pressCount++;
            PluginLog.Info($"CustomCommand pressed {this._pressCount} times.");
            // Request redraw of this button's LCD display
            this.ActionImageChanged();
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            using (var builder = new BitmapBuilder(imageSize))
            {
                // Background color (dark slate)
                builder.FillRectangle(0, 0, builder.Width, builder.Height, new BitmapColor(30, 30, 35));
                // Draw text showing current press count on the LCD key
                builder.DrawText($"Hits: {this._pressCount}", BitmapColor.White, 16);
                return builder.ToImage();
            }
        }
    }
}
