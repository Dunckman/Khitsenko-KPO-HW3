using FileStoringService.Configuration;
using FileStoringService. Data;
using FileStoringService.Repositories;
using FileStoringService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace FileStoringService
{
    /// <summary>
    /// Точка входа в приложение сервиса хранения файлов
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Запускает веб-приложение сервиса хранения
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication. CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // преобразуем enum в строки при сериализации JSON
                    options.JsonSerializerOptions. Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            builder.Services. AddSwaggerGen(options =>
            {
                options. SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "File Storing Service API",
                        Version = "v1",
                        Description = "Сервис для приёма, хранения и выдачи файлов студенческих работ"
                    });
            });

            var baseDir = AppContext.BaseDirectory;
            var connectionString = builder.Configuration.GetConnectionString("FileStoring");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                // если строка подключения не указана в конфигурации, используем файл в папке приложения
                var dbPath = Path.Combine(baseDir, "file_storing. db");
                connectionString = $"Data Source={dbPath}";
            }

            builder.Services.AddDbContext<FileStoringDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            builder.Services.Configure<FileStorageOptions>(
                builder.Configuration.GetSection("FileStorage"));

            builder.Services.AddScoped<IFileRepository, FileRepository>();
            builder. Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "File Storing Service API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // создаём базу данных и таблицы, если они ещё не существуют
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider. GetRequiredService<FileStoringDbContext>();
                dbContext. Database.EnsureCreated();
            }

            app.Run();
        }
    }
}