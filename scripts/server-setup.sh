#!/usr/bin/env bash
# One-off setup of the home server for CI deploys. Run as root: sudo ./server-setup.sh <path/to/docker-compose.yml>
# Safe to re-run: existing .env files are kept, everything else is overwritten.
set -euo pipefail

COMPOSE_SRC=${1:?usage: $0 <path/to/docker-compose.yml>}
ROOT=/srv/meal-planner

[ "$(id -u)" -eq 0 ] || { echo "Run as root." >&2; exit 1; }
[ -f "$COMPOSE_SRC" ] || { echo "$COMPOSE_SRC not found." >&2; exit 1; }
command -v docker >/dev/null || { echo "docker is not installed." >&2; exit 1; }
docker compose version >/dev/null || { echo "docker compose plugin is missing." >&2; exit 1; }
command -v tailscale >/dev/null || { echo "tailscale is not installed." >&2; exit 1; }
command -v visudo >/dev/null || { echo "visudo is not installed." >&2; exit 1; }

# Deploy user: no password, not in the docker group. It can only reach docker through the sudo rule below.
id deploy &>/dev/null || useradd --create-home --shell /bin/bash deploy
passwd -l deploy >/dev/null

# One shared compose file; one directory per environment holding its own .env (own port and DB credentials).
mkdir -p "$ROOT"
# container_name is fixed in the repo's compose file and would collide across the two projects.
sed '/^[[:space:]]*container_name:/d' "$COMPOSE_SRC" > "$ROOT/docker-compose.yml"

setup_env() {
  local port=$2 aspnet_env=$3 dir=$ROOT/$1
  mkdir -p "$dir"
  if [ ! -f "$dir/.env" ]; then
    umask 077
    cat > "$dir/.env" <<EOF
APP_PORT=$port
ASPNETCORE_ENVIRONMENT=$aspnet_env
POSTGRES_DB=meal_planner
POSTGRES_USER=meal_planner
POSTGRES_PASSWORD=$(head -c 32 /dev/urandom | base64 | tr -dc 'A-Za-z0-9' | head -c 32)
EOF
    umask 022
  fi
}
setup_env prod 5300 Production
setup_env dev 5301 Development

# The only thing the deploy user may run as root.
cat > /usr/local/sbin/meal-planner-deploy <<'EOF'
#!/usr/bin/env bash
set -euo pipefail
env=${1:-}
tag=${2:-}
[[ "$env" =~ ^(dev|prod)$ ]] || { echo "env must be dev or prod" >&2; exit 1; }
[[ "$tag" =~ ^[0-9A-Za-z._-]+$ ]] || { echo "invalid tag" >&2; exit 1; }
exec 9>"/run/meal-planner-deploy-$env.lock"
flock 9
cd /srv/meal-planner
MEAL_PLANNER_VERSION=$tag docker compose -p "meal-planner-$env" --env-file "$env/.env" up -d --pull always
EOF
chown root:root /usr/local/sbin/meal-planner-deploy
chmod 755 /usr/local/sbin/meal-planner-deploy

# What the workflow calls: `deploy <env> <tag>`.
cat > /usr/local/bin/deploy <<'EOF'
#!/usr/bin/env bash
exec sudo /usr/local/sbin/meal-planner-deploy "$@"
EOF
chmod 755 /usr/local/bin/deploy

# Validate the sudoers file before installing it, so a typo can't break sudo.
tmp=$(mktemp)
echo 'deploy ALL=(root) NOPASSWD: /usr/local/sbin/meal-planner-deploy' > "$tmp"
visudo -cf "$tmp"
install -m 440 -o root -g root "$tmp" /etc/sudoers.d/meal-planner-deploy
rm "$tmp"

tailscale set --ssh

cat <<EOF

Done. Still to do by hand:
  1. Tag this machine with tag:homeserver (admin console), if not already done.
  2. Expose each port on the tailnet, e.g. prod 127.0.0.1:5300 and dev 127.0.0.1:5301 (see 'tailscale serve --help').
  3. Let docker pull from ghcr: make the package public, or 'docker login ghcr.io' as root with a read:packages token.
  4. Check $ROOT/{prod,dev}/.env.
EOF
