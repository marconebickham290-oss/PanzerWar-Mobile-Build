#!/data/data/com.termux/files/usr/bin/bash
set -euo pipefail
USER=${1:-}
REPO=${2:-}
if [ -z "$USER" ] || [ -z "$REPO" ]; then
  echo "用法: bash init_repo.sh GitHub用户名 仓库名"
  exit 2
fi
if [ ! -d .git ]; then git init; fi
git add .
git commit -m "Panzer War Leopard1 cloud build bootstrap" || true
git branch -M main
gh repo create "$USER/$REPO" --private --source=. --remote=origin --push
printf '\n仓库已创建。接下来在 GitHub Actions Secrets 中配置 Unity 许可证。\n'
