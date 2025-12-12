using System;

namespace AntiplagiatSystem. Shared. Contracts. Requests
{
    /// <summary>
    /// Запрос на запуск анализа конкретной сдачи работы
    /// </summary>
    public class AnalyzeFileRequest
    {
        /// <summary>
        /// Идентификатор сдачи, которую нужно проанализировать
        /// </summary>
        public Guid SubmissionId { get; set; } = Guid.Empty;

        /// <summary>
        /// Идентификатор файла в сервисе хранения
        /// </summary>
        public Guid FileId { get; set; } = Guid.Empty;

        /// <summary>
        /// Идентификатор задания, к которому относится работа
        /// </summary>
        public int WorkId { get; set; } = 0;
    }
}