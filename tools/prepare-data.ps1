# 一键准备运行环境
#
# 从原版镜像（CLOSETO.mdf）或安装包（Setup.exe）里取出游戏数据，
# 放到 bin\Release 旁边，这样就可以直接跑编译出来的 CloseTo.exe。
#
# 数据不进仓库（总量约 900 MB，GitHub 放不下），所以每次换机器都要跑一遍。
#
# 用法：
#   .\tools\prepare-data.ps1 -Source "D:\Download\思念的碎片\CLOSETO.mdf"
#   .\tools\prepare-data.ps1 -Source "D:\somewhere\Setup.exe"
#
# 需要 7-Zip（7z.exe）。没有的话先装：https://www.7-zip.org/

param(
    [Parameter(Mandatory=$true)][string]$Source,
    [string]$SevenZip = "",
    [string]$OutDir  = ""
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot

if (-not $OutDir) { $OutDir = Join-Path $root 'bin\Release' }
if (-not (Test-Path $OutDir)) { New-Item -ItemType Directory -Force -Path $OutDir | Out-Null }

# --- 找 7z ---
if (-not $SevenZip) {
    foreach ($p in @(
        "$env:ProgramFiles\7-Zip\7z.exe",
        "${env:ProgramFiles(x86)}\7-Zip\7z.exe",
        (Join-Path $root 'tools\7z.exe')
    )) {
        if (Test-Path $p) { $SevenZip = $p; break }
    }
}
if (-not $SevenZip -or -not (Test-Path $SevenZip)) {
    throw "找不到 7z.exe，请用 -SevenZip 指定路径，或从 https://www.7-zip.org/ 安装"
}
Write-Host "7-Zip: $SevenZip"

# --- 定 Setup.exe ---
$setup = $null
if ($Source -match '\.mdf$|\.iso$') {
    Write-Host "从镜像里取 Setup.exe ..."
    & $SevenZip x $Source -o"$OutDir\_iso" Setup.exe -y | Out-Null
    $setup = Join-Path $OutDir '_iso\Setup.exe'
} else {
    $setup = $Source
}
if (-not (Test-Path $setup)) { throw "找不到 Setup.exe: $setup" }

# --- 取数据与运行期 DLL ---
$items = @(
    'bg.dat', 'bg.zh-CHS.dat',
    'bgm.dat', 'bgm.zh-CHS.dat',
    'chara.dat',
    'font.dat', 'font.zh-CHS.dat',
    'movie.dat',
    'script.dat', 'script.zh-CHS.dat',
    'se.dat',
    'system.dat', 'system.zh-CHS.dat',
    'voice.dat',
    'language.xml',
    'DirectShowLib-2005.dll',
    'PlayOgg.dll',
    'CloseToSetting.exe',
    'zh-CHS\*',
    'Bgm\*'
)

Write-Host "提取数据（约 900 MB）..."
& $SevenZip x $setup -o"$OutDir" @items -y | Out-Null

# 顺带把编译用的依赖也放过去
Copy-Item (Join-Path $root 'deps\*.dll') $OutDir -Force

# 清掉临时目录
Remove-Item (Join-Path $OutDir '_iso') -Recurse -Force -ErrorAction SilentlyContinue

$total = (Get-ChildItem $OutDir -File | Measure-Object Length -Sum).Sum
Write-Host ("完成，输出目录 {0}（{1:N1} MB）" -f $OutDir, ($total / 1MB))
Write-Host "现在可以直接运行 bin\Release\CloseTo.exe"
