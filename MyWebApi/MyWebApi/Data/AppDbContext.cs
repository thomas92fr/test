using Microsoft.EntityFrameworkCore;
using MyWebApi.Models;

namespace MyWebApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Brasserie> Brasseries { get; set; }
        public DbSet<Biere> Bieres { get; set; }
        public DbSet<Grossiste> Grossistes { get; set; }
        public DbSet<GrossisteBiere> GrossisteBieres { get; set; }  // Table de jonction

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Utilisation d'une base de données In-Memory
            optionsBuilder.UseInMemoryDatabase("BiereDB");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relation entre Brasserie et Biere
            modelBuilder.Entity<Biere>()
                .HasOne(b => b.Brasserie)
                .WithMany(br => br.Bieres)
                .HasForeignKey(b => b.BrasserieId); 

            // Relation entre Biere et Grossiste (Relation Many-to-Many)
            modelBuilder.Entity<GrossisteBiere>()
                .HasKey(gb => new { gb.BiereId, gb.GrossisteId });

            modelBuilder.Entity<GrossisteBiere>()
                .HasOne(gb => gb.Biere)
                .WithMany(b => b.GrossisteBieres)
                .HasForeignKey(gb => gb.BiereId);

            modelBuilder.Entity<GrossisteBiere>()
                .HasOne(gb => gb.Grossiste)
                .WithMany(g => g.GrossisteBieres)
                .HasForeignKey(gb => gb.GrossisteId);
        }
    }
}
