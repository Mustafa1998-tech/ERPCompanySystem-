using Microsoft.EntityFrameworkCore;
using ERPCompanySystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ERPCompanySystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Configure connection pooling and retry logic
            Database.SetCommandTimeout(30);
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<IpBlock> IpBlocks { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                var logger = this.GetService<ILogger<AppDbContext>>();
                logger?.LogError(ex, "Database update error occurred");
                throw;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Sales)
                .HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Product)
                .WithMany(p => p.Purchases)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Client)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.ClientId);

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Purchases)
                .HasForeignKey(p => p.SupplierId);

            // Configure LoginAttempts table
            modelBuilder.Entity<LoginAttempt>()
                .HasKey(la => la.Id);
            modelBuilder.Entity<LoginAttempt>()
                .Property(la => la.AttemptTime)
                .HasDefaultValueSql("GETDATE()");

            // Configure IpBlocks table
            modelBuilder.Entity<IpBlock>()
                .HasKey(ib => ib.Id);
            modelBuilder.Entity<IpBlock>()
                .Property(ib => ib.BlockedUntil)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
