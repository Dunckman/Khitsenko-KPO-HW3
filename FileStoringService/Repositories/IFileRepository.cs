using FileStoringService.Entities;

namespace FileStoringService.Repositories
{
    /// <summary>
    /// Описывает базовые операции по работе с сущностями файлов в базе
    /// </summary>
    public interface IFileRepository
    {
        /// <summary>
        /// Добавляет новый файл в контекст и сохраняет изменения
        /// </summary>
        /// <param name="file">Сущность файла, подготовленная к сохранению</param>
        /// <param name="cancellationToken">Токен для отмены операции</param>
        /// <returns>Сущность файла после сохранения в базе</returns>
        Task<StoredFile> AddAsync(StoredFile file, CancellationToken cancellationToken);

        /// <summary>
        /// Ищет файл по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Найденный файл или null, если он отсутствует</returns>
        Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Пытается найти файл по хешу содержимого
        /// </summary>
        /// <param name="hash">Строковое представление хеша содержимого</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Найденный файл или null, если совпадений не обнаружено</returns>
        Task<StoredFile?> GetByHashAsync(string hash, CancellationToken cancellationToken);
    }
}