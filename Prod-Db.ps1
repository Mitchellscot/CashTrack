Write-Host "Installing Dotnet EF" 
dotnet tool install --global dotnet-ef
Write-Host "Creating new Migration Files" 
dotnet ef migrations add SeededInit --no-build --project ./src/CashTrack.csproj -o ./Data/Migrations -- seed