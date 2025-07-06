using Microsoft.EntityFrameworkCore;
using ERPCompanySystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ERPCompanySystem.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ILogger<AppDbContext> _logger;

public AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger) : base(options)
        {
            _logger = logger;
            Database.SetCommandTimeout(30);
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
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
                _logger.LogError(ex, "Database update error occurred");
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
                .HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<LoginAttempt>()
                .Property(la => la.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Configure IpBlocks table
            modelBuilder.Entity<IpBlock>()
                .HasKey(ib => ib.Id);
            modelBuilder.Entity<IpBlock>()
                .Property(ib => ib.BlockedUntil)
                .HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<IpBlock>()
                .Property(ib => ib.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Configure UserRoles relationship
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Employee-Department relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Product-Warehouse relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Warehouse)
                .WithMany(w => w.Products)
                .HasForeignKey(p => p.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PurchaseOrder relationships
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrderDetail>()
                .HasOne(pod => pod.PurchaseOrder)
                .WithMany(po => po.Details)
                .HasForeignKey(pod => pod.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseOrderDetail>()
                .HasOne(pod => pod.Product)
                .WithMany()
                .HasForeignKey(pod => pod.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Payment relationship
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Sale)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
