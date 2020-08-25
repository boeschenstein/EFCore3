# Entity Framework (EF) Core 3

Information about EF Core 3

## EF Core releases and planning

https://docs.microsoft.com/en-us/ef/core/what-is-new/

## Compare EF Core & EF6

https://docs.microsoft.com/en-us/ef/efcore-and-ef6/

## Update EF6 -> EF Core: Generate Models and Context from existing database (reverse engineer)

Create a new .NET Core console app. Install CLI tools. Scaffold entities from existing database:

```cmd
Scaffold-DbContext "Server=.\SQLExpress;Database=SchoolDB;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
```

https://www.entityframeworktutorial.net/efcore/create-model-for-existing-database-in-ef-core.aspx

> Do not convert Migration (there is no support) - start over with an empty migration

Details: https://docs.microsoft.com/en-us/ef/efcore-and-ef6/porting/port-code#update-your-code

## Power Tools

https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools

## CLI

Start following statements (`dotnet ef ...`) in a cmd, path must set to the folder the solution file (.sln), or choose different relative path settings for --project and --startup-project:

`dotnet tool install --global dotnet-ef`
`dotnet add package Microsoft.EntityFrameworkCore.Design`

Details: https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet

### EF Migrations Add

`dotnet ef migrations add --project .\MyApp.DataLayer --startup-project .\MyApp.Api MyNewMigration`

### EF Database Update

> Install migrations (which are not applied yet) in database

`dotnet ef database update --project .\MyApp.DataLayer --startup-project .\MyApp.Api`

### EF Migration (Rollback database changes - DATA LOSS MAY OCCURE - please check all Down() methods first)

`dotnet ef database update  <old_migration_filename_without_suffix> --project .\MyApp.DataLayer --startup-project .\MyApp.Api`

Example:

`dotnet ef database update 20200825193430_init --project .\MyApp.DataLayer --startup-project .\MyApp.Api`

### EF Migrations - Remove last migration file (if not applied in database yet)

`dotnet ef migrations remove --project .\MyApp.DataLayer --startup-project .\MyApp.Api`

### EF Issues

If you are stuck (parallel migrations from other users), it sometime helps to add an empty migration.
