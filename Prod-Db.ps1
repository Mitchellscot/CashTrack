Write-Host "Installing Dotnet EF" 
dotnet tool install --global dotnet-ef --version 6.0.21
Write-Host "Creating new Migration Files" 
dotnet ef migrations add SeededInit --no-build --project ./src/CashTrack.csproj -o ./Data/Migrations -- seed
Write-Host "Creating new SQLite Database"
dotnet ef database update --project ./src/CashTrack.csproj