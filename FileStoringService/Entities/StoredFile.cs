namespace FileStoringService.Entities
{
    /// <summary>
    /// Представляет файл, сохранённый в файловом хранилище
    /// </summary>
    public class StoredFile
    {
        /// <summary>
        /// Уникальный идентификатор файла
        /// </summary>
        public Guid Id { get; init; } = Guid. NewGuid();

        /// <summary>
        /// Исходное имя файла, с которым его загрузил пользователь
        /// </summary>
        public string OriginalFileName { get; init; } = string.Empty;

        /// <summary>
        /// Имя файла в хранилище после переименования
        /// </summary>
        public string StoredFileName { get; init; } = string.Empty;

        /// <summary>
        /// Путь к папке, где хранится файл
        /// </summary>
        public string FilePath { get; init; } = string.Empty;

        /// <summary>
        /// MIME тип содержимого файла
        /// </summary>
        public string ContentType { get; init; } = string.Empty;

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long FileSize { get; init; } = 0L;

        /// <summary>
        /// Хеш содержимого для дедупликации одинаковых файлов
        /// </summary>
        public string ContentHash { get; init; } = string.Empty;

        /// <summary>
        /// Дата и время загрузки файла в систему
        /// </summary>
        public DateTime UploadedAt { get; init; } = DateTime. UtcNow;

        /// <summary>
        /// Навигационное свойство для связи с сдачами, которые ссылаются на этот файл
        /// </summary>
        public ICollection<WorkSubmission> Submissions { get; init; } = new List<WorkSubmission>();
    }
}