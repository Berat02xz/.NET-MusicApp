using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using MusicStore.Repository;
using System;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
       
        // CLOUD
        optionsBuilder.UseSqlServer("Server=tcp:berat-finki.database.windows.net,1433;Initial Catalog=MusicApp;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;");
        
        // Local
        // optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-MusicStore-64eebfaa-bada-4a52-8741-59b98369e778;Trusted_Connection=True;MultipleActiveResultSets=true");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}