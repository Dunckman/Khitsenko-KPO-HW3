namespace FileAnalysisService.Configuration
{
    /// <summary>
    /// Параметры генерации облака слов
    /// </summary>
    public class WordCloudOptions
    {
        /// <summary>
        /// Базовый URL внешнего сервиса построения облака слов
        /// </summary>
        public string BaseUrl { get; set; } = "https://quickchart.io/wordcloud";

        /// <summary>
        /// Ширина изображения в пикселях
        /// </summary>
        public int Width { get; set; } = 800;

        /// <summary>
        /// Высота изображения в пикселях
        /// </summary>
        public int Height { get; set; } = 400;

        /// <summary>
        /// Максимальное количество слов для передачи в облако
        /// </summary>
        public int MaxWords { get; set; } = 100;
    }
}