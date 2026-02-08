# AG-Exercise

# Requirements

Write a Blazor Web Assembly application with the following requirements:

- Create a 'Railcar Trips' page.
- The page has a way to process railcar/equipment events into trips by uploading a file such as the attached: equipment_events.csv. Note: event times are local to the time zone of the corresponding city and are not ordered in the attached file.
- Events should be processed into trips per railcar/equipment and stored in a database where Trip is a parent.
- The logic for processing trips is as follows:
  - Event code W (Released) starts a new trip.
  - Event code Z (Placed) ends a trip.
- Trips should contain fields:  equipment id, origin city id, destination city id, start utc, end utc, and total trip hours (all stored in the database).
- The railcar trips page should show the processed trips in a grid (equipment id, origin, destination, start date/time, end date/time, total trip hours).
- There should be a way to select a particular trip and see its events in order (bonus / nice to have).
- Entity Framework should be used for database access.
- Note: the other 2 CSV files can be scripted into the database (don't need a UI to upload or configure).

Please use your discretion for how much time to spend on this. We’re looking to understand how you would organize an appropriately sized solution for this task and considerations you’ve made, rather than a complete fully flushed out working solution. TODO comments are appropriate to show considerations you’ve made that you don’t have time to fully implement. Please also list questions you have and the assumptions you’ve made to answer them.

Attachments:
- [canadian_cities.csv](https://github.com/user-attachments/files/25063229/canadian_cities.csv)
- [event_code_definitions.csv](https://github.com/user-attachments/files/25083280/event_code_definitions.csv)
- [equipment_events.csv](https://github.com/user-attachments/files/25063212/equipment_events.csv)

# Local Setup

1. ASP.NET [development certificate](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs) - if not already trusted, run `dotnet dev-certs https --trust` in a terminal and follow the prompts to trust the certificate.
1. If storing DB connection string using user secrets, run `dotnet user-secrets set "ConnectionStrings:RailcarDb" "Server=localhost,1433;Database=RailcarDb;User Id=sa;Password=Passw0rd;TrustServerCertificate=True" --project `.


## Database setup

1. Start Docker SQL Server instance

```PowerShell
docker run -e "ACCEPT_EULA=Y" `
   -e "SA_PASSWORD=Passw0rd" `
   -p 1433:1433 `
   --name ag_exercise `
   -d mcr.microsoft.com/mssql/server:2025-latest
```

1. Create database

```PowerShell
docker exec ag_exercise /opt/mssql-tools18/bin/sqlcmd `
  -S localhost -U sa -P 'Passw0rd' -C `
  -Q "IF DB_ID('RailcarDb') IS NULL CREATE DATABASE RailcarDb;"
```

1. Restore EF Core tools

```PowerShell
dotnet tool restore
```

## Data model diagram

![Railcar data model](docs/images/railcar-data-model.png)
