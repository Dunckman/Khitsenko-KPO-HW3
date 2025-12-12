using AntiplagiatSystem.Shared.Contracts.Requests;
using AntiplagiatSystem.Shared.Contracts.Responses;

namespace FileAnalysisService.Services.Analysis
{
    /// <summary>
    /// Инкапсулирует логику запуска анализа и получения отчётов
    /// </summary>
    public interface IAnalysisService
    {
        /// <summary>
        /// Выполняет анализ указанной сдачи и формирует отчёт
        /// </summary>
        /// <param name="request">Параметры анализа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Сформированный отчёт об анализе</returns>
        Task<AnalysisReportResponse> RunAnalysisAsync(
            AntiplagiatSystem.Shared.Contracts.Requests.AnalyzeFileRequest request,
            CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает отчёт по идентификатору
        /// </summary>
        /// <param name="reportId">Идентификатор отчёта</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Ответ с отчётом, если он найден</returns>
        Task<AnalysisReportResponse?> GetReportByIdAsync(Guid reportId, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает отчёт по идентификатору сдачи
        /// </summary>
        /// <param name="submissionId">Идентификатор сдачи работы</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Ответ с отчётом, если он найден</returns>
        Task<AnalysisReportResponse?> GetReportBySubmissionAsync(Guid submissionId, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает все отчёты по указанному заданию
        /// </summary>
        /// <param name="workId">Идентификатор задания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список отчётов по заданию</returns>
        Task<WorkReportsResponse> GetReportsByWorkIdAsync(int workId, CancellationToken cancellationToken);
    }
}