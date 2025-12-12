using AntiplagiatSystem.Shared.Contracts.Requests;
using AntiplagiatSystem.Shared.Contracts.Responses;
using AntiplagiatSystem.Shared.DTOs;

namespace ApiGateway.Services
{
    /// <summary>
    /// Описывает операции, которые шлюз выполняет при обращении к сервису хранения файлов
    /// </summary>
    public interface IFileStoringGatewayClient
    {
        /// <summary>
        /// Загружает файл работы и фиксирует сдачу в сервисе хранения
        /// </summary>
        /// <param name="fileStream">Поток с содержимым файла</param>
        /// <param name="fileName">Имя файла</param>
        /// <param name="contentType">Тип содержимого файла</param>
        /// <param name="request">Метаданные сдачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Ответ с информацией о сдаче</returns>
        Task<FileUploadResponse> UploadWorkAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            SubmitWorkRequest request,
            CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает все сдачи по указанному заданию
        /// </summary>
        /// <param name="workId">Идентификатор задания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список сдач</returns>
        Task<IReadOnlyCollection<WorkSubmissionDto>> GetSubmissionsByWorkIdAsync(
            int workId,
            CancellationToken cancellationToken);
    }
}