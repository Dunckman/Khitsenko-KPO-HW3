using FileAnalysisService. Configuration;
using FileAnalysisService.Data;
using FileAnalysisService.Repositories;
using FileAnalysisService.Services. Analysis;
using FileAnalysisService.Services.Clients;
using FileAnalysisService.Services.WordCloud;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace FileAnalysisService
{
    /// <summary>
    /// Точка входа в приложение сервиса анализа работ
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Запускает веб-приложение сервиса анализа
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
                options. SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "File Analysis Service API",
                        Version = "v1",
                        Description = "Сервис для анализа работ на плагиат и формирования отчётов"
                    });
            });

            var baseDir = AppContext.BaseDirectory;
            var connectionString = builder.Configuration.GetConnectionString("Analysis");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                // если строка подключения не указана в конфигурации, используем файл в папке приложения
                var dbPath = Path.Combine(baseDir, "file_analysis.db");
                connectionString = $"Data Source={dbPath}";
            }

            builder.Services.AddDbContext<AnalysisDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            builder.Services.Configure<FileStoringServiceOptions>(
                builder.Configuration.GetSection("FileStoringService"));

            builder.Services.Configure<WordCloudOptions>(
                builder.Configuration.GetSection("WordCloud"));

            builder.Services. AddHttpClient<IFileStoringClient, FileStoringClient>();

            // регистрируем сервисы анализа
            builder.Services.AddScoped<ITextSimilarityCalculator, TextSimilarityCalculator>();
            builder.Services. AddScoped<IWordCloudService, WordCloudService>();
            builder.Services.AddScoped<IAnalysisService, AnalysisService>();

            builder.Services.AddScoped<IReportRepository, ReportRepository>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "File Analysis Service API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // создаём базу данных и таблицы, если они ещё не существуют
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AnalysisDbContext>();
                dbContext.Database.EnsureCreated();
            }

            app.Run();
        }
    }
}