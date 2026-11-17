#!/usr/bin/env bash
set -euo pipefail
echo "Database reset is disabled to protect existing data." >&2
echo "To restart without deleting data, run: docker compose restart sqlserver" >&2
exit 1
