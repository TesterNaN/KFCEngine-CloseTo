# Convert .resx to .resources.
# Must run under Windows PowerShell 5.1: only .NET Framework's
# ResXResourceReader can handle the BinaryFormatter items in this resx.
#
# Note: PS 5.1 writes assembly references as "Version=4.0.0.0", but the game
# runs on .NET 2.0 which only has 2.0.0.0. Rewrite those tags byte-safely
# (Latin1 maps 0..255 to 0..255, so nothing is lost).
param(
    [Parameter(Mandatory=$true)][string]$In,
    [Parameter(Mandatory=$true)][string]$Out
)

Add-Type -AssemblyName System.Windows.Forms

$reader = New-Object System.Resources.ResXResourceReader($In)
$writer = New-Object System.Resources.ResourceWriter($Out)
$n = 0
$skip = 0
foreach ($d in $reader.GetEnumerator()) {
    if ($d.Key -like '$this.*') { $skip++; continue }
    try { $writer.AddResource($d.Key, $d.Value); $n++ } catch { $skip++ }
}
$reader.Close()
$writer.Close()

# 4.0.0.0 -> 2.0.0.0
$bytes = [System.IO.File]::ReadAllBytes($Out)
$latin = [System.Text.Encoding]::GetEncoding(28591)   # ISO-8859-1
$text  = $latin.GetString($bytes)
$fixed = $text.Replace('Version=4.0.0.0', 'Version=2.0.0.0')
if ($fixed -ne $text) {
    [System.IO.File]::WriteAllBytes($Out, $latin.GetBytes($fixed))
    Write-Output "rewrote assembly refs to 2.0.0.0"
}

Write-Output ("resx -> resources: {0} items, {1} skipped" -f $n, $skip)
