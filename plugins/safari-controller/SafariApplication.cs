namespace Loupedeck.SafariPlugin
{
    using System;

    /// <summary>
    /// Represents the Apple Safari client application integration for Logitech MX Creative Console.
    /// Required by LogiPluginService loader to bind application lifecycle and bundle metadata.
    /// </summary>
    public class SafariApplication : ClientApplication
    {
        public SafariApplication()
        {
        }

        protected override String GetProcessName() => "Safari";

        protected override String GetBundleName() => "com.apple.Safari";

        public override ClientApplicationStatus GetApplicationStatus() => ClientApplicationStatus.Installed;
    }
}
