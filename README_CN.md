# 装甲纷争 Leopard 1：纯手机 Termux + 云端 Unity 构建骨架

这个包解决的是“没有电脑”的工作流问题：手机只用 Termux；Unity Editor 放到 GitHub Actions 的 x86_64 Linux runner 上执行。

## 已经包含

- 已整理的 Leopard 1 模型/节点/贴图准备资源
- Termux 初始化脚本
- Termux 一键触发/下载脚本
- GitHub Actions Unity 构建工作流
- 官方 Panzer War 持续构建 manifest 模板

## 重要限制

1. Unity CI 必须有有效 Unity 许可证。免费 Personal 的当前 GameCI 流程通常要求先取得 `UNITY_LICENSE` (`.ulf`) 并写入 GitHub Secrets。
2. Leopard 1 的几何 Prefab 已能自动生成，但**真正的 Panzer War Vehicle / BuildPipline / ModPackage 资产仍必须由官方 Vehicle Mod SDK 创建或由一个现成车辆模板派生**。没有这些 SDK 资产时，云端脚本会明确报错停止，不会生成假的 `.modpack`。
3. 官方持续构建入口是：
   `ShanghaiWindy.Editor.Utility_BuildPipline.DoBuildPiplineManifest`

## 手机上第一次设置

```bash
cd ~
unzip PanzerWar_Termux_CloudBuild.zip
cd PanzerWar_Termux_CloudBuild
bash termux_setup.sh
```

登录 GitHub：

```bash
gh auth login
```

然后创建一个空 GitHub 仓库，再执行：

```bash
bash init_repo.sh 你的GitHub用户名 你的仓库名
```

## GitHub Secrets

仓库网页：Settings -> Secrets and variables -> Actions

至少需要：

- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

如果没有 `UNITY_LICENSE`，工作流会停在授权阶段。

## 一键触发

```bash
bash build.sh
```

构建成功后脚本会尝试下载 artifact 到：

```text
./downloads/
```

如果 artifact 内包含 `.modpack`，再复制到游戏：

```text
Android/data/com.shanghaiwindy.PanzerWarOpenSource/files/mods/Installs/
```

Android 11+ 对 `Android/data` 有访问限制；可以使用系统文件选择器/Shizuku/你已有的文件管理方式移动文件。

## Panzer War manifest

`cloud/Leopard1BuildManifest.json` 当前的 `modNames` 是占位名 `BuildPipline-Vehicle-Leopard1`。

当 SDK 中真正的 BuildPipline 资产名称确定后，把它改为真实名称即可。官方规则：如果 BuildPipline 位于 `Assets/ModManager/AAAA/BBBB/...`，manifest 中也要带相同的子目录前缀。

## 当前状态

这套包已经把“手机 -> GitHub -> 云端 Unity -> 官方 batchmode 构建 -> 下载产物”的链路搭好。
真正生成可玩的豹一，还剩 **Unity 授权** 和 **Panzer War Vehicle SDK 资产绑定** 两个硬条件。
