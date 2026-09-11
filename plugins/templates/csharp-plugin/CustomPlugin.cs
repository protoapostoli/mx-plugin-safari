namespace Loupedeck.CustomPlugin
{
    using System;

    public class CustomPlugin : Plugin
    {
        public override Boolean UsesApplicationApiOnly => true;
        public override Boolean HasNoApplication => true;

        public CustomPlugin()
        {
            // Initializing plugin
        }

        public override void Load()
        {
            PluginLog.Info("Custom C# Plugin Loaded successfully for MX Creative Console!");
        }

        public override void Unload()
        {
            PluginLog.Info("Custom C# Plugin Unloaded.");
        }
    }

    public static class PluginLog
    {
        private static PluginLogFile _log;
        public static void Init(PluginLogFile log) => _log = log;
        public static void Info(String text) => _log?.Info(text);
        public static void Warning(String text) => _log?.Warning(text);
        public static void Error(String text) => _log?.Error(text);
    }
}
