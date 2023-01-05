#Remove the migration files
Get-ChildItem  .\src\Data\Migrations -Recurse | Remove-Item
#Remove the old database
Get-ChildItem .\src\cashtrack.db | Remove-Item
#Create new EF Migration files 
dotnet ef migrations add SQLightInit --project .\src\CashTrack.csproj -o .\Data\Migrations -- seed
#Create a new database
dotnet ef database update --project .\src\CashTrack.csproj