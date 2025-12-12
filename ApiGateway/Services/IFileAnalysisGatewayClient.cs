using AntiplagiatSystem.Shared.Contracts.Requests;
using AntiplagiatSystem.Shared.Contracts.Responses;

namespace ApiGateway.Services
{
    /// <summary>
    /// Описывает операции шлюза при обращении к сервису анализа работ
    /// </summary>
    public interface IFileAnalysisGatewayClient
    {
        /// <summary>
        /// Запускает синхронный анализ указанной сдачи
        /// </summary>
        /// <param name="request">Параметры анализа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Ответ с отчётом</returns>
        Task<AnalysisReportResponse> RunAnalysisAsync(
            AnalyzeFileRequest request,
            CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает все отчёты по заданию
        /// </summary>
        /// <param name="workId">Идентификатор задания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Сводка отчётов</returns>
        Task<WorkReportsResponse> GetReportsByWorkIdAsync(int workId, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает отчёт по сдаче
        /// </summary>
        /// <param name="submissionId">Идентификатор сдачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Отчёт или null</returns>
        Task<AnalysisReportResponse?> GetReportBySubmissionAsync(Guid submissionId, CancellationToken cancellationToken);
    }
}