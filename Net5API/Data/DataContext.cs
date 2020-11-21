using Microsoft.EntityFrameworkCore;
using Net5API.Models;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<Photo> Photo { get; set; }
}
