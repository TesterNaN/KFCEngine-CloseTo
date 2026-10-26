# KFCEngine v3 —《思念的碎片 -Close to-》

《思念的碎片 -Close to-》简体中文版（KIDS Fans Channel 2008，北京娱乐通代理）的
运行时逆向还原。

这是 KFCEngine 系列的第三个产物：**引擎、数据、素材都在**，编译出来就能直接玩。

- 第一代：[KFCEngine-Yukihota](https://github.com/TesterNaN/KFCEngine-Yukihota) —《雪萤》
- 第二代：[KFCEngine-Again](https://github.com/TesterNaN/KFCEngine-Again) —《秋之回忆：从今以后 AGAIN》

---

## 这套东西是什么

原版是三层结构：

```
CloseTo.exe              原生启动器，7.0 MB，SecuROM 7.38 加壳
  └─ 资源 id=103         2,078,216 字节，3DES 加密的托管程序集
       └─ CloseTo.Managed.exe    完整的 .NET 2.0 引擎
```

启动器把程序集解出来交给 `Invoke.dll`：

```csharp
// KidFansChannel.Native.Invoke.StartProgram(byte[] prog, string pwd, string[] args)
string keyText = "KIDS Fans Channel";      // 3DES key，补零到 24 字节
byte[] iv = pwd 的前 8 字节;                // pwd = "CloseTo"
byte[] plain = new byte[prog.Length - 8];  // 砍掉末尾 8 字节
TripleDESCryptoServiceProvider.CreateDecryptor(key, iv);  // 解密
Assembly.Load(plain).EntryPoint.Invoke(null, args[1..]);
```

本仓库 `src/` 里就是那个 2,078,216 字节程序集反编译出来的全部源码。

数据包是 KFCD 容器，密钥在 `KidFansChannel.DataCore.DataDecoder` 里写死：

```csharp
private static readonly string key = "www.KIDS FansChannel.net";
private static readonly string iv  = "ctMullen";
```

---

## 编译

只需要 .NET Framework 2.0 自带的 `csc.exe`（与原始程序集同代工具链）。

```
msbuild CloseTo.csproj /t:Build
```

> 用 `/t:Build` 而不是 `/t:Rebuild`——`Rebuild` 会先清空 `bin\`，
> 那里放着你准备好的 900 MB 数据，删了要重来。

产物：`bin\Release\CloseTo.exe`（约 2.07 MB）。

编译要求：

- .NET Framework 2.0（`C:\Windows\Microsoft.NET\Framework\v2.0.50727`）
- MSBuild 4.0（`C:\Windows\Microsoft.NET\Framework\v4.0.30319`）
- Windows PowerShell 5.1（把 resx 转 resources，见下文）

### 关于 resx

`src/KidFansChannel.CloseTo.Resource.resx`（1.8 MB）里有 `script_lang_dat`、
光标、界面文本，是运行时必需的，不能删。

它**必须**用 Windows PowerShell 5.1 转，因为：

1. resx 里有 BinaryFormatter 序列化的二进制项，.NET 5+ 已经移除该支持；
2. PowerShell 5.1 的 `ResourceWriter` 写出的程序集引用是 `Version=4.0.0.0`，
   而程序跑在 .NET 2.0 上只有 `2.0.0.0`，所以脚本里会做一次字节级替换改回 2.0。

`tools/make-resources.ps1` 处理这两件事，csproj 会自动调用它。

---

## 跑起来

数据不在仓库里（约 910 MB，GitHub 放不下）。两条路，任选一条。

### 方式一：从原版镜像取数据

手上有 `CLOSETO.mdf`（1.43 GB 光盘镜像）就用这个。

```
.\tools\prepare-data.ps1 -Source "D:\Download\思念的碎片\CLOSETO.mdf"
```

### 方式二：直接下载数据包

不想折腾镜像的话，数据包已经打好了（1.09 GB，含全部素材）：

| 网盘 | 链接 |
|---|---|
| Google Drive | *(待填)* |
| 百度网盘 | *(待填，提取码：*) |

下载 `CloseTo-data.7z.001` 后：

```
.\tools\restore-data.ps1 -Archive "D:\Downloads\CloseTo-data.7z.001"
```

脚本会先核对 SHA256 再解包，文件对不上就停下：

```
SHA256  9B11EAC363536909F845A0C9551B39EA2CDCB907B988AD0A7F0288678E244D67
大小    1,140,073,389 字节
```

### 数据里有什么

不管走哪条路，`bin\Release` 最后会是这样：

| 文件 | 大小 | 内容 |
|---|---|---|
| `bg.dat` | 329 MB | 393 张背景 PNG（960×720） |
| `chara.dat` | 368 MB | 角色立绘 |
| `voice.dat` | 190 MB | 语音 |
| `system.dat` | 9.8 MB | 系统脚本 |
| `system.zh-CHS.dat` | 3.1 MB | 简体覆盖 |
| `font.zh-CHS.dat` | 2.4 MB | 中文字体 |
| `se.dat` | 2.7 MB | 音效 |
| `script.dat` | 1.2 MB | 剧情脚本 |
| `Bgm/` | 60 个 | 背景音乐 |

`.zh-CHS.dat` 是语言覆盖包，引擎查文件时会优先命中它们
（`DataCore.CheckFile` / `CheckPackage` 里那套优先顺序）。

第一次启动会调用 `CloseToSetting.exe` 让你选显示设备，选项存在
`%LOCALAPPDATA%\KID\CloseTo\Options.xml`。

---

## 源码状态

239 个文件、51,532 行，`msbuild` 干净出包，能跑完整游戏。

反编译用的是 `ilspycmd -lv CSharp2`——默认的 C# 12 输出会带 file-scoped
namespace，.NET 2.0 的编译器不认。

为了让它在 2.0 上编译，动过这些地方：

1. ILSpy 把 COM 的 `out` 参数写成了 `ref`（`VideoCore.cs`），改回 `out`；
2. 有个 COM 属性访问被写成索引器 `((IMediaPosition)val)[ref result]`，改成
   `get_CurrentPosition(out result)`；
3. `VideoCore/Allocator.cs` 里枚举和整数直接 `|`，C# 2.0 不允许，加了 `(int)`；
4. `Program.cs` 加了一行把异常写进 `error.log`，方便排查。

局部变量名仍是反编译器生成的 `num` / `val` / `flag`（这一点和
[KFCEngine-Again](https://github.com/TesterNaN/KFCEngine-Again) 不同，那边已经
做过语义化）。因为原程序集的 PDB 没有随发行版流出：

```
PDB: H:\KFCProgram\CloseTo\Project\Managed\NoStaff\CloseTo.pdb   （仅路径，无文件）
```

---

## 和另外两代的关系

| | v1《雪萤》 | v2《AGAIN》 | **v3《Close to》** |
|---|---|---|---|
| 发行 | 2006-03 | 2007 秋 | 2008-10 |
| 运行时 | .NET 1.1 | .NET 2.0 | .NET 2.0 |
| 保护 | XenoCode PostBuild | StarForce FrontLine 5 | SecuROM 7.38.0007 |
| 程序集 | 混淆 | 2,686,472 字节 | 2,078,216 字节 |
| 反编译文件 | — | 216 | 239 |
| 数据 | 无 | 部分 | 全套 |
| 可运行 | — | — | ✓ |

三代是同一条引擎线上长出来的：雪萤定下底子，AGAIN 换成 .NET 2.0 并加上
DirectShow 视频，Close to 在这一套上扩了地图（`CloseTo.Commands.Map`）和
房间（`CloseTo.Commands.Room`）两类指令，文件数从 216 涨到 239。

密钥也跟着换了一代：

```
                 key                              iv
v2 AGAIN   "KID Fans Channel"                "Again"
v3 CloseTo "KIDS Fans Channel"               "CloseTo"
数据        "www.KID Fans Channel.net"         "v2Mullen"     （v2）
数据        "www.KIDS FansChannel.net"          "ctMullen"     （v3）
```

v3 这边 `KID` 变成 `KIDS`，`v2Mullen` 变成 `ctMullen`（CloseTo + Mullen）。

---

## 已知问题

- 局部变量名未语义化（见上）
- `DirectShowLib` 必须用游戏自带的 `DirectShowLib-2005.dll`（引用 `mscorlib 2.0.0.0`）。
  如果误用别处那份 2.1.0.1599 版（.NET 4 编译），`LogoScreen` 播影片时会抛
  `BadImageFormatException`，表现为黑屏。
- 必须是 32 位进程（`/platform:x86`）。`AnyCPU` 在 64 位系统上加载 32 位的
  `CloseToGDF.dll` 会失败，报 `0x800700C1`。

---

## 版权

引擎代码版权属 **KIDS Fans Channel**（2008，`Copyright (C) KIDS Fans Channel 2008`）。
原作《Close to ～祈りの丘～》版权属 **KID**。

本仓库仅作技术存档与研究，不含任何游戏数据、素材或原版可执行文件。
详见 [NOTICE.md](NOTICE.md)。
