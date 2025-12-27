##########
## Configure
##########
# The absolute path to the Stardew Valley game folder, used to resolve references.
$gamePath = 'C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley'

# The absolute path containing mods to decompile. This will be searched recursively.
$modsPath = "$gamePath\Mods"

# The absolute path to the folder in which to create the decompiled solution.
# NOTE: this folder will be deleted on each run.
$outputRootPath = "$gamePath\Mods (decompiled)"


##########
## Reset output folder
##########
if (Test-Path $outputRootPath) {
    Write-Host "Deleting $outputRootPath..."
    Remove-Item -Recurse -Force $outputRootPath
}


##########
## Decompile mod assemblies
##########
$projects = @()
Get-ChildItem -Path $modsPath -Recurse -File -Include *.dll, *.exe | ForEach-Object {
    $assemblyName = $_.BaseName
    $assemblyPath = $_.FullName
    $parentName   = $_.Directory.Name

    Write-Host "Decompiling $assemblyName..."

    try
    {
        # init output folder
        $projectFolderPath = Join-Path $outputRootPath $parentName $assemblyName
        New-Item -ItemType Directory -Path $projectFolderPath | Out-Null

        # decompile into project
        & ilspycmd `
            $assemblyPath `
            --outputdir $projectFolderPath `
            --referencepath "$gamePath" `
            --referencepath (Join-Path "$gamePath" 'smapi-internal') `
            --project `
            --use-varnames-from-pdb `
            --nested-directories `
            --disable-updatecheck | Out-Null

        # add project path to solution list
        $found = $false
        Get-ChildItem -LiteralPath $projectFolderPath -Filter *.csproj -Recurse | ForEach-Object {
            $found = $true

            $projects += [System.IO.Path]::GetRelativePath($outputRootPath, $_.FullName)
        }

        if ($found -eq $false) {
            Write-Warning "No .csproj found in $projectFolderPath"
        }
    }
    catch {
        Write-Error "Failed to decompile $($assemblyPath): $_"
    }
}


##########
## Create solution file
##########
Write-Host 'Preparing solution file...'

# group projects by their mod folder name (i.e. first part of their relative path)
$projectsByFolder = $projects | ForEach-Object {
    $relativePath = $_
    $folderName = ($relativePath -split [System.IO.Path]::DirectorySeparatorChar, 0, 'SimpleMatch')[0]

    return @{
        Folder = $folderName
        Path   = $relativePath
    }
} | Group-Object -Property 'Folder'

# generate solution XML
$solutionXml = "<Solution>`n"
foreach ($group in $projectsByFolder) {
    $folderName = $group.Name
    $solutionXml += "  <Folder Name=""/$folderName/"">`n"
    foreach ($proj in $group.Group) {
        $solutionXml += "    <Project Path=""$($proj.Path)"" />`n"
    }
    $solutionXml += "  </Folder>`n"
}
$solutionXml += "</Solution>`n"

# write file
$solutionPath = Join-Path $outputRootPath 'DecompiledMods.slnx'
$solutionXml | Set-Content -Path $solutionPath -Encoding UTF8

Write-Host "Solution created at $solutionPath"
