using FileStoringService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileStoringService.Data
{
    /// <summary>
    /// Контекст базы данных для сервиса хранения файлов и сдач работ
    /// </summary>
    public class FileStoringDbContext : DbContext
    {
        /// <summary>
        /// Создаёт новый экземпляр контекста с указанными параметрами
        /// </summary>
        /// <param name="options">Набор параметров для настройки контекста</param>
        public FileStoringDbContext(DbContextOptions<FileStoringDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Набор файлов, размещённых в хранилище
        /// </summary>
        public DbSet<StoredFile> Files { get; set; } = null!;

        /// <summary>
        /// Набор фактов сдачи работ студентами
        /// </summary>
        public DbSet<WorkSubmission> Submissions { get; set; } = null!;

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);   // оставляем базовую конфигурацию

            modelBuilder.Entity<StoredFile>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.OriginalFileName)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(x => x.StoredFileName)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(x => x.FilePath)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(x => x.ContentType)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(x => x.ContentHash)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasIndex(x => x.ContentHash)
                    .HasDatabaseName("IX_StoredFile_ContentHash");
            });

            modelBuilder.Entity<WorkSubmission>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.StudentName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(x => x.StudentGroup)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(x => x.WorkTitle)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.HasOne(x => x.File)
                    .WithMany(f => f.Submissions)
                    .HasForeignKey(x => x.FileId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}