namespace AntiplagiatSystem.Shared.DTOs
{
    /// <summary>
    /// Представляет ссылку на облако слов, построенное для конкретной работы
    /// </summary>
    public class WordCloudDto
    {
        /// <summary>
        /// Идентификатор отчёта, к которому относится это облако слов
        /// </summary>
        public Guid ReportId { get; set; } = Guid.Empty;

        /// <summary>
        /// Полный URL, по которому можно посмотреть сгенерированное облако слов
        /// </summary>
        public string Url { get; set; } = string.Empty;
    }
}