# 版权说明

本仓库涉及三层权利，分别归属不同主体。

## 1. 引擎代码

**KIDS Fans Channel**，2008 年。

依据（原程序集 `CloseTo.Managed.exe` 的元数据）：

```
AssemblyCompany    KIDS Fans Channel
AssemblyProduct    CloseTo
AssemblyCopyright  Copyright (C) KIDS Fans Channel 2008
AssemblyVersion    2.3.0.0
AssemblyDescription Omoi no Kakera -Close to- Executable File
```

`src/` 下的 239 个源文件是这个程序集的反编译产物，逻辑与原始实现一致。
本仓库对其作技术存档与可编译性还原，不主张任何权利。

## 2. 原作

**《Close to ～祈りの丘～》**，版权属 **KID**（株式会社キッド）。

- 2001-04-19：Dreamcast 版
- 2003-07-24：PS2 版，改题《想いのかけら -Close to-》
- 2008：PC 中文版（本仓库对应的是简体中文版）

人物、剧本、音乐、美术等原作内容的权利均属 KID 及其权利继受方。

## 3. 中文化

简体中文版由 **北京娱乐通** 代理发行，繁体中文版由 **英特衛多媒体** 代理。

---

## 本仓库不含什么

- **不含任何游戏数据**：`bg.dat`、`chara.dat`、`voice.dat`、`system.dat`、
  `script.dat`、`se.dat`、`font.*.dat`、`Bgm/` 均不在仓库内，由使用者
  从自备的原版镜像生成（见 `tools/prepare-data.ps1`）。
- **不含原版可执行文件**：`CloseTo.exe`（原生启动器）、`CloseToGDF.dll`、
  `PlayOgg.dll` 均不在仓库内。
- **不含资源内容**：`src/KidFansChannel.CloseTo.Resource.resx` 只保留结构与
  引擎运行所需的少量界面字符串，游戏素材本身不在此列。

## 用途

仅供技术研究与引擎演化研究。要游玩请支持正版。
