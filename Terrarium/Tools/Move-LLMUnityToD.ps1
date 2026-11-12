param([switch]$RemoveUnusedDeepSeek)
$ErrorActionPreference = 'Stop'
$src = [IO.Path]::GetFullPath((Join-Path $env:APPDATA 'LLMUnity'))
$dst = 'D:\LLMUnity'
$backup = "$src.migration-backup"
if (Get-Process Unity -ErrorAction SilentlyContinue) { throw 'Close Unity first. No changes made.' }
if (-not (Test-Path -LiteralPath $src)) { throw 'Source missing.' }
$item = Get-Item -LiteralPath $src -Force
if ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) { throw 'Source is already a link. Inspect manually.' }
if ((Test-Path -LiteralPath $dst) -or (Test-Path -LiteralPath $backup)) { throw 'Target or backup already exists.' }
$files = @(Get-ChildItem -LiteralPath $src -Recurse -File)
$total = ($files | Measure-Object Length -Sum).Sum
if ((Get-PSDrive D).Free -lt ($total + 1GB)) { throw 'Insufficient space on D:.' }
New-Item -ItemType Directory -Path $dst | Out-Null
& robocopy $src $dst /E /COPY:DAT /R:2 /W:2 /NFL /NDL /NJH /NJS
if ($LASTEXITCODE -ge 8) { throw 'Copy failed. Source retained.' }
foreach ($file in $files) {
    $relative = $file.FullName.Substring($src.Length + 1)
    $copy = Join-Path $dst $relative
    if ((Get-Item -LiteralPath $copy).Length -ne $file.Length -or (Get-FileHash -LiteralPath $file.FullName).Hash -ne (Get-FileHash -LiteralPath $copy).Hash) { throw "Verification failed: $relative" }
    Write-Host "SHA256 verified: $relative"
}
if (Get-Process Unity -ErrorAction SilentlyContinue) { throw 'Unity reopened. Source retained.' }
if ((Split-Path $src -Parent) -ne [IO.Path]::GetFullPath($env:APPDATA) -or (Split-Path $backup -Parent) -ne [IO.Path]::GetFullPath($env:APPDATA)) { throw 'Unexpected source path.' }
Move-Item -LiteralPath $src -Destination $backup
try {
    New-Item -ItemType Junction -Path $src -Target $dst | Out-Null
    foreach ($file in $files) {
        $relative = $file.FullName.Substring($src.Length + 1)
        if ((Get-Item -LiteralPath (Join-Path $src $relative)).Length -ne $file.Length) { throw 'Junction verification failed.' }
    }
} catch {
    if (Test-Path -LiteralPath $src) { [IO.Directory]::Delete($src) }
    Move-Item -LiteralPath $backup -Destination $src
    throw
}
Remove-Item -LiteralPath $backup -Recurse -Force
if ($RemoveUnusedDeepSeek) {
    $unused = Join-Path $dst 'models\DeepSeek-R1-Distill-Llama-8B-Q4_K_M.gguf'
    Remove-Item -LiteralPath $unused -Force
    Write-Host "Removed: $unused"
}
$global:LASTEXITCODE = 0
Get-Item -LiteralPath $src -Force | Format-List FullName,LinkType,Target
Get-ChildItem -LiteralPath (Join-Path $src 'models') -File | Select-Object Name,Length
