namespace Loupedeck.SafariPlugin.Actions
{
    using System;
    using Loupedeck.SafariPlugin.Helpers;

    public class SafariNewTabCommand : PluginDynamicCommand
    {
        public SafariNewTabCommand()
            : base(displayName: "New Tab", description: "Opens a new Safari tab", groupName: "Safari Controls")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            SafariAppleScript.NewTab();
            PluginLog.Info("[Safari] New Tab opened.");
        }
    }

    public class SafariCloseTabCommand : PluginDynamicCommand
    {
        public SafariCloseTabCommand()
            : base(displayName: "Close Tab", description: "Closes the current Safari tab", groupName: "Safari Controls")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            SafariAppleScript.CloseActiveTab();
            PluginLog.Info("[Safari] Current tab closed.");
        }
    }

    public class SafariDuplicateTabCommand : PluginDynamicCommand
    {
        public SafariDuplicateTabCommand()
            : base(displayName: "Duplicate Tab", description: "Duplicates current active tab with the same URL", groupName: "Safari Controls")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            SafariAppleScript.DuplicateActiveTab();
            PluginLog.Info("[Safari] Current tab duplicated.");
        }
    }

    public class SafariBookmarkletCommand : PluginDynamicCommand
    {
        public SafariBookmarkletCommand()
            : base(displayName: "Clean Reader (De-Clutter)", description: "Strips ads, modal popups, cookie overlays, and fixed banners", groupName: "Safari Controls")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            SafariAppleScript.CleanReaderDeClutter();
            PluginLog.Info("[Safari] Clean Reader / De-Clutter bookmarklet executed.");
        }
    }

    public class SafariReaderModeCommand : PluginDynamicCommand
    {
        public SafariReaderModeCommand()
            : base(displayName: "Toggle Reader View", description: "Toggles Safari native Reader Mode (Cmd+Shift+R)", groupName: "Safari Controls")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            SafariAppleScript.ToggleReaderMode();
            PluginLog.Info("[Safari] Reader Mode toggled.");
        }
    }

    public class SafariBackCommand : PluginDynamicCommand
    {
        public SafariBackCommand()
            : base(displayName: "History Back", description: "Navigates back in Safari history (Cmd+[)", groupName: "Safari Navigation")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            SafariAppleScript.RunAppleScript(@"tell application ""System Events"" to keystroke ""["" using command down");
        }
    }

    public class SafariForwardCommand : PluginDynamicCommand
    {
        public SafariForwardCommand()
            : base(displayName: "History Forward", description: "Navigates forward in Safari history (Cmd+])", groupName: "Safari Navigation")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            SafariAppleScript.RunAppleScript(@"tell application ""System Events"" to keystroke ""]"" using command down");
        }
    }

    public class SafariReloadCommand : PluginDynamicCommand
    {
        public SafariReloadCommand()
            : base(displayName: "Reload Page", description: "Refreshes the current tab (Cmd+R)", groupName: "Safari Navigation")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            SafariAppleScript.RunAppleScript(@"tell application ""System Events"" to keystroke ""r"" using command down");
        }
    }

    public class SafariReopenClosedTabCommand : PluginDynamicCommand
    {
        public SafariReopenClosedTabCommand()
            : base(displayName: "Reopen Closed Tab", description: "Reopens the last closed tab (Cmd+Shift+T)", groupName: "Safari Controls")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            SafariAppleScript.RunAppleScript(@"tell application ""System Events"" to keystroke ""t"" using {command down, shift down}");
        }
    }
}
