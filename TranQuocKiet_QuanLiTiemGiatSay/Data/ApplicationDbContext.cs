using Microsoft.EntityFrameworkCore;
using TranQuocKiet_QuanLiTiemGiatSay.Constants;
using TranQuocKiet_QuanLiTiemGiatSay.Models;

namespace TranQuocKiet_QuanLiTiemGiatSay.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.UserId);
                entity.HasIndex(x => x.Username).IsUnique();
                entity.HasIndex(x => x.Phone).IsUnique();

                entity.Property(x => x.FullName).HasMaxLength(200);
                entity.Property(x => x.Username).HasMaxLength(100);
                entity.Property(x => x.Phone).HasMaxLength(20);
                entity.Property(x => x.Role).HasMaxLength(20);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(x => x.CustomerId);
                entity.HasIndex(x => x.Phone);
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(x => x.ServiceId);
                entity.HasIndex(x => x.ServiceName).IsUnique();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(x => x.OrderId);
                entity.HasIndex(x => x.OrderCode).IsUnique();
                entity.HasIndex(x => x.Status);

                entity.Property(x => x.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.HasOne(x => x.Customer)
                      .WithMany(x => x.Orders)
                      .HasForeignKey(x => x.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Receiver)
                      .WithMany()
                      .HasForeignKey(x => x.ReceivedBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(x => x.OrderItemId);

                entity.HasOne(x => x.Order)
                      .WithMany(x => x.OrderItems)
                      .HasForeignKey(x => x.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Service)
                      .WithMany(x => x.OrderItems)
                      .HasForeignKey(x => x.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(x => x.PaymentId);

                entity.HasOne(x => x.Order)
                      .WithMany(x => x.Payments)
                      .HasForeignKey(x => x.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(x => x.Method)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.HasOne(x => x.Receiver)
                      .WithMany()
                      .HasForeignKey(x => x.ReceivedBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.HasKey(x => x.InventoryTxnId);
                entity.HasIndex(x => x.TxnDate);

                entity.HasOne(x => x.Creator)
                      .WithMany()
                      .HasForeignKey(x => x.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            
        }
    }
}