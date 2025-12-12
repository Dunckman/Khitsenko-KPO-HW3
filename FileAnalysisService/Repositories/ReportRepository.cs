using FileAnalysisService.Data;
using FileAnalysisService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileAnalysisService. Repositories
{
    /// <summary>
    /// Реализация репозитория отчётов через Entity Framework Core
    /// </summary>
    public class ReportRepository : IReportRepository
    {
        private readonly AnalysisDbContext _dbContext;

        /// <summary>
        /// Создаёт экземпляр репозитория отчётов
        /// </summary>
        /// <param name="dbContext">Контекст базы данных анализа</param>
        public ReportRepository(AnalysisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<AnalysisReport> AddAsync(AnalysisReport report, CancellationToken cancellationToken)
        {
            await _dbContext.Reports.AddAsync(report, cancellationToken);

            return report;
        }

        /// <inheritdoc />
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _dbContext. SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<AnalysisReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var report = await _dbContext.Reports
                .Include(x => x.Matches)
                .FirstOrDefaultAsync(x => x. Id == id, cancellationToken);

            return report;
        }

        /// <inheritdoc />
        public async Task<AnalysisReport?> GetBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken)
        {
            var report = await _dbContext.Reports
                .Include(x => x.Matches)
                .FirstOrDefaultAsync(x => x.SubmissionId == submissionId, cancellationToken);

            return report;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<AnalysisReport>> GetByWorkIdAsync(int workId, CancellationToken cancellationToken)
        {
            var reports = await _dbContext.Reports
                . Where(x => x.WorkId == workId)
                .Include(x => x.Matches)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return reports;
        }
    }
}