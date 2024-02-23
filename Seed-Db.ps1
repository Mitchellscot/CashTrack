#to run this you need to have the dotnet-ef tool installed

#Remove the migration files
Get-ChildItem  ./src/Data/Migrations -Recurse | Remove-Item
#Remove the old database
$dbPath = "./src/wwwroot/data/cashtrack.db*"
$secondDbPath = "./src/Data/CashTrack.db*"
if(Test-Path $dbPath){
    Write-Host "Removing old SQLite Database"
    Get-ChildItem $dbPath | Remove-Item
}
else{
    Write-Host "Removing old SQLite Database"
    Get-ChildItem $secondDbPath | Remove-Item
}
#Create new EF Migration files
Write-Host "Creating new Migration Files" 
dotnet ef migrations add SeededInit --project ./src/CashTrack.csproj -o ./Data/Migrations -- seed
#Create a new database
Write-Host "Creating new SQLite Database"
dotnet ef database update --project ./src/CashTrack.csproj