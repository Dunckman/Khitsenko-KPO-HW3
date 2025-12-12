using AntiplagiatSystem. Shared.DTOs;

namespace FileAnalysisService.Services. Clients
{
    /// <summary>
    /// Описывает операции, которые сервис анализа выполняет при обращении к сервису хранения
    /// </summary>
    public interface IFileStoringClient
    {
        /// <summary>
        /// Загружает текстовое содержимое файла по его идентификатору
        /// </summary>
        /// <param name="fileId">Идентификатор файла в сервисе хранения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Строка с текстом файла</returns>
        Task<string> GetFileTextAsync(Guid fileId, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает все сдачи по указанному заданию
        /// </summary>
        /// <param name="workId">Идентификатор задания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Коллекция сдач по заданию</returns>
        Task<IReadOnlyCollection<WorkSubmissionDto>> GetSubmissionsByWorkIdAsync(int workId, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает сдачи по заданию, которые были сделаны до указанной даты
        /// </summary>
        /// <param name="workId">Идентификатор задания</param>
        /// <param name="beforeDate">Граничная дата, не включая её</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Коллекция сдач, сделанных раньше указанного времени</returns>
        Task<IReadOnlyCollection<WorkSubmissionDto>> GetSubmissionsByWorkIdBeforeDateAsync(
            int workId,
            DateTime beforeDate,
            CancellationToken cancellationToken);
    }
}