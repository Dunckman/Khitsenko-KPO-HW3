using System. Net.Http.Headers;
using System.Text.Json;
using AntiplagiatSystem.Shared. Contracts. Requests;
using AntiplagiatSystem.Shared.Contracts.Responses;
using AntiplagiatSystem.Shared. DTOs;
using ApiGateway.Configuration;
using Microsoft.Extensions.Options;

namespace ApiGateway.Services
{
    /// <summary>
    /// HTTP клиент шлюза для общения с сервисом хранения файлов
    /// </summary>
    public class FileStoringGatewayClient : IFileStoringGatewayClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Создаёт экземпляр клиента для сервиса хранения файлов
        /// </summary>
        /// <param name="httpClient">HTTP клиент, управляемый DI контейнером</param>
        /// <param name="options">Настройки адресов сервисов</param>
        public FileStoringGatewayClient(HttpClient httpClient, IOptions<ServicesOptions> options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.FileStoringBaseUrl, UriKind. Absolute);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };
        }

        /// <inheritdoc />
        public async Task<FileUploadResponse> UploadWorkAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            SubmitWorkRequest request,
            CancellationToken cancellationToken)
        {
            using var form = new MultipartFormDataContent();

            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            form.Add(fileContent, "file", fileName);
            form.Add(new StringContent(request.StudentName), "studentName");
            form.Add(new StringContent(request.StudentGroup), "studentGroup");
            form.Add(new StringContent(request.WorkId.ToString()), "workId");
            form.Add(new StringContent(request.WorkTitle), "workTitle");

            using var response = await _httpClient.PostAsync("api/files/upload", form, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Сервис хранения вернул ошибку при загрузке файла");
            }

            var result = await response.Content.ReadFromJsonAsync<FileUploadResponse>(cancellationToken:  cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException("Не удалось разобрать ответ сервиса хранения при загрузке файла");
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<WorkSubmissionDto>> GetSubmissionsByWorkIdAsync(
            int workId,
            CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"api/submissions/by-work/{workId}", cancellationToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = JsonSerializer. Deserialize<IReadOnlyCollection<WorkSubmissionDto>>(content, _jsonOptions);

            if (result == null)
            {
                return Array.Empty<WorkSubmissionDto>();
            }

            return result;
        }
    }
}