# Entity Framework (EF) Core 3 + 5

Information about EF Core 3 + 5

> EF 5 can be used in Core 3 app.

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

> Create Database (if needed), Install all migrations (which are not applied yet) in database (check table __efmigrationshistory)

`dotnet ef database update --project .\MyApp.DataLayer --startup-project .\MyApp.Api`

### EF Migration (Rollback database changes - DATA LOSS MAY OCCURE - please check all Down() methods first)

`dotnet ef database update  <old_migration_filename_without_suffix> --project .\MyApp.DataLayer --startup-project .\MyApp.Api`

Example:

`dotnet ef database update 20200825193430_init --project .\MyApp.DataLayer --startup-project .\MyApp.Api`

### EF Migrations - Remove last migration file (if not applied in database yet)

`dotnet ef migrations remove --project .\MyApp.DataLayer --startup-project .\MyApp.Api`

### EF Issues

- Handling EF Core migrations in a team: <https://jkdev.me/handling-ef-core-migrations/>
- If you are stuck (parallel migrations from other users), it sometime helps to add an empty migration. (tbv: still valid in EF Core?)

## Inheritance

<https://docs.microsoft.com/en-us/ef/core/modeling/inheritance>

### EF 3+5: TPH (Table Per Hierarchy) - discriminator

<https://docs.microsoft.com/en-us/ef/core/modeling/inheritance#table-per-hierarchy-and-discriminator-configuration>

By default, EF Core maps an inheritance hierarchy of .NET types to a single database table. This is known as table-per-hierarchy (TPH) mapping.

TPH is implemented by inheritance:

```cs
public class Dog : Animal
```

A new field `discriminator` will be added:

```sql
 [Discriminator]  NVARCHAR (MAX) NOT NULL,
```

### EF 5: TPT (Table Per Type)

<https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-5.0/whatsnew#table-per-type-tpt-mapping>
<https://docs.microsoft.com/en-us/ef/core/modeling/inheritance#table-per-type-configuration>

TPT is defined by inheritance _and_ dedicated table name:

```cs
[Table("People")]
public class Person

[Table("Students")]
public class Student : Person
```

or 

```cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Person>().ToTable("People");
    modelBuilder.Entity<Student>().ToTable("Students");
}
```

If you try this in EF3, you'll geht this error: "The entity type 'Student' cannot be mapped to a table because it is derived from 'Person'. Only base entity types can be mapped to a table."

### TPC: Table-per-concrete-type (TPC) 

TPC (Table-per-concrete-type) is supported by EF6, but is not yet supported by EF Core. (28.02.2021)

## Knowledge Base
  
### Remove a property from model, but keep it in database "Shadow Property"
  
```cs
// OLD/BEFORE: normal property definition
entity.Property(x => x.Status).HasColumnName("status").IsFixedLength(true).HasMaxLength(1).IsUnicode(false);
```

Remove the property "status" from the model and change the last line to this:
  
```cs
// NEW/AFTER: The next line will keep the status field in the database. To remove the field from the database, just delete this line and create a new migration
entity.Property(typeof(string), "status").IsFixedLength(true).HasMaxLength(1).IsUnicode(false).IsRequired(false);
```

More details see "Configuring shadow properties": <https://docs.microsoft.com/en-us/ef/core/modeling/shadow-properties#configuring-shadow-properties>

### Performance: use 'explicit loading' to relax main query

Try this solution: Relax the main query and load complex data separately.

```cs
MyItem myItem = Context.MyItems
    .Include(x => x.Table0)
    .Include(x => x.Table1)
  //.Include(x => x.Table3).ThenInclude(y => y.SubTable3)
  //.Include(x => x.Table4).ThenInclude(y => y.SubTable4a)
  //.Include(x => x.Table4).ThenInclude(y => y.SubTable4b)
    .FirstOrDefault(x => x.ID == id);

// nested explicit loading: https://stackoverflow.com/questions/49968247/explicit-loading-nested-related-models-in-entity-framework
Context.Entry(myItem).Collection(x => x.Table3).Query().Include(x => x.SubTable3).Load();
Context.Entry(myItem).Collection(x => x.Table4).Query().Include(x => x.SubTable4a).Include(x => x.SubTable4b).Load();

return myItem;
```

## Add EF Core Logging 
  
Add the following to appsettings.json, section Logging, Loglevel:

```json
"Microsoft.EntityFrameworkCore.Database.Command": "Information"
```

## Update from .NET Classic
  
### beware of queries using Any()

```
return x => ids.Contains(x.MyId.Value); //  new version (fix)

return x => ids.Any(y => y == x.MyId); // Orignal code, gave this error:
```

Error message:

```
The LINQ expression 'y => (int?)y == EntityShaperExpression:
MyApp.Data.Model.KeyAccount
ValueBufferExpression:
ProjectionBindingExpression: EmptyProjectionMember
IsNullable: False
.MyId' could not be translated.
```

## Links

- Bulk Extension <https://github.com/borisdj/EFCore.BulkExtensions>
- Recommended extensions: <https://docs.microsoft.com/en-us/ef/core/extensions/>
- Be careful using ToLower: <https://docs.microsoft.com/en-us/ef/core/miscellaneous/collations-and-case-sensitivity>
- Migrations
  - Handling EF Core migrations in a team <https://jkdev.me/handling-ef-core-migrations/>
  - Deploy Migrations: <http://www.leerichardson.com/2023/03/how-to-deploy-ef-database-migrations.html>
