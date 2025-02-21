#to run this you need to have the dotnet-ef tool installed 
#that matches the nuget package installed

Write-Host "Installing Dotnet EF version 9.0.2" 
dotnet tool install --global dotnet-ef --version 9.0.2
Write-Host "Creating new Migration Files" 
dotnet ef migrations add SeededInit --no-build --project ./src/CashTrack.csproj -o ./Data/Migrations -- seed
Write-Host "Creating new SQLite Database"
dotnet ef database update --project ./src/CashTrack.csproj