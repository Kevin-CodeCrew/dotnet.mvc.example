using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace test_mvc_webapp.Data
{
    public class MvcWebAppDbContextFactory : IDesignTimeDbContextFactory<MvcWebAppDbContext>
    {
        public MvcWebAppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MvcWebAppDbContext>();
            optionsBuilder.UseSqlite("Data Source=app.db");
            return new MvcWebAppDbContext(optionsBuilder.Options);
        }
    }
}