#!/usr/bin/env bash
# tools/build-and-restart.sh - Builds Safari plugin and restarts LogiPluginService
set -e

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
export DOTNET_CLI_HOME="${DOTNET_CLI_HOME:-$DIR/.dotnet_home}"

DOTNET_BIN="$HOME/.dotnet/dotnet"
if [ ! -x "$DOTNET_BIN" ]; then
    DOTNET_BIN="$(which dotnet 2>/dev/null || true)"
fi

if [ -z "$DOTNET_BIN" ]; then
    echo "Error: dotnet CLI not found." >&2
    exit 1
fi

echo "==> 1. Building Safari Plugin (.NET)..."
"$DOTNET_BIN" build "$DIR/src/SafariPlugin.csproj" -c Release --nologo

echo "==> 2. Assembling plugin package..."
python3 "$DIR/tools/assemble_safari_plugin.py" > /dev/null

echo "==> 3. Deploying binaries to LogiPluginService..."
PLUGIN_DIR="$HOME/Library/Application Support/Logi/LogiPluginService/Plugins/Safari"
mkdir -p "$PLUGIN_DIR"
cp -R "$DIR/dist/Safari/"* "$PLUGIN_DIR/"

echo "==> 4. Clearing cached action icon overrides..."
rm -rf "$HOME/Library/Application Support/Logi/LogiPluginService/Applications/Loupedeck70/@_safari/Profiles/"*/ActionIcons/* 2>/dev/null || true

echo "==> 5. Gracefully restarting LogiPluginService..."
pkill -f "LogiPluginService" 2>/dev/null || true
sleep 1.5
rm -rf "$HOME/Library/Application Support/Logi/LogiPluginService/Logs/plugin_crashes/"*
open -g "/Applications/Utilities/LogiPluginService.app" 2>/dev/null || open "/Applications/Utilities/LogiPluginService.app"

echo "==> Done! Safari plugin reloaded with new settings."
