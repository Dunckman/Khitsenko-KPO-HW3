using AntiplagiatSystem.Shared.DTOs;

namespace AntiplagiatSystem. Shared.Contracts.Responses
{
    /// <summary>
    /// Ответ на запрос загрузки файла в сервис хранения
    /// </summary>
    public class FileUploadResponse
    {
        /// <summary>
        /// Информация о зарегистрированной сдаче
        /// </summary>
        public WorkSubmissionDto Submission { get; set; } = new WorkSubmissionDto();
    }
}