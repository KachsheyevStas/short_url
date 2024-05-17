namespace short_url.Helpers;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using short_url.Entities;

public class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Database.EnsureCreated();
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to postgres with connection string from app settings
        //options.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
        var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "MyDb.db" };
        var connectionString = connectionStringBuilder.ToString();
        var connection = new SqliteConnection(connectionString);

        options.UseSqlite(connection);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UrlMapping> UrlMappings { get; set; }
}
