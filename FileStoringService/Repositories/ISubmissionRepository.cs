using FileStoringService. Entities;

namespace FileStoringService.Repositories
{
    /// <summary>
    /// Описывает операции доступа к фактам сдачи работ
    /// </summary>
    public interface ISubmissionRepository
    {
        /// <summary>
        /// Добавляет новую запись о сдаче и сохраняет изменения
        /// </summary>
        /// <param name="submission">Сущность сдачи, подготовленная к сохранению</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Сущность сдачи после сохранения</returns>
        Task<WorkSubmission> AddAsync(WorkSubmission submission, CancellationToken cancellationToken);

        /// <summary>
        /// Получает сдачу по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сдачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Найденная сдача или null</returns>
        Task<WorkSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает все сдачи по указанному заданию
        /// </summary>
        /// <param name="workId">Идентификатор задания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Коллекция сдач по нужному заданию</returns>
        Task<IReadOnlyCollection<WorkSubmission>> GetByWorkIdAsync(int workId, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает сдачи по заданию, которые были сделаны до указанной даты
        /// </summary>
        /// <param name="workId">Идентификатор задания</param>
        /// <param name="beforeDate">Граничная дата, не включая её</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Коллекция сдач, сделанных раньше указанного времени</returns>
        Task<IReadOnlyCollection<WorkSubmission>> GetByWorkIdBeforeDateAsync(
            int workId,
            DateTime beforeDate,
            CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает все сохранённые сдачи
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Полный список сдач с данными о файлах</returns>
        Task<IReadOnlyCollection<WorkSubmission>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает все сдачи, связанные с указанным файлом
        /// </summary>
        /// <param name="fileId">Идентификатор файла</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Коллекция сдач для данного файла</returns>
        Task<IReadOnlyCollection<WorkSubmission>> GetByFileIdAsync(Guid fileId, CancellationToken cancellationToken);
    }
}