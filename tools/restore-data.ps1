# 从下载的数据包还原运行环境
#
# 适用于「直接下载数据包」的方式：拿到 CloseTo-data.7z.001 之后跑这个，
# 它会先校验 SHA256，再解到 bin\Release。校验不过就不会解。
#
# 用法：
#   .\tools\restore-data.ps1 -Archive "D:\Downloads\CloseTo-data.7z.001"
#
# 需要 7-Zip（7z.exe）。没有的话先装：https://www.7-zip.org/

param(
    [Parameter(Mandatory=$true)][string]$Archive,
    [string]$SevenZip = "",
    [string]$OutDir  = "",
    [switch]$SkipHash
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot

if (-not $OutDir) { $OutDir = Join-Path $root 'bin\Release' }
if (-not (Test-Path $OutDir)) { New-Item -ItemType Directory -Force -Path $OutDir | Out-Null }

if (-not (Test-Path $Archive)) { throw "找不到数据包: $Archive" }

# --- 期望的校验和（与仓库里 SHA256SUMS.md 保持一致）---
$expected = 'B508033D29A2B6729724B54BDE47A89734EEDD63AF3B3C3F1567DBD88FB6472C'

if (-not $SkipHash) {
    Write-Host "校验 SHA256 ..."
    $actual = (Get-FileHash $Archive -Algorithm SHA256).Hash
    if ($actual -ne $expected) {
        throw "校验失败`n  期望: $expected`n  实际: $actual`n文件可能损坏或被改动，已停止。"
    }
    Write-Host "校验通过"
}

# --- 找 7z ---
if (-not $SevenZip) {
    foreach ($p in @(
        "$env:ProgramFiles\7-Zip\7z.exe",
        "${env:ProgramFiles(x86)}\7z.exe",
        "${env:ProgramFiles(x86)}\7-Zip\7z.exe",
        (Join-Path $root 'tools\7z.exe')
    )) {
        if (Test-Path $p) { $SevenZip = $p; break }
    }
}
if (-not $SevenZip -or -not (Test-Path $SevenZip)) {
    throw "找不到 7z.exe，请用 -SevenZip 指定路径"
}

Write-Host "解包到 $OutDir ..."
& $SevenZip x $Archive -o"$OutDir" -y | Out-Null

$total = (Get-ChildItem $OutDir -File | Measure-Object Length -Sum).Sum
Write-Host ("完成（{0:N1} MB）" -f ($total / 1MB))
Write-Host "现在可以直接运行 bin\Release\CloseTo.exe"
