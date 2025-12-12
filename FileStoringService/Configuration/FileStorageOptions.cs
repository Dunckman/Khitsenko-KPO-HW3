namespace FileStoringService.Configuration
{
    /// <summary>
    /// Параметры настройки файлового хранилища
    /// </summary>
    public class FileStorageOptions
    {
        /// <summary>
        /// Корневая папка для размещения файлов
        /// </summary>
        public string RootPath { get; init; } = "uploads";

        /// <summary>
        /// Максимальный размер загружаемого файла в байтах
        /// </summary>
        public long MaxFileSizeBytes { get; init; } = 10 * 1024 * 1024;

        /// <summary>
        /// Набор разрешённых расширений файлов
        /// </summary>
        public HashSet<string> AllowedExtensions { get; set; } = ["txt", "pdf", "docx"];
    }
}