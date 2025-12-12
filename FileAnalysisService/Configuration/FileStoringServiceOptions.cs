namespace FileAnalysisService.Configuration
{
    /// <summary>
    /// Настройки подключения к сервису хранения файлов
    /// </summary>
    public class FileStoringServiceOptions
    {
        /// <summary>
        /// Базовый URL сервиса хранения
        /// </summary>
        public string BaseUrl { get; init; } = "http://localhost:5059";
    }
}