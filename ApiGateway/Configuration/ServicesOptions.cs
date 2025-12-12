namespace ApiGateway.Configuration
{
    /// <summary>
    /// Содержит адреса внутренних микросервисов, с которыми работает шлюз
    /// </summary>
    public class ServicesOptions
    {
        /// <summary>
        /// Базовый адрес сервиса хранения файлов
        /// </summary>
        public string FileStoringBaseUrl { get; init; } = "http://localhost:5001/";

        /// <summary>
        /// Базовый адрес сервиса анализа работ
        /// </summary>
        public string FileAnalysisBaseUrl { get; init; } = "http://localhost:5002/";
    }
}