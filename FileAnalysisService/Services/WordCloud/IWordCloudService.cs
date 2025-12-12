namespace FileAnalysisService.Services.WordCloud
{
    /// <summary>
    /// Отвечает за подготовку и получение ссылок на облака слов для работ
    /// </summary>
    public interface IWordCloudService
    {
        /// <summary>
        /// Формирует ссылку на облако слов для указанного текста
        /// </summary>
        /// <param name="text">Текст работы, по которому строится облако</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>URL на сгенерированное облако слов</returns>
        Task<string> BuildWordCloudUrlAsync(string text, CancellationToken cancellationToken);
    }
}