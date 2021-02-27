# Entity Framework (EF) Core 3 + 5

Information about EF Core 3 + 5

## Create Console App

Partly from: https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

```cmd
cd c:\ temp
dotnet new console -o EFGetStarted
cd EFGetStarted

dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

<details>
  <summary>Here you can find the DBContext and 2 entities aka database tables:</summary>

```cs
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BloggingEF5_Test01");
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; } = new List<Post>();
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
```

</details>

```cmd
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate

REM This will create the database now:
dotnet ef database update
```

## EF Core releases and planning

https://docs.microsoft.com/en-us/ef/core/what-is-new/

## Compare EF Core & EF6

https://docs.microsoft.com/en-us/ef/efcore-and-ef6/

## Update EF6 -> EF Core: Generate Models and Context from existing database (reverse engineer)

Create a new .NET Core console app. Install Sql Server Client and CLI tools:

```cmd
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.SqlServer.Design
Install-Package Microsoft.EntityFrameworkCore.Tools
```

Scaffold entities from existing database:

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

## EF 3+5: TPH (Table Per Hierarchy)

<https://docs.microsoft.com/en-us/ef/core/modeling/inheritance>

By default, EF Core maps an inheritance hierarchy of .NET types to a single database table. This is known as table-per-hierarchy (TPH) mapping.

TPH is implemented by inheritance:

```cs
public class Dog : Animal
```

A new field `discriminator` will be added:

```sql
 [Discriminator]  NVARCHAR (MAX) NOT NULL,
```

## EF 5: TPT (Table Per Type)

<https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-5.0/whatsnew#table-per-type-tpt-mapping>
