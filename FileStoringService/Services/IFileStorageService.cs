using AntiplagiatSystem.Shared.Contracts.Requests;
using AntiplagiatSystem.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Http;

namespace FileStoringService.Services
{
    /// <summary>
    /// Содержит операции по приёму, сохранению и выдаче файлов
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Принимает файл и сопутствующие данные, сохраняет его и фиксирует сдачу
        /// </summary>
        /// <param name="file">Файл, который отправил студент</param>
        /// <param name="request">Метаданные, описывающие сдачу</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Информация о зафиксированной сдаче</returns>
        Task<FileUploadResponse> UploadAsync(IFormFile file, UploadFileRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает поток с содержимым файла по идентификатору
        /// </summary>
        /// <param name="fileId">Идентификатор файла в системе хранения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Поток для чтения файла и сопутствующую информацию</returns>
        Task<(Stream Stream, string FileName, string ContentType)> GetFileStreamAsync(Guid fileId, CancellationToken cancellationToken);
    }
}