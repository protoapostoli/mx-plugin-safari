#!/usr/bin/env bash
# tools/restart-service.sh - Gracefully restart LogiPluginService
DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
exec "$DIR/bin/mxdev" restart
