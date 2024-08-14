using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicStore.Domain.Models;
using MusicStore.Domain.Identity;
using MusicStore.Domain.JunctionTables;

namespace MusicStore.Repository
{
    public class ApplicationDbContext : IdentityDbContext<MusicStoreUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Track> Tracks { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<AlbumArtist> AlbumArtist { get; set; }
        public DbSet<ArtistTrack> ArtistTrack { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-MusicStore-64eebfaa-bada-4a52-8741-59b98369e778;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure relationships and entities here
            modelBuilder.Entity<AlbumArtist>()
                 .HasKey(aa => new { aa.AlbumId, aa.ArtistId });

            modelBuilder.Entity<ArtistTrack>()
                .HasKey(at => new { at.ArtistId, at.TrackId });

        }



    }
}
