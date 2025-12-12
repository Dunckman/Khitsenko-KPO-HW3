using System.Net;
using System. Text;
using AntiplagiatSystem.Shared.Extensions;
using Microsoft.Extensions.Options;

namespace FileAnalysisService.Services.WordCloud
{
    /// <summary>
    /// Строит ссылку на облако слов, используя публичный API QuickChart
    /// </summary>
    public class WordCloudService : IWordCloudService
    {
        private readonly HttpClient _httpClient;
        private readonly Configuration.WordCloudOptions _options;

        /// <summary>
        /// Создаёт экземпляр сервиса построения облака слов
        /// </summary>
        /// <param name="httpClient">HTTP клиент для обращений к QuickChart</param>
        /// <param name="options">Настройки API QuickChart</param>
        public WordCloudService(HttpClient httpClient, IOptions<Configuration.WordCloudOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(_options.BaseUrl, UriKind.Absolute);
            }
        }

        /// <inheritdoc />
        public async Task<string> BuildWordCloudUrlAsync(string text, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var words = text.SplitToWords();

            if (words. Count == 0)
            {
                return string.Empty;
            }

            // ограничиваем количество слов, чтобы не перегружать URL
            var limitedWords = words
                .Take(_options.MaxWords)
                .ToArray();

            var joined = string.Join(' ', limitedWords);
            var encodedText = WebUtility.UrlEncode(joined);

            var urlBuilder = new StringBuilder();
            urlBuilder.Append(_options.BaseUrl);
            urlBuilder.Append("?text=");
            urlBuilder.Append(encodedText);

            var finalUrl = urlBuilder.ToString();

            // оставляем метод асинхронным для единообразия сигнатуры
            await Task.CompletedTask;

            return finalUrl;
        }
    }
}