using CommentsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommentsService.Database
{
    public class DataContext : DbContext
    {
        //only here for developing purposes. To be removed.
        //database docker string: docker run -e POSTGRES_DB=commentspostgres -e POSTGRES_PASSWORD=unpickablePassword4 -d -p 32768:5432 postgres:latest
        //to update database, into package manager terminal ->
        //Add-Migration InitialDatabase
        //Update-Database
        //https://ghidyon.hashnode.dev/creating-worker-service-entity-framework-core-in-net
        public DataContext(DbContextOptions<DataContext> options) : base(options) {
            Database.EnsureCreated();
        }
        public DataContext()
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }
        public DbSet<Comment> Comments
        {
            get;
            set;
        } 
    }
}
