#Remove the migration files
Get-ChildItem  .\src\Data\Migrations -Recurse | Remove-Item
#Create new EF Migration files 
dotnet ef migrations add init -o .\Data\Migrations --project .\src\CashTrack.csproj
#Remove the old database
$deleteDatabase = Start-Job {Invoke-Sqlcmd -ServerInstance '(localdb)\MSSQLLocalDB' -Database CashTrack -Query "USE master; ALTER DATABASE CashTrack SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE CashTrack;" }
Wait-Job $deleteDatabase
Receive-Job $deleteDatabase
#Create a new database
dotnet ef database update --project .\src\CashTrack.csproj