#!/usr/bin/env bash
# tools/install-dotnet.sh - Installs .NET 8 SDK locally in user space (~/.dotnet)
set -e

DOTNET_DIR="$HOME/.dotnet"
INSTALL_SCRIPT="/tmp/dotnet-install.sh"
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

echo "============================================================"
echo "  Installing .NET 8 SDK (User-space: $DOTNET_DIR)"
echo "============================================================"

if [ -f "$DOTNET_DIR/dotnet" ]; then
    echo ".NET SDK already found at $DOTNET_DIR/dotnet"
    "$DOTNET_DIR/dotnet" --version
else
    echo "Downloading Microsoft .NET installer script..."
    curl -sSL https://raw.githubusercontent.com/dotnet/install-scripts/main/src/dotnet-install.sh -o "$INSTALL_SCRIPT"
    chmod +x "$INSTALL_SCRIPT"

    echo "Running dotnet installer for channel 8.0..."
    bash "$INSTALL_SCRIPT" --channel 8.0 --install-dir "$DOTNET_DIR"
    rm -f "$INSTALL_SCRIPT"
fi

# Create bin/dotnet wrapper
mkdir -p "$PROJECT_DIR/bin"
cat << 'EOF' > "$PROJECT_DIR/bin/dotnet"
#!/usr/bin/env bash
set -e
DOTNET_BIN="$HOME/.dotnet/dotnet"
if [ -f "$DOTNET_BIN" ]; then
    exec "$DOTNET_BIN" "$@"
fi
if command -v /usr/local/share/dotnet/dotnet >/dev/null 2>&1; then
    exec /usr/local/share/dotnet/dotnet "$@"
fi
echo "Error: .NET 8 SDK not found. Run ./tools/install-dotnet.sh to install." >&2
exit 1
EOF
chmod +x "$PROJECT_DIR/bin/dotnet"

echo "Created wrapper: $PROJECT_DIR/bin/dotnet"
"$PROJECT_DIR/bin/dotnet" --version
echo "============================================================"
echo "  .NET 8 SDK installed and verified successfully!"
echo "============================================================"
