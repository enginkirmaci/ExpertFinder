using Common.Database;
using Microsoft.EntityFrameworkCore;

namespace ExpertFinder.Models
{
    public partial class teklifcepteDBContext : ApplicationDbContext<User>
    {
        public teklifcepteDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BoughtHistory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.Campain).WithMany(p => p.BoughtHistory).HasForeignKey(d => d.CampainId);

                entity.HasOne(d => d.User).WithMany(p => p.BoughtHistory).HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Campain>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Parent).WithMany(p => p.ChildCategories).HasForeignKey(d => d.ParentId);
            });

            modelBuilder.Entity<CategoryQuestions>(entity =>
            {
                entity.HasOne(d => d.Category).WithMany(p => p.CategoryQuestions).HasForeignKey(d => d.CategoryId);
            });

            modelBuilder.Entity<Content>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.HasOne(d => d.Province).WithMany(p => p.District).HasForeignKey(d => d.ProvinceId);
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Price).HasColumnType("decimal");

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.Category).WithMany(p => p.Item).HasForeignKey(d => d.CategoryId);

                entity.HasOne(d => d.District).WithMany(p => p.Item).HasForeignKey(d => d.DistrictId);

                entity.HasOne(d => d.Province).WithMany(p => p.Item).HasForeignKey(d => d.ProvinceId);

                entity.HasOne(d => d.User).WithMany(p => p.Item).HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Notifications>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.User).WithMany(p => p.Notifications).HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Offer>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.OfferPrice).HasColumnType("decimal");

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.Item).WithMany(p => p.Offer).HasForeignKey(d => d.ItemId);

                entity.HasOne(d => d.User).WithMany(p => p.Offer).HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<SpentHistory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.User).WithMany(p => p.SpentHistory).HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserCategoryRelation>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.Category).WithMany(p => p.UserCategoryRelation).HasForeignKey(d => d.CategoryId);

                entity.HasOne(d => d.User).WithMany(p => p.UserCategoryRelation).HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserExperienceImages>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.User).WithMany(p => p.UserExperienceImages).HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserRatings>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.User).WithMany(p => p.UserRatings).HasForeignKey(d => d.UserId);

                entity.HasOne(d => d.Item).WithMany(p => p.UserRatings).HasForeignKey(d => d.ItemId);

                entity.HasOne(d => d.WinnerOffer).WithMany(p => p.UserRatings).HasForeignKey(d => d.WinnerOfferId);
            });
        }

        public virtual DbSet<BoughtHistory> BoughtHistory { get; set; }

        public virtual DbSet<Campain> Campain { get; set; }

        public virtual DbSet<Category> Category { get; set; }

        public virtual DbSet<CategoryQuestions> CategoryQuestions { get; set; }

        public virtual DbSet<Content> Content { get; set; }

        public virtual DbSet<District> District { get; set; }

        public virtual DbSet<Item> Item { get; set; }

        public virtual DbSet<Notifications> Notifications { get; set; }

        public virtual DbSet<Offer> Offer { get; set; }

        public virtual DbSet<Province> Province { get; set; }

        public virtual DbSet<SpentHistory> SpentHistory { get; set; }

        public virtual DbSet<UserCategoryRelation> UserCategoryRelation { get; set; }

        public virtual DbSet<UserExperienceImages> UserExperienceImages { get; set; }

        public virtual DbSet<UserRatings> UserRatings { get; set; }
    }
}