using System. Text. Json;
using AntiplagiatSystem.Shared.Contracts. Requests;
using AntiplagiatSystem.Shared.Contracts. Responses;
using AntiplagiatSystem.Shared.DTOs;
using ApiGateway.Configuration;
using Microsoft.Extensions.Options;

namespace ApiGateway.Services
{
    /// <summary>
    /// HTTP клиент шлюза для общения с сервисом анализа работ
    /// </summary>
    public class FileAnalysisGatewayClient :  IFileAnalysisGatewayClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Создаёт экземпляр клиента для сервиса анализа
        /// </summary>
        /// <param name="httpClient">HTTP клиент из DI контейнера</param>
        /// <param name="options">Настройки адресов сервисов</param>
        public FileAnalysisGatewayClient(HttpClient httpClient, IOptions<ServicesOptions> options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.FileAnalysisBaseUrl, UriKind.Absolute);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };
        }

        /// <inheritdoc />
        public async Task<AnalysisReportResponse> RunAnalysisAsync(
            AnalyzeFileRequest request,
            CancellationToken cancellationToken)
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync("api/analysis/run", content, cancellationToken);

            var responseContent = await response.Content. ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                // выводим детальную информацию об ошибке
                throw new InvalidOperationException(
                    $"Сервис анализа вернул ошибку {response.StatusCode}. " +
                    $"Детали:  {responseContent}");
            }

            var result = JsonSerializer. Deserialize<AnalysisReportResponse>(responseContent, _jsonOptions);

            if (result == null)
            {
                throw new InvalidOperationException("Не удалось разобрать ответ сервиса анализа");
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<WorkReportsResponse> GetReportsByWorkIdAsync(int workId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"api/reports/by-work/{workId}", cancellationToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = JsonSerializer. Deserialize<WorkReportsResponse>(content, _jsonOptions);

            if (result == null)
            {
                return new WorkReportsResponse
                {
                    WorkId = workId,
                    WorkTitle = string.Empty,
                    Reports = Array.Empty<ReportDto>()
                };
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<AnalysisReportResponse?> GetReportBySubmissionAsync(
            Guid submissionId,
            CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"api/reports/by-submission/{submissionId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = JsonSerializer.Deserialize<AnalysisReportResponse>(content, _jsonOptions);

            return result;
        }
    }
}