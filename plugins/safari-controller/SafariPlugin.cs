namespace Loupedeck.SafariPlugin
{
    using System;
    using System.Timers;
    using Loupedeck.SafariPlugin.Actions;
    using Loupedeck.SafariPlugin.Helpers;

    public class SafariPlugin : Plugin
    {
        public override Boolean UsesApplicationApiOnly => false;
        public override Boolean HasNoApplication => false;

        private Timer _tabPollerTimer;
        private static SafariActiveTabKey _activeTabKeyInstance;

        public static void RegisterActiveTabKey(SafariActiveTabKey key)
        {
            _activeTabKeyInstance = key;
        }

        public SafariPlugin()
        {
            PluginLog.Init(this.Log);
        }

        public override void Load()
        {
            PluginLog.Init(this.Log);
            try { System.IO.File.AppendAllText("/tmp/safari_keypad.log", $"[{DateTime.Now:HH:mm:ss.fff}] SafariPlugin.Load() called\n"); } catch {}
            PluginLog.Info("[SafariPlugin] Loaded successfully for Logitech MX Creative Console.");

            // Start active tab background poller (polls every 500ms)
            this._tabPollerTimer = new Timer(500) { AutoReset = true };
            this._tabPollerTimer.Elapsed += this.OnTabPollTick;
            this._tabPollerTimer.Start();
        }

        public override void Unload()
        {
            this._tabPollerTimer?.Stop();
            this._tabPollerTimer?.Dispose();
            this._tabPollerTimer = null;
            _activeTabKeyInstance?.StopTimers();
            PluginLog.Info("[SafariPlugin] Unloaded.");
        }

        private void OnTabPollTick(Object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_activeTabKeyInstance != null)
                {
                    var (url, title, domain) = SafariAppleScript.GetActiveTabInfo();
                    _activeTabKeyInstance.UpdateTab(url, title, domain);
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error($"[SafariPlugin] Error during tab poll: {ex.Message}");
            }
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
