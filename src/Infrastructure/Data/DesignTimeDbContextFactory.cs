using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Educar.Backend.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        // Ajuste a connection string conforme seu ambiente
        optionsBuilder.UseNpgsql("Host=localhost;Port=15432;Database=educar;Username=psqladmin;Password=pgadmin");
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
