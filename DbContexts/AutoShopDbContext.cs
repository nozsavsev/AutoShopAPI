using AutoShopAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AutoShopAPI.DbContexts
{
    public class AutoShopDbContext : DbContext
    {
        public AutoShopDbContext(DbContextOptions<AutoShopDbContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Company).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Model).IsRequired().HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Navigation(e => e.Users)
                                    .AutoInclude();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Navigation(e => e.Car)
                                    .AutoInclude();

                entity.HasOne(e => e.Car)
                      .WithMany(c => c.Users)
                      .HasForeignKey(e => e.CarId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }

        public override int SaveChanges()
        {
            ApplyTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyTimestamps()
        {
            var utcNow = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    TrySetProperty(entry, "CreatedAt", utcNow);
                    TrySetProperty(entry, "UpdatedAt", utcNow);
                }
                else if (entry.State == EntityState.Modified)
                {
                    TrySetProperty(entry, "UpdatedAt", utcNow);
                }
            }
        }

        private static void TrySetProperty(EntityEntry entry, string propertyName, object value)
        {
            var property = entry.Metadata.FindProperty(propertyName);
            if (property == null)
            {
                return;
            }

            // Only set if the property exists and is not shadow state
            var clrProperty = entry.Entity.GetType().GetProperty(propertyName);
            if (clrProperty == null || !clrProperty.CanWrite)
            {
                return;
            }

            // Avoid overriding values that may already be set explicitly
            var current = entry.Property(propertyName).CurrentValue;
            if (entry.State == EntityState.Added || current == null || Equals(current, default))
            {
                entry.Property(propertyName).CurrentValue = value;
            }
        }
    }
}
