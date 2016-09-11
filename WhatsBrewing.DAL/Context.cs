using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WhatsBrewing.DAL.Models;

namespace WhatsBrewing.DAL
{
    [DbConfigurationType(typeof(Configuration))]
    public class Context : DbContext
    {
        public Context(string connectionString) : base(connectionString) { }

        public Context() { }

        public DbSet<Beer> Beers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Brewery> Breweries { get; set; }
        public DbSet<TapSession> TapSessions { get; set; }
        public DbSet<Activity> Activities { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            //modelBuilder.Entity<Course>()
            //    .HasMany(c => c.Instructors).WithMany(i => i.Courses)
            //    .Map(t => t.MapLeftKey("CourseID")
            //        .MapRightKey("InstructorID")
            //        .ToTable("CourseInstructor"));

            //modelBuilder.Entity<Department>().MapToStoredProcedures();
        }
    }
}
