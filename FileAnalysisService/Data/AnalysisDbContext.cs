using FileAnalysisService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileAnalysisService.Data
{
    /// <summary>
    /// Контекст базы данных для сервиса анализа работ
    /// </summary>
    public class AnalysisDbContext :  DbContext
    {
        /// <summary>
        /// Создаёт новый экземпляр контекста с указанными параметрами
        /// </summary>
        /// <param name="options">Набор параметров для настройки контекста</param>
        public AnalysisDbContext(DbContextOptions<AnalysisDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Набор отчётов об анализе работ
        /// </summary>
        public DbSet<AnalysisReport> Reports { get; set; } = null!;

        /// <summary>
        /// Набор совпадений с другими работами
        /// </summary>
        public DbSet<PlagiarismMatch> Matches { get; set; } = null!;

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AnalysisReport>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.WordCloudUrl)
                    . HasMaxLength(2048);

                entity.Property(x => x.ErrorMessage)
                    .HasMaxLength(2048);

                // индекс для быстрого поиска отчёта по сдаче
                entity.HasIndex(x => x.SubmissionId)
                    .HasDatabaseName("IX_AnalysisReport_SubmissionId");

                // индекс для быстрого поиска отчётов по заданию
                entity.HasIndex(x => x.WorkId)
                    .HasDatabaseName("IX_AnalysisReport_WorkId");
            });

            modelBuilder.Entity<PlagiarismMatch>(entity =>
            {
                entity. HasKey(x => x.Id);

                entity.Property(x => x.MatchedStudentName)
                    .IsRequired()
                    .HasMaxLength(256);

                // связь с отчётом
                entity.HasOne(x => x.Report)
                    .WithMany(r => r.Matches)
                    .HasForeignKey(x => x.ReportId)
                    .OnDelete(DeleteBehavior. Cascade);
            });
        }
    }
}