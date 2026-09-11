#!/usr/bin/env bash
# ==============================================================================
# Logitech MX Creative Console - Safari Controller Plugin Installer
# ==============================================================================
# Automatically compiles, packages, installs the Safari plugin, and restarts
# the LogiPluginService to activate the controls on your MX Keypad & Dialpad.
# ==============================================================================
set -e

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
export DOTNET_CLI_HOME="${DOTNET_CLI_HOME:-$DIR/.dotnet_home}"

echo "============================================================"
echo "  Safari Controller Plugin Installer for Logitech MX Console"
echo "============================================================"

# 1. Check Operating System
if [[ "$OSTYPE" != "darwin"* ]]; then
    echo "❌ Error: This plugin requires macOS and Safari." >&2
    exit 1
fi

# 2. Check Logi Plugin Service / Logi Options+
LOGI_APP="/Applications/Utilities/LogiPluginService.app"
if [ ! -d "$LOGI_APP" ]; then
    echo "❌ Error: LogiPluginService not found at $LOGI_APP" >&2
    echo "   Please install Logitech Options+ and set up your MX Creative Console first." >&2
    exit 1
fi

# 3. Locate .NET SDK
DOTNET_BIN=""
if command -v dotnet >/dev/null 2>&1; then
    DOTNET_BIN="$(which dotnet)"
elif [ -x "$HOME/.dotnet/dotnet" ]; then
    DOTNET_BIN="$HOME/.dotnet/dotnet"
elif [ -x "/usr/local/share/dotnet/dotnet" ]; then
    DOTNET_BIN="/usr/local/share/dotnet/dotnet"
elif [ -x "/opt/homebrew/bin/dotnet" ]; then
    DOTNET_BIN="/opt/homebrew/bin/dotnet"
fi

if [ -z "$DOTNET_BIN" ]; then
    echo "⚠️  .NET SDK was not found on your system."
    echo "   Attempting to download and install .NET SDK locally into ~/.dotnet..."
    TMP_INSTALL="/tmp/dotnet-install.sh"
    curl -sSL https://dot.net/v1/dotnet-install.sh -o "$TMP_INSTALL"
    chmod +x "$TMP_INSTALL"
    bash "$TMP_INSTALL" --channel 10.0 --install-dir "$HOME/.dotnet" || bash "$TMP_INSTALL" --channel 8.0 --install-dir "$HOME/.dotnet"
    rm -f "$TMP_INSTALL"
    DOTNET_BIN="$HOME/.dotnet/dotnet"
fi

if [ ! -x "$DOTNET_BIN" ]; then
    echo "❌ Error: Could not locate or install .NET SDK." >&2
    echo "   Please install .NET SDK via Homebrew: brew install dotnet-sdk" >&2
    echo "   Or download from: https://dotnet.microsoft.com/download" >&2
    exit 1
fi

echo "✔ Found .NET SDK: $($DOTNET_BIN --version)"

# 4. Compile Safari Plugin
echo ""
echo "==> 1. Compiling Safari Plugin..."
"$DOTNET_BIN" build "$DIR/src/SafariPlugin.csproj" -c Release --nologo

# 5. Assemble Plugin Package
echo ""
echo "==> 2. Assembling plugin package & application profiles..."
python3 "$DIR/tools/assemble_safari_plugin.py"

# 6. Deploy to LogiPluginService
PLUGIN_DIR="$HOME/Library/Application Support/Logi/LogiPluginService/Plugins/Safari"
echo ""
echo "==> 3. Deploying to LogiPluginService ($PLUGIN_DIR)..."
mkdir -p "$PLUGIN_DIR"
cp -R "$DIR/dist/Safari/"* "$PLUGIN_DIR/"

# 7. Clear Cached Overrides
echo "==> 4. Clearing cached action icons..."
rm -rf "$HOME/Library/Application Support/Logi/LogiPluginService/Applications/Loupedeck70/@_safari/Profiles/"*/ActionIcons/* 2>/dev/null || true
rm -rf "$HOME/Library/Application Support/Logi/LogiPluginService/Logs/plugin_crashes/"* 2>/dev/null || true

# 8. Restart Service
echo "==> 5. Restarting LogiPluginService..."
pkill -f "LogiPluginService" 2>/dev/null || true
sleep 1.5
open -g "$LOGI_APP" 2>/dev/null || open "$LOGI_APP"

echo ""
echo "============================================================"
echo "  🎉 Safari Controller Plugin installed successfully!"
echo "============================================================"
echo ""
echo "Next Steps:"
echo "1. Bring Safari to the foreground."
echo "2. Your MX Keypad will automatically switch to the Safari profile:"
echo "   - Center Button: Displays your active Safari tab title with real-time scrolling."
echo "   - Press Center Button: Copies the current tab URL to your clipboard."
echo "   - Dial: Scrub through open Safari tabs."
echo "   - Roller: Smooth vertical webpage scroll."
echo ""
echo "Note: If macOS prompts for permission to control Safari via AppleScript, click 'Allow'."
echo "============================================================"
