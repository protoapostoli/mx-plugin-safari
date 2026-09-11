namespace Loupedeck.SafariPlugin.Actions
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Timers;
    using Loupedeck.SafariPlugin.Helpers;

    /// <summary>
    /// Dynamic LCD Key that displays the TITLE of the active Safari tab on the MX Keypad (80x80 LCD).
    /// Displays large, bold text. If the title does not fit, it scrolls smoothly across the button.
    /// Tap to copy the current tab URL to the clipboard with visual feedback.
    /// </summary>
    public class SafariActiveTabKey : PluginDynamicCommand
    {
        // Display configuration: giant font filling button height, vertically centered
        // - FontSize: Giant text point size (26 = giant, legible across room)
        // - VisibleCharCount: 5 characters fit horizontally across 80x80 LCD without clipping
        // - ScrollIntervalMs: Speed of the marquee scroll (ms per character step)
        // - PauseTicks: Number of steps to pause at the start of title and after each loop
        public const Int32 FontSize = 26;
        public const Int32 VisibleCharCount = 5;
        public const Int32 ScrollIntervalMs = 230;
        public const Int32 PauseTicks = 6;

        private String _currentUrl = null;
        private String _currentTitle = null;
        private String _currentDomain = null;
        private Boolean _justCopied = false;

        private readonly Timer _pollTimer;
        private readonly Timer _marqueeTimer;
        private readonly Timer _copiedResetTimer;

        private String _marqueeBuffer = String.Empty;
        private Int32 _scrollIndex = 0;
        private Int32 _pauseCounter = 0;
        private readonly Object _lock = new Object();
        private Boolean _isPolling = false;

        private static void LogDebug(String msg)
        {
            try
            {
                var line = $"[{DateTime.Now:HH:mm:ss.fff}] {msg}\n";
                System.IO.File.AppendAllText("/tmp/safari_keypad.log", line);
            }
            catch {}
        }

        public SafariActiveTabKey()
            : base(displayName: String.Empty, description: "Displays active Safari tab title with giant marquee scrolling. Press to copy URL.", groupName: "Safari Controls")
        {
            this.IsWidget = true;
            LogDebug("SafariActiveTabKey constructor called");
            SafariPlugin.RegisterActiveTabKey(this);

            // 1. Timer for copy confirmation reset
            this._copiedResetTimer = new Timer(1200) { AutoReset = false };
            this._copiedResetTimer.Elapsed += (s, e) =>
            {
                this._justCopied = false;
                this.ActionImageChanged();
            };

            // 2. Marquee animation timer
            this._marqueeTimer = new Timer(ScrollIntervalMs) { AutoReset = true };
            this._marqueeTimer.Elapsed += this.OnMarqueeTick;

            // 3. Tab poll timer: polls Safari active tab every 500ms
            this._pollTimer = new Timer(500) { AutoReset = true };
            this._pollTimer.Elapsed += this.OnPollTick;
            this._pollTimer.Start();

            // Initial fetch on background task
            Task.Run(() => this.CheckActiveTab());
        }

        public void StopTimers()
        {
            try
            {
                this._pollTimer?.Stop();
                this._pollTimer?.Dispose();
                this._marqueeTimer?.Stop();
                this._marqueeTimer?.Dispose();
                this._copiedResetTimer?.Stop();
                this._copiedResetTimer?.Dispose();
            }
            catch {}
        }

        private void OnPollTick(Object sender, ElapsedEventArgs e)
        {
            this.CheckActiveTab();
        }

        private void CheckActiveTab()
        {
            if (this._isPolling)
            {
                return;
            }

            this._isPolling = true;
            try
            {
                var (url, title, domain) = SafariAppleScript.GetActiveTabInfo();
                this.UpdateTab(url, title, domain);
            }
            catch (Exception ex)
            {
                LogDebug($"Poll error: {ex.Message}");
                PluginLog.Error($"[SafariActiveTabKey] Poll error: {ex.Message}");
            }
            finally
            {
                this._isPolling = false;
            }
        }

        public void UpdateTab(String url, String title, String domain)
        {
            lock (this._lock)
            {
                var cleanTitle = String.IsNullOrWhiteSpace(title) ? "Safari" : title.Trim();
                var cleanUrl = url ?? String.Empty;

                if (this._currentUrl == cleanUrl && this._currentTitle == cleanTitle)
                {
                    return;
                }

                LogDebug($"Tab updated: Title='{cleanTitle}', Url='{cleanUrl}'");
                this._currentUrl = cleanUrl;
                this._currentTitle = cleanTitle;
                this._currentDomain = domain ?? "Safari";

                // Setup marquee
                if (this._currentTitle.Length > VisibleCharCount)
                {
                    // Looping buffer: Title followed by distinct spacer
                    this._marqueeBuffer = this._currentTitle + "    •••    ";
                    this._scrollIndex = 0;
                    this._pauseCounter = PauseTicks; // Pause so start is readable
                    if (!this._marqueeTimer.Enabled)
                    {
                        this._marqueeTimer.Start();
                        LogDebug("Marquee timer started");
                    }
                }
                else
                {
                    this._marqueeBuffer = this._currentTitle;
                    this._scrollIndex = 0;
                    this._pauseCounter = 0;
                    this._marqueeTimer.Stop();
                    LogDebug("Marquee timer stopped (title fits)");
                }

                this.ActionImageChanged();
            }
        }

        private void OnMarqueeTick(Object sender, ElapsedEventArgs e)
        {
            lock (this._lock)
            {
                if (this._currentTitle.Length <= VisibleCharCount || String.IsNullOrEmpty(this._marqueeBuffer) || this._justCopied)
                {
                    return;
                }

                // Pause at the beginning of the title or after looping
                if (this._pauseCounter > 0)
                {
                    this._pauseCounter--;
                    return;
                }

                this._scrollIndex++;
                if (this._scrollIndex >= this._marqueeBuffer.Length)
                {
                    this._scrollIndex = 0;
                    this._pauseCounter = PauseTicks;
                }

                this.ActionImageChanged();
            }
        }

        private String GetDisplayTitleSlice()
        {
            lock (this._lock)
            {
                if (String.IsNullOrWhiteSpace(this._currentTitle))
                {
                    return "Safari";
                }

                if (this._currentTitle.Length <= VisibleCharCount)
                {
                    return this._currentTitle;
                }

                var bufLen = this._marqueeBuffer.Length;
                var sliceLen = Math.Min(VisibleCharCount, bufLen);
                var chars = new Char[sliceLen];

                for (var i = 0; i < sliceLen; i++)
                {
                    chars[i] = this._marqueeBuffer[(this._scrollIndex + i) % bufLen];
                }

                return new String(chars);
            }
        }

        protected override void RunCommand(String actionParameter)
        {
            if (!String.IsNullOrEmpty(this._currentUrl))
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "/usr/bin/pbcopy",
                        UseShellExecute = false,
                        RedirectStandardInput = true
                    };
                    using var proc = Process.Start(psi);
                    if (proc != null)
                    {
                        proc.StandardInput.Write(this._currentUrl);
                        proc.StandardInput.Close();
                        proc.WaitForExit(500);
                    }

                    this._justCopied = true;
                    this._copiedResetTimer.Stop();
                    this._copiedResetTimer.Start();
                    this.ActionImageChanged();
                    PluginLog.Info($"[SafariActiveTabKey] Copied URL to clipboard: {this._currentUrl}");
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"[SafariActiveTabKey] Failed to copy URL: {ex.Message}");
                }
            }
            else
            {
                this.CheckActiveTab();
            }
        }

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => String.Empty;

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            using (var builder = new BitmapBuilder(imageSize))
            {
                var w = builder.Width;
                var h = builder.Height;

                // 1. Sleek dark background filling the full 80x80 LCD (no borders, no boxes)
                var bgColor = new BitmapColor(18, 20, 24);
                builder.FillRectangle(0, 0, w, h, bgColor);

                if (this._justCopied)
                {
                    // Copy feedback: deep green background, crisp centered "COPIED!"
                    var bgGreen = new BitmapColor(18, 55, 28);
                    builder.FillRectangle(0, 0, w, h, bgGreen);
                    builder.DrawText("COPIED!", BitmapColor.White, 18);
                    return builder.ToImage();
                }

                // 2. Render giant title text, vertically and horizontally centered across the entire button
                var displayTitle = this.GetDisplayTitleSlice();
                builder.DrawText(displayTitle, BitmapColor.White, FontSize);

                return builder.ToImage();
            }
        }
    }
}
