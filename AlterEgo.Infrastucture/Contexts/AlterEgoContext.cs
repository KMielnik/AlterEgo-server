using AlterEgo.Core.Domains;
using AlterEgo.Core.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AlterEgo.Infrastructure.Contexts
{
    public class AlterEgoContext : DbContext
    {
        private readonly EntityFrameworkSettings _settings;

        public AlterEgoContext(DbContextOptions dbContextOptions, IOptions<EntityFrameworkSettings> settings) : base(dbContextOptions)
        {
            _settings = settings.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (_settings.InMemory)
                optionsBuilder.UseInMemoryDatabase("alterego");
            else
                optionsBuilder.UseSqlServer(_settings.ConnectionString);
        }

        public DbSet<User> Users { get; private set; }
        public DbSet<Image> Images { get; private set; }
        public DbSet<DrivingVideo> DrivingVideos { get; private set; }
        public DbSet<ResultVideo> ResultVideos { get; private set; }
        public DbSet<AnimationTask> AnimationTasks { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasKey(x => x.Login);

            modelBuilder.Entity<Image>()
                .HasKey(x => x.Filename);

            modelBuilder.Entity<DrivingVideo>()
                .HasKey(x => x.Filename);

            modelBuilder.Entity<ResultVideo>()
                .HasKey(x => x.Filename);

            modelBuilder.Entity<AnimationTask>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<AnimationTask>()
                .HasOne(x => x.SourceImage)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AnimationTask>()
                .HasOne(x => x.SourceVideo)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AnimationTask>()
                .HasOne(x => x.ResultAnimation)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
