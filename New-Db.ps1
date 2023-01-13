#Remove the migration files
Get-ChildItem  ./src/Data/Migrations -Recurse | Remove-Item
#Remove the old database
$dbPath = "./src/cashtrack.db*"
if(Test-Path $dbPath){
    Get-ChildItem $dbPath | Remove-Item
}
#Create new EF Migration files
Write-Host "Creating new Migration Files" 
dotnet ef migrations add SQLiteInit --project ./src/CashTrack.csproj -o ./Data/Migrations -- seed
#Create a new database
Write-Host "Creating new SQLite Database"
dotnet ef database update --project ./src/CashTrack.csproj