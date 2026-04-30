#!/usr/bin/env bash
set -euo pipefail

APP_NAME="blazortemplate"
SERVICE_NAME="blazortemplate.service"
TARGET_ROOT="/var/www/blazortemplate"
TARGET_APP="${TARGET_ROOT}/app"
TARGET_DATA="${TARGET_ROOT}/data"
PUBLISH_DIR="/home/admin/Dokumente/köö/LAP_Templates/_deploy/BlazorTemplate/linux-arm64-stable"
PROJECT_DIR="/home/admin/Dokumente/köö/LAP_Templates/BlazorTemplate"
PROJECT_FILE="${PROJECT_DIR}/BlazorTemplate.csproj"
SERVICE_TEMPLATE="/home/admin/Dokumente/köö/LAP_Templates/_deploy/BlazorTemplate/blazortemplate.service"
RUN_USER="${RUN_USER:-admin}"
RUN_GROUP="${RUN_GROUP:-www-data}"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet is not installed or not in PATH" >&2
  exit 1
fi

if ! command -v rsync >/dev/null 2>&1; then
  echo "rsync is not installed; install it first" >&2
  exit 1
fi

echo "[1/7] Publishing app (linux-arm64, Release)"
dotnet publish "${PROJECT_FILE}" -c Release -r linux-arm64 --self-contained true -o "${PUBLISH_DIR}"

echo "[2/7] Creating target directories"
sudo mkdir -p "${TARGET_APP}" "${TARGET_DATA}"

echo "[3/7] Syncing published files to ${TARGET_APP}"
sudo rsync -a --delete "${PUBLISH_DIR}/" "${TARGET_APP}/"

echo "[4/7] Ensuring executable bit"
sudo chmod +x "${TARGET_APP}/BlazorTemplate"

echo "[5/7] Migrating existing database if target is empty"
if [ ! -f "${TARGET_DATA}/koowebsite.db" ] && [ -f "${PROJECT_DIR}/data/koowebsite.db" ]; then
  sudo cp "${PROJECT_DIR}/data/koowebsite.db" "${TARGET_DATA}/koowebsite.db"
fi
if [ -f "${PROJECT_DIR}/data/koowebsite.db-wal" ]; then
  sudo cp "${PROJECT_DIR}/data/koowebsite.db-wal" "${TARGET_DATA}/koowebsite.db-wal" || true
fi
if [ -f "${PROJECT_DIR}/data/koowebsite.db-shm" ]; then
  sudo cp "${PROJECT_DIR}/data/koowebsite.db-shm" "${TARGET_DATA}/koowebsite.db-shm" || true
fi

echo "[6/7] Setting ownership and permissions"
sudo chown -R "${RUN_USER}:${RUN_GROUP}" "${TARGET_ROOT}"
sudo find "${TARGET_APP}" -type d -exec chmod 755 {} \;
sudo find "${TARGET_APP}" -type f -exec chmod 644 {} \;
sudo chmod +x "${TARGET_APP}/BlazorTemplate"
sudo find "${TARGET_DATA}" -type d -exec chmod 755 {} \;
sudo find "${TARGET_DATA}" -type f -exec chmod 664 {} \;

echo "[7/7] Installing and restarting systemd service"
TMP_SERVICE="$(mktemp)"
sed "s/^User=.*/User=${RUN_USER}/; s/^Group=.*/Group=${RUN_GROUP}/" "${SERVICE_TEMPLATE}" > "${TMP_SERVICE}"
sudo cp "${TMP_SERVICE}" "/etc/systemd/system/${SERVICE_NAME}"
rm -f "${TMP_SERVICE}"

sudo systemctl daemon-reload
sudo systemctl enable "${SERVICE_NAME}"
sudo systemctl restart "${SERVICE_NAME}"

echo "Deployment complete. Service status:"
systemctl --no-pager --full status "${SERVICE_NAME}" | sed -n '1,15p'

echo "If an old dev service exists, disable it manually, e.g.:"
echo "  sudo systemctl disable --now <old-service-name>"
