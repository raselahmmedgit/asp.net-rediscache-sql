using lab.RedisCacheSql.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace lab.RedisCacheSql.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        //private const string connectionString = @"data source=LAPTOP-UFF1V1E7\\SQLEXPRESS2016;initial catalog=TestDb;persist security info=True;user id=sa;password=12345;MultipleActiveResultSets=True;";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        public DbSet<PatientProfile> PatientProfile { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*Non mapped database property so column in database table will not create for it*/
            //modelBuilder.Entity<PatientProfile>().Ignore(e => e.Id);

            //modelBuilder.Ignore<MediaDate>();
            /*End*/

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
