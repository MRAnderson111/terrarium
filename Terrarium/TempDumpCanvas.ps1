$path = "D:\Unity\terrarium\Terrarium\Assets\YoYoTest\TerrariumLevel 1.unity"
$text = [IO.File]::ReadAllText($path)
$docs = $text -split "`n--- !u!"
$gos = @{}
$transforms = @{}
foreach ($d in $docs) {
    if ($d -notmatch '(?m)^(\d+) &(\d+)') { continue }
    $typ = $Matches[1]; $fid = $Matches[2]
    if ($typ -eq '1') {
        $name = if ($d -match 'm_Name: (.*)') { $Matches[1].Trim() } else { '?' }
        $active = if ($d -match 'm_IsActive: (\d)') { $Matches[1] } else { '?' }
        $gos[$fid] = @{ name = $name; active = $active }
    }
    if ($typ -eq '4' -or $typ -eq '224') {
        $go = if ($d -match 'm_GameObject: \{fileID: (\d+)\}') { $Matches[1] } else { $null }
        $father = if ($d -match 'm_Father: \{fileID: (\d+)\}') { $Matches[1] } else { '0' }
        $children = @()
        $cm = [regex]::Match($d, 'm_Children:\s*(.*?)\s*m_Father:', 'Singleline')
        if ($cm.Success) {
            [regex]::Matches($cm.Groups[1].Value, '\{fileID: (\d+)\}') | ForEach-Object { $children += $_.Groups[1].Value }
        }
        $transforms[$fid] = @{ go = $go; father = $father; children = $children }
    }
}
function Dump($tid, $indent, $limit) {
    if ($indent -gt $limit) { return }
    $t = $transforms[$tid]
    $pad = '  ' * $indent
    if (-not $t) { Write-Output "$pad[missing $tid]"; return }
    $g = $gos[$t.go]
    $n = if ($g) { $g.name } else { '?' }
    $a = if ($g) { $g.active } else { '?' }
    Write-Output "$pad$n active=$a tf=$tid"
    foreach ($c in $t.children) { Dump $c ($indent+1) $limit }
}
Write-Output '=== ROOT CANVAS ==='
Dump '2114707062' 0 3
Write-Output '=== PROJECT CANVAS ==='
Dump '6254119161575108975' 0 2
Write-Output '=== ProjectProgram ==='
Dump '546235433' 0 1
