Remove-Item -Path .\src\obj\ -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path .\src\bin\ -Recurse -Force -ErrorAction SilentlyContinue

$process = Get-Process -Name 'Electron' -ErrorAction SilentlyContinue
if ($process) {
    Stop-Process -Name 'Electron' -Force
    Write-Host "Process 'Electron' has been terminated."
} else {
    Write-Host "Process 'Electron' is not running."
}

. .\Seed-Db.ps1
new-item -ItemType Directory -path .\src\obj\Host\bin\Data -Force
Copy-Item -Path .\src\Data\cashtrack.db* -Destination .\src\obj\Host\bin\Data -Force
Set-Location .\src\
electronize start /PublishSingleFile false /watch