# CashTrack
![CashTrack](src/wwwroot/images/cash-track.png)

This is a budgeting app.

To seed data in a new DB:
    -add the data project to the root folder
    -change the appsettings.development.json value CreateDb to true
    -run in the /src folder: dotnet ef migrations add init -o ./Data/Migrations
    -then run: dotnet ef database update
    -then change CreateDb back to false and save it.