#!/data/data/com.termux/files/usr/bin/bash
set -euo pipefail
mkdir -p downloads
BRANCH=$(git branch --show-current 2>/dev/null || echo main)
git add .
git commit -m "Trigger Leopard1 cloud build" || true
git push origin "$BRANCH"
gh workflow run panzerwar-build.yml --ref "$BRANCH"
echo "已触发。等待本次 workflow 出现..."
sleep 5
RUN_ID=$(gh run list --workflow panzerwar-build.yml --branch "$BRANCH" --limit 1 --json databaseId -q '.[0].databaseId')
if [ -z "$RUN_ID" ] || [ "$RUN_ID" = "null" ]; then
  echo "没有找到 workflow run，请到 GitHub Actions 查看。"
  exit 3
fi
gh run watch "$RUN_ID" --exit-status || {
  echo "云端构建失败。运行下面命令看日志："
  echo "gh run view $RUN_ID --log"
  exit 4
}
rm -rf downloads/*
gh run download "$RUN_ID" -D downloads || true
echo "下载完成："
find downloads -maxdepth 4 -type f -print
find downloads -type f -name '*.modpack' -print -quit | grep -q . && echo "找到 .modpack，可复制到装甲纷争 mods/Installs。" || echo "未找到 .modpack；查看云端日志判断是授权还是 SDK Vehicle/BuildPipline 尚未绑定。"
