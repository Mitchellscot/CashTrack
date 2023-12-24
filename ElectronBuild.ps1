param(
    [Parameter(Mandatory=$false)]
    [string]$Framework = "win"
)

Write-Host "Building an Electron app for $Framework"

Remove-Item -Path .\src\obj\ -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path .\src\bin\ -Recurse -Force -ErrorAction SilentlyContinue
dotnet clean .\src\CashTrack.csproj -v q

. .\New-Db.ps1
#new-item -ItemType Directory -path .\src\obj\Host\bin\Data -Force
#Copy-Item -Path .\src\Data\cashtrack.db* -Destination .\src\obj\Host\bin\ -Force
Set-Location .\src\
electronize build /target $Framework /PublishSingleFile false