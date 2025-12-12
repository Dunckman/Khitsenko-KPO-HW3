using FileStoringService.Data;
using FileStoringService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileStoringService.Repositories
{
    /// <summary>
    /// Реализация репозитория для работы с файлами через Entity Framework Core
    /// </summary>
    public class FileRepository : IFileRepository
    {
        private readonly FileStoringDbContext _dbContext;

        /// <summary>
        /// Создаёт экземпляр репозитория файлов
        /// </summary>
        /// <param name="dbContext">Контекст базы данных сервиса хранения</param>
        public FileRepository(FileStoringDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<StoredFile> AddAsync(StoredFile file, CancellationToken cancellationToken)
        {
            await _dbContext.Files.AddAsync(file, cancellationToken);   // добавляем сущность в набор

            await _dbContext.SaveChangesAsync(cancellationToken);   // сохраняем изменения в базе

            return file;
        }

        /// <inheritdoc />
        public async Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var file = await _dbContext.Files
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);   // ищем файл по идентификатору

            return file;
        }

        /// <inheritdoc />
        public async Task<StoredFile?> GetByHashAsync(string hash, CancellationToken cancellationToken)
        {
            var file = await _dbContext.Files
                .FirstOrDefaultAsync(x => x.ContentHash == hash, cancellationToken);   // ищем первый файл с данным хешем

            return file;
        }
    }
}