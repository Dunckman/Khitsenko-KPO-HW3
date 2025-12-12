namespace AntiplagiatSystem.Shared.Contracts.Responses
{
    /// <summary>
    /// Краткое описание файла, хранящегося в файловом сервисе
    /// </summary>
    public class FileInfoResponse
    {
        /// <summary>
        /// Внутренний идентификатор файла в сервисе хранения
        /// </summary>
        public Guid FileId { get; set; } = Guid.Empty;

        /// <summary>
        /// Исходное имя файла, с которым студент его загрузил
        /// </summary>
        public string OriginalFileName { get; set; } = string.Empty;

        /// <summary>
        /// Тип содержимого файла, который был определён при загрузке
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long FileSize { get; set; } = 0L;

        /// <summary>
        /// Хеш содержимого файла, который используется для поиска полных совпадений
        /// </summary>
        public string ContentHash { get; set; } = string.Empty;
    }
}