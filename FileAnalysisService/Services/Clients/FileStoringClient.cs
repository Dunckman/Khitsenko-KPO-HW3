using System.Net.Http.Json;
using AntiplagiatSystem.Shared.DTOs;
using FileAnalysisService.Configuration;
using Microsoft.Extensions.Options;

namespace FileAnalysisService.Services.Clients
{
    /// <summary>
    /// HTTP клиент для общения с сервисом хранения файлов
    /// </summary>
    public class FileStoringClient : IFileStoringClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Создаёт новый экземпляр клиента для сервиса хранения
        /// </summary>
        /// <param name="httpClient">Экземпляр HTTP клиента, управляемый DI контейнером</param>
        /// <param name="options">Настройки подключения к сервису хранения</param>
        public FileStoringClient(HttpClient httpClient, IOptions<FileStoringServiceOptions> options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value. BaseUrl, UriKind. Absolute);
        }

        /// <inheritdoc />
        public async Task<string> GetFileTextAsync(Guid fileId, CancellationToken cancellationToken)
        {
            var url = $"api/files/{fileId:D}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (! response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Не удалось получить файл из сервиса хранения");
            }

            var contentType = response.Content.Headers. ContentType?.MediaType ?? string.Empty;

            if (! string. Equals(contentType, "text/plain", StringComparison. OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Сервис анализа в текущей версии умеет работать только с текстовыми файлами");
            }

            var text = await response.Content.ReadAsStringAsync(cancellationToken);

            return text;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<WorkSubmissionDto>> GetSubmissionsByWorkIdAsync(
            int workId,
            CancellationToken cancellationToken)
        {
            var url = $"api/submissions/by-work/{workId}";

            var submissions = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<WorkSubmissionDto>>(url, cancellationToken);

            if (submissions == null)
            {
                return Array.Empty<WorkSubmissionDto>();
            }

            return submissions;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<WorkSubmissionDto>> GetSubmissionsByWorkIdBeforeDateAsync(
            int workId,
            DateTime beforeDate,
            CancellationToken cancellationToken)
        {
            // форматируем дату в ISO 8601
            var formattedDate = beforeDate.ToString("o");

            var url = $"api/submissions/by-work/{workId}/before/{formattedDate}";

            var submissions = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<WorkSubmissionDto>>(url, cancellationToken);

            if (submissions == null)
            {
                return Array. Empty<WorkSubmissionDto>();
            }

            return submissions;
        }
    }
}