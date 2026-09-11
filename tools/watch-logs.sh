#!/usr/bin/env bash
# tools/watch-logs.sh - Watch LogiPluginService logs in real-time
DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
exec "$DIR/bin/mxdev" logs "$@"
