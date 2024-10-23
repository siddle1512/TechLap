using Microsoft.EntityFrameworkCore;
using TechLap.API.Models;

namespace TechLap.API.Data
{
    public class TechLapContext : DbContext
    {
        public TechLapContext()
        {
        }
        public TechLapContext(DbContextOptions<TechLapContext> options) : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Discount> Discount { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseModel>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.UtcNow;
                        break;
                }
            }
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>()
                .HasKey(o => new { o.OrderId, o.ProductId });

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(o => o.Order)
                .HasForeignKey(o => o.OrderId);

            modelBuilder.Entity<Product>()
                .HasMany(o => o.OrderDetails)
                .WithOne(o => o.Product)
                .HasForeignKey(o => o.ProductId);

            modelBuilder.Entity<User>()
                .HasMany(o => o.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Category>()
                .HasMany(o => o.Products)
                .WithOne(o => o.Category)
                .HasForeignKey(o => o.CategoryId);

            modelBuilder.Entity<Discount>()
                .HasMany(o => o.Orders)
                .WithOne(o => o.Discount)
                .HasForeignKey(o => o.DiscountId);

            modelBuilder.Entity<ChatMessage>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(cm => cm.SenderId);

            modelBuilder.Entity<ChatMessage>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(cm => cm.ReceiverId);
        }
    }
}