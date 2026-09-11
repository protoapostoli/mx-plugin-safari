#!/usr/bin/env bash
# tools/toggle-dev-mode.sh - Enable or disable DeveloperMode in LoupedeckSettings.ini
DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
exec "$DIR/bin/mxdev" dev-mode "$@"
