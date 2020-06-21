using Apsiyon.Logger.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace ApsiyonLogger.Data.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Log> Logs { get; set; }
    }
}