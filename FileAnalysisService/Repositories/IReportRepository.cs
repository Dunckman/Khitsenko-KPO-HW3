using FileAnalysisService.Entities;

namespace FileAnalysisService.Repositories
{
    /// <summary>
    /// Описывает операции доступа к отчётам об анализе работ
    /// </summary>
    public interface IReportRepository
    {
        /// <summary>
        /// Добавляет отчёт в контекст без немедленного сохранения
        /// </summary>
        /// <param name="report">Сущность отчёта</param>
        /// <param name="cancellationToken">Токен отмены</param>
        Task<AnalysisReport> AddAsync(AnalysisReport report, CancellationToken cancellationToken);

        /// <summary>
        /// Сохраняет все накопленные изменения в базе
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        Task SaveChangesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Получает отчёт по идентификатору
        /// </summary>
        Task<AnalysisReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Получает отчёт по идентификатору сдачи
        /// </summary>
        Task<AnalysisReport?> GetBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken);

        /// <summary>
        /// Получает все отчёты по заданию
        /// </summary>
        Task<IReadOnlyCollection<AnalysisReport>> GetByWorkIdAsync(int workId, CancellationToken cancellationToken);
    }
}