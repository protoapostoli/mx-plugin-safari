#!/usr/bin/env bash
# HomemadeMXDeckControls - Environment activation script
# Compatible with both Zsh and Bash

if [ -n "$ZSH_VERSION" ]; then
    _SCRIPT_PATH="${(%):-%x}"
elif [ -n "$BASH_VERSION" ]; then
    _SCRIPT_PATH="${BASH_SOURCE[0]}"
else
    _SCRIPT_PATH="$0"
fi

SCRIPT_DIR="$(cd "$(dirname "$_SCRIPT_PATH")/.." && pwd)"
export PATH="$HOME/.dotnet:$SCRIPT_DIR/bin:$PATH"
export NUGET_PACKAGES="$SCRIPT_DIR/.nuget"
export DOTNET_CLI_HOME="$SCRIPT_DIR/.dotnet_home"

echo "============================================================"
echo "  Logitech MX Creative Console Dev Environment Activated"
echo "============================================================"
echo "  Project Root:  $SCRIPT_DIR"
echo "  Added to PATH: $SCRIPT_DIR/bin"
echo "  Node:          $(node -v 2>/dev/null || echo 'Not found')"
echo "  .NET SDK:      $(dotnet --version 2>/dev/null || echo 'Not found')"
echo "  mxdev CLI:     $(which mxdev)"
echo ""
echo "Try running:"
echo "  mxdev status       - Check device & plugin status"
echo "  mxdev logs         - Watch plugin logs in real time"
echo "  mxdev help         - View all commands"
echo "============================================================"
