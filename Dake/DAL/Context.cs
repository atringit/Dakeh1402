using Dake.Models;
using Microsoft.EntityFrameworkCore;

namespace Dake.DAL
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) {}

        public DbSet<Banner> Banner { get; set; }
        public DbSet<BannerImage> BannerImage { get; set; }

        public DbSet<Notice> Notices { get; set; }
        public DbSet<Factor> Factors { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ContactUs> ContactUss { get; set; }
        public DbSet<AboutUs> AboutUss { get; set; }
        public DbSet<Stir> Stirs { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<FactorItem> FactorItems { get; set; }
        public DbSet<Information> Informations { get; set; }
        public DbSet<NoticeImage> NoticeImages { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<VisitNotice> VisitNotices { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<ReportNotice> ReportNotices { get; set; }
        public DbSet<VisitNoticeUser> VisitNoticeUsers { get; set; }
        public DbSet<PaymentRequestAttemp> PaymentRequestAttemps { get; set; }
        public DbSet<StaticPrice> StaticPrices { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<UsersToDiscountCode> UsersToDiscountCodes { get; set; }
        public DbSet<InformationMedia> InformationMedias { get; set; }

        public DbSet<AdminsInCity> AdminsInCities { get; set; }

        public DbSet<AdminsInProvice> AdminsInProvices { get; set; }
        public DbSet<AdminActivity> AdminActivities { get; set; }
        public DbSet<Message> Messages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Banner>()
            //.HasMany(i => i.BannerImage)
            //.WithOne()
            //.IsRequired()
            //.OnDelete(DeleteBehavior.Cascade);

            // modelBuilder.Entity<BannerImage>()
            //.HasOne(i => i.BannerId)
            //.WithOne()
            //.IsRequired()
            //.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notice>()
                .HasOne(i => i.city)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notice>()
                .HasOne(i => i.province)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Notice>()
                .HasOne(i => i.area)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VisitNoticeUser>()
                .HasOne(i => i.notice)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VisitNoticeUser>()
                .HasOne(i => i.user)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserFavorite>()
                .HasOne(i => i.user)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserFavorite>()
                .HasOne(i => i.notice)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ReportNotice>()
                .HasOne(i => i.notice)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ReportNotice>()
                .HasOne(i => i.user)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
                .HasQueryFilter(u => u.isCodeConfirmed);


            base.OnModelCreating(modelBuilder);
        }
    }
}