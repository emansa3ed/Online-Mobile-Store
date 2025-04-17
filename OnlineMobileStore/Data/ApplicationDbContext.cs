using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineMobileStore.Models;
using System.Reflection.Emit;

namespace OnlineMobileStore.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User>? User { get; set; }
        public DbSet<Order>? Order { get; set; }
        public DbSet<OrderItem>? OrderItem { get; set; }
        public DbSet<Company>? Company { get; set; }
        public DbSet<Mobile>? Mobile { get; set; }
        public DbSet<Payment>? Payment { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderId, oi.MobileId });

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

    }
}
