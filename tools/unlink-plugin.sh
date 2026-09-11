#!/usr/bin/env bash
# tools/unlink-plugin.sh - Remove symlink of a plugin from LogiPluginService
DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
exec "$DIR/bin/mxdev" unlink "$@"
