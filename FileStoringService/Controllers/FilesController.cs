using AntiplagiatSystem.Shared.Contracts.Requests;
using AntiplagiatSystem.Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using FileStoringService.Services;

namespace FileStoringService.Controllers
{
    /// <summary>
    /// Позволяет загружать работы студентов и получать сохранённые файлы
    /// </summary>
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        /// <summary>
        /// Создаёт экземпляр контроллера для работы с файлами
        /// </summary>
        /// <param name="fileStorageService">Сервис работы с файловым хранилищем</param>
        public FilesController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Принимает файл с работой и регистрирует новую сдачу
        /// </summary>
        /// <param name="file">Файл работы в формате multipart/form-data</param>
        /// <param name="studentName">Имя и фамилия студента</param>
        /// <param name="studentGroup">Учебная группа студента</param>
        /// <param name="workId">Идентификатор задания</param>
        /// <param name="workTitle">Человеко-читаемое название задания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Информация о зафиксированной сдаче</returns>
        [HttpPost("upload")]
        [ProducesResponseType(typeof(FileUploadResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadAsync(
            IFormFile file,
            [FromForm] string studentName,
            [FromForm] string studentGroup,
            [FromForm] int workId,
            [FromForm] string workTitle,
            CancellationToken cancellationToken)
        {
            var request = new UploadFileRequest
            {
                Student = new AntiplagiatSystem.Shared.DTOs.StudentInfoDto
                {
                    StudentName = studentName,
                    StudentGroup = studentGroup
                },
                WorkId = workId,
                WorkTitle = workTitle
            };

            var response = await _fileStorageService.UploadAsync(file, request, cancellationToken);

            return Ok(response);
        }

        /// <summary>
        /// Возвращает сохранённый файл по его идентификатору
        /// </summary>
        /// <param name="fileId">Идентификатор файла</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Файл с корректным типом содержимого</returns>
        [HttpGet("{fileId:guid}")]
        public async Task<IActionResult> DownloadAsync(Guid fileId, CancellationToken cancellationToken)
        {
            var (stream, fileName, contentType) = await _fileStorageService.GetFileStreamAsync(fileId, cancellationToken);   // получаем поток файла из хранилища

            return File(stream, contentType, fileName);
        }
    }
}