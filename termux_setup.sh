#!/data/data/com.termux/files/usr/bin/bash
set -e
pkg update -y
pkg install -y git gh unzip zip jq curl
mkdir -p downloads
printf '\n完成。下一步运行: gh auth login\n'
