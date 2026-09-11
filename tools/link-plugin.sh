#!/usr/bin/env bash
# tools/link-plugin.sh - Symlink a plugin to LogiPluginService Plugins directory
DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
exec "$DIR/bin/mxdev" link "$@"
