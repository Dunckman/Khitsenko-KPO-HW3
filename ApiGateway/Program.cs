using ApiGateway.Configuration;
using ApiGateway. Services;
using Microsoft.OpenApi.Models;

namespace ApiGateway
{
    /// <summary>
    /// Точка входа в приложение API шлюза
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Запускает веб-приложение API шлюза
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication. CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // преобразуем enum в строки при сериализации JSON
                    options.JsonSerializerOptions.Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Antiplagiat API Gateway",
                        Version = "v1",
                        Description = "Центральное API для сдачи работ и просмотра отчётов"
                    });
            });

            builder.Services.Configure<ServicesOptions>(
                builder.Configuration.GetSection("Services"));

            // регистрируем HTTP-клиенты для связи с микросервисами
            builder. Services.AddHttpClient<IFileStoringGatewayClient, FileStoringGatewayClient>()
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    // базовый URL задаётся в конструкторе FileStoringGatewayClient
                });

            builder.Services.AddHttpClient<IFileAnalysisGatewayClient, FileAnalysisGatewayClient>()
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    // базовый URL задаётся в конструкторе FileAnalysisGatewayClient
                });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Antiplagiat API Gateway v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}