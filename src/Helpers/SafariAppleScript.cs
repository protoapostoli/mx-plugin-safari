namespace Loupedeck.SafariPlugin.Helpers
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    public static class SafariAppleScript
    {
        public static String RunAppleScript(String script)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "/usr/bin/osascript",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                psi.ArgumentList.Add("-e");
                psi.ArgumentList.Add(script);

                using var process = Process.Start(psi);
                if (process == null)
                {
                    return String.Empty;
                }

                var output = process.StandardOutput.ReadToEnd().Trim();
                var error = process.StandardError.ReadToEnd().Trim();
                process.WaitForExit(1500);

                if (!String.IsNullOrEmpty(error))
                {
                    PluginLog.Warning($"AppleScript stderr: {error}");
                }

                return output;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"AppleScript execution failed: {ex.Message}");
                return String.Empty;
            }
        }

        public static (String Url, String Title, String Domain) GetActiveTabInfo()
        {
            var script = @"if application ""Safari"" is running then
    tell application ""Safari""
        if (count of windows) > 0 then
            tell current tab of front window
                return (get URL) & ""|||"" & (get name)
            end tell
        else
            return ""NO_WINDOWS""
        end if
    end tell
else
    return ""NOT_RUNNING""
end if";

            var result = RunAppleScript(script);
            if (String.IsNullOrWhiteSpace(result) || !result.Contains("|||"))
            {
                return (String.Empty, "Safari", "Safari");
            }

            var parts = result.Split(new[] { "|||" }, StringSplitOptions.None);
            var url = parts.Length > 0 ? parts[0].Trim() : String.Empty;
            var title = parts.Length > 1 ? parts[1].Trim() : String.Empty;

            var domain = "Safari";
            if (!String.IsNullOrEmpty(url))
            {
                try
                {
                    if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                    {
                        domain = uri.Host;
                        if (domain.StartsWith("www."))
                        {
                            domain = domain.Substring(4);
                        }
                    }
                    else
                    {
                        var match = Regex.Match(url, @"https?://(?:www\.)?([^/]+)");
                        if (match.Success)
                        {
                            domain = match.Groups[1].Value;
                        }
                    }
                }
                catch
                {
                    domain = "Safari";
                }
            }

            return (url, title, domain);
        }

        public static void NewTab()
        {
            RunAppleScript(@"
tell application ""Safari""
    activate
    tell front window to make new tab
end tell");
        }

        public static void CloseActiveTab()
        {
            RunAppleScript(@"
tell application ""Safari""
    if (count of windows) > 0 then
        tell current tab of front window to close
    end if
end tell");
        }

        public static void DuplicateActiveTab()
        {
            RunAppleScript(@"
tell application ""Safari""
    if (count of windows) > 0 then
        set curUrl to URL of current tab of front window
        tell front window to make new tab with properties {URL:curUrl}
    end if
end tell");
        }

        public static void NextTab()
        {
            RunAppleScript(@"
tell application ""Safari""
    if (count of windows) > 0 then
        tell front window
            set curIdx to index of current tab
            set totalTabs to count of tabs
            set nextIdx to curIdx + 1
            if nextIdx > totalTabs then set nextIdx to 1
            set current tab to tab nextIdx
        end tell
    end if
end tell");
        }

        public static void PreviousTab()
        {
            RunAppleScript(@"
tell application ""Safari""
    if (count of windows) > 0 then
        tell front window
            set curIdx to index of current tab
            set totalTabs to count of tabs
            set prevIdx to curIdx - 1
            if prevIdx < 1 then set prevIdx to totalTabs
            set current tab to tab prevIdx
        end tell
    end if
end tell");
        }

        public static void ZoomIn()
        {
            RunAppleScript(@"tell application ""System Events"" to keystroke ""+"" using command down");
        }

        public static void ZoomOut()
        {
            RunAppleScript(@"tell application ""System Events"" to keystroke ""-"" using command down");
        }

        public static void ResetZoom()
        {
            RunAppleScript(@"tell application ""System Events"" to keystroke ""0"" using command down");
        }

        public static void Scroll(Int32 diff)
        {
            var keyCode = diff > 0 ? 125 : 126; // 125: Down Arrow, 126: Up Arrow
            var repeatCount = Math.Min(Math.Max(Math.Abs(diff), 1) * 3, 15);
            RunAppleScript($@"tell application ""System Events""
    repeat {repeatCount} times
        key code {keyCode}
    end repeat
end tell");
        }

        public static void ToggleReaderMode()
        {
            RunAppleScript(@"tell application ""System Events"" to keystroke ""r"" using {command down, shift down}");
        }

        public static void OpenBookmarksSidebar()
        {
            RunAppleScript(@"tell application ""Safari"" to activate
tell application ""System Events"" to keystroke ""1"" using {command down, control down}");
        }

        public static void OpenReadingListSidebar()
        {
            RunAppleScript(@"tell application ""Safari"" to activate
tell application ""System Events"" to keystroke ""2"" using {command down, control down}");
        }
    }
}
