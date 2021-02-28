using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EF3GetStarted
{
    // source: https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        // TPH: https://docs.microsoft.com/en-us/ef/core/modeling/inheritance#table-per-hierarchy-and-discriminator-configuration
        //public DbSet<Animal> Animals { get; set; }
        //public DbSet<Dog> Dogs { get; set; }

        // TPT 1/2: https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-5.0/whatsnew#table-per-type-tpt-mapping
        public DbSet<Person> People { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BloggingEF3_Test01");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TPT 2/2: https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-5.0/whatsnew#table-per-type-tpt-mapping
            modelBuilder.Entity<Person>().ToTable("People");
            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Teacher>().ToTable("Teachers");

            // TPT in EF 3 error: The entity type 'Student' cannot be mapped to a table because it is derived from 'Person'. Only base entity types can be mapped to a table.
        }
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

    // TPH: https://docs.microsoft.com/en-us/ef/core/modeling/inheritance#table-per-hierarchy-and-discriminator-configuration
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Cat : Animal
    {
        public string EducationLevel { get; set; }
    }

    public class Dog : Animal
    {
        public string FavoriteToy { get; set; }
    }

    // TPT: https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-5.0/whatsnew#table-per-type-tpt-mapping

    //[Table("People")]
    public class Person
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }

    //[Table("Students")]
    public class Student : Person
    {
        public DateTime EnrollmentDate { get; set; }
    }

    //[Table("Teachers")]
    public class Teacher : Person
    {
        public DateTime HireDate { get; set; }
    }

}
