using BlackGreenApi.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BlackGreenApi.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        /// <summary>
        /// Регистрация контекста моделей в контекст бд
        /// </summary>
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<EcoRating> EcoRatings { get; set; }
        public DbSet<AboutUser> AboutUsers { get; set; }

        /// <summary>
        /// Связи моделей
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<EcoRating>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            builder.Entity<Receipt>()
                .HasMany(r => r.Items)
                .WithOne(i => i.Receipt)
                .HasForeignKey(i => i.ReceiptId);

            builder.Entity<Item>()
                .HasOne(i => i.Recommendation)
                .WithMany()
                .HasForeignKey(i => i.RecommendationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Item>()
                .Property(i => i.Price)
                .HasPrecision(18, 2);

            builder.Entity<Receipt>()
                .Property(r => r.TotalAmount)
                .HasPrecision(18, 2);
        }
    }
}