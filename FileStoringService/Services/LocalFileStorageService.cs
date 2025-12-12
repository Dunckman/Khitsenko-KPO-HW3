using System.Security.Cryptography;
using System.Text;
using AntiplagiatSystem.Shared.Contracts.Requests;
using AntiplagiatSystem.Shared.Contracts.Responses;
using AntiplagiatSystem.Shared.DTOs;
using FileStoringService.Configuration;
using FileStoringService.Entities;
using FileStoringService.Repositories;
using Microsoft.Extensions.Options;

namespace FileStoringService.Services
{
    /// <summary>
    /// Реализация сервиса хранения файлов на локальном диске
    /// </summary>
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IFileRepository _fileRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly FileStorageOptions _options;

        /// <summary>
        /// Создаёт новый экземпляр сервиса локального хранения файлов
        /// </summary>
        /// <param name="fileRepository">Репозиторий доступа к файлам в базе</param>
        /// <param name="submissionRepository">Репозиторий доступа к сдачам работ</param>
        /// <param name="options">Параметры конфигурации файлового хранилища</param>
        public LocalFileStorageService(
            IFileRepository fileRepository,
            ISubmissionRepository submissionRepository,
            IOptions<FileStorageOptions> options)
        {
            _fileRepository = fileRepository;
            _submissionRepository = submissionRepository;
            _options = options.Value;
        }

        /// <inheritdoc />
        public async Task<FileUploadResponse> UploadAsync(
            IFormFile file,
            UploadFileRequest request,
            CancellationToken cancellationToken)
        {
            ValidateFile(file);   // проверяем базовые ограничения по формату и размеру

            var rootPath = EnsureRootDirectoryExists();   // убеждаемся, что корневая папка создана

            var fileHash = await ComputeHashAsync(file, cancellationToken);   // считаем хеш содержимого

            var existingFile = await _fileRepository.GetByHashAsync(fileHash, cancellationToken);   // ищем совпадающий файл

            StoredFile storedFile;

            if (existingFile != null)
            {
                storedFile = existingFile;   // повторно используем уже загруженный файл
            }
            else
            {
                storedFile = await SaveNewFileAsync(file, fileHash, rootPath, cancellationToken);   // сохраняем новый файл
            }

            var submission = new WorkSubmission
            {
                Id = Guid.NewGuid(),
                StudentName = request.Student.StudentName,
                StudentGroup = request.Student.StudentGroup,
                WorkId = request.WorkId,
                WorkTitle = request.WorkTitle,
                FileId = storedFile.Id,
                SubmittedAt = DateTime.UtcNow
            };

            await _submissionRepository.AddAsync(submission, cancellationToken);   // фиксируем сдачу в базе

            var submissionDto = new WorkSubmissionDto
            {
                SubmissionId = submission.Id,
                WorkId = submission.WorkId,
                WorkTitle = submission.WorkTitle,
                Student = new StudentInfoDto
                {
                    StudentName = submission.StudentName,
                    StudentGroup = submission.StudentGroup
                },
                FileId = storedFile.Id,
                SubmittedAt = submission.SubmittedAt
            };

            var response = new FileUploadResponse
            {
                Submission = submissionDto
            };

            return response;
        }

        /// <inheritdoc />
        public async Task<(Stream Stream, string FileName, string ContentType)> GetFileStreamAsync(
            Guid fileId,
            CancellationToken cancellationToken)
        {
            var storedFile = await _fileRepository. GetByIdAsync(fileId, cancellationToken);

            if (storedFile == null)
            {
                throw new FileNotFoundException("Файл с указанным идентификатором не найден");
            }

            var fullPath = Path.Combine(storedFile.FilePath, storedFile.StoredFileName);

            try
            {
                var stream = File.OpenRead(fullPath);
                return (stream, storedFile.OriginalFileName, storedFile.ContentType);
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"Физический файл отсутствует в хранилище: {fullPath}", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new InvalidOperationException($"Отсутствует доступ к файлу: {fullPath}", ex);
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException($"Ошибка чтения файла {fullPath}:  файл может быть заблокирован", ex);
            }
        }

        private void ValidateFile(IFormFile file)
        {
            if (file.Length <= 0)
            {
                throw new InvalidOperationException("Передан пустой файл");
            }

            if (file.Length > _options.MaxFileSizeBytes)
            {
                throw new InvalidOperationException("Размер файла превышает допустимое ограничение");
            }

            var extension = Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new InvalidOperationException("У файла отсутствует расширение");
            }

            var trimmedExtension = extension.TrimStart('.').ToLowerInvariant();

            if (!_options.AllowedExtensions.Contains(trimmedExtension))
            {
                throw new InvalidOperationException("Файл с таким расширением не поддерживается");
            }
        }

        private string EnsureRootDirectoryExists()
        {
            var rootPath = _options.RootPath;

            if (!Path.IsPathRooted(rootPath))
            {
                var baseDir = AppContext.BaseDirectory;   // берём корневую папку приложения
                rootPath = Path.Combine(baseDir, rootPath);
            }

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);   // создаём папку при первом обращении
            }

            return rootPath;
        }

        private async Task<string> ComputeHashAsync(IFormFile file, CancellationToken cancellationToken)
        {
            using var sha256 = SHA256.Create();
    
            await using var stream = file.OpenReadStream();
    
            var hashBytes = await sha256.ComputeHashAsync(stream, cancellationToken);
    
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        private async Task<StoredFile> SaveNewFileAsync(
            IFormFile file,
            string hash,
            string rootPath,
            CancellationToken cancellationToken)
        {
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            var storedFileName = $"{fileId:N}{extension}";
            var fullPath = Path.Combine(rootPath, storedFileName);

            try
            {
                // сначала пытаемся записать файл
                await using (var targetStream = File.Create(fullPath))
                {
                    await file.CopyToAsync(targetStream, cancellationToken);
                }

                var storedFile = new StoredFile
                {
                    Id = fileId,
                    OriginalFileName = file.FileName,
                    StoredFileName = storedFileName,
                    FilePath = rootPath,
                    ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                    FileSize = file.Length,
                    ContentHash = hash,
                    UploadedAt = DateTime.UtcNow
                };

                try
                {
                    await _fileRepository.AddAsync(storedFile, cancellationToken);
                    return storedFile;
                }
                catch (Exception dbException)
                {
                    // если запись в БД упала — удаляем файл, чтобы не засорять диск
                    try
                    {
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                        }
                    }
                    catch (Exception deleteException)
                    {
                        // если не удалось удалить файл, оборачиваем обе ошибки в AggregateException
                        throw new AggregateException(
                            "Не удалось сохранить данные о файле в базе и удалить физический файл",
                            dbException,
                            deleteException);
                    }
                    
                    // если удаление прошло успешно, пробрасываем только ошибку БД
                    throw new InvalidOperationException(
                        $"Не удалось сохранить данные о файле в базе данных. Физический файл был удалён",
                        dbException);
                }
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException($"Не удалось сохранить файл на диск: {ex.Message}", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new InvalidOperationException($"Отсутствует доступ для записи файла:  {ex.Message}", ex);
            }
        }
    }
}