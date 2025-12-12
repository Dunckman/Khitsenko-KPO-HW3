using AntiplagiatSystem.Shared.Contracts. Requests;
using AntiplagiatSystem.Shared.Contracts.Responses;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    /// <summary>
    /// Предоставляет внешнее API для сдачи работ и получения отчётов
    /// </summary>
    [ApiController]
    [Route("api/works")]
    public class WorksController : ControllerBase
    {
        private readonly IFileStoringGatewayClient _fileStoringClient;
        private readonly IFileAnalysisGatewayClient _fileAnalysisClient;

        /// <summary>
        /// Создаёт экземпляр контроллера работ
        /// </summary>
        /// <param name="fileStoringClient">Клиент для работы с сервисом хранения</param>
        /// <param name="fileAnalysisClient">Клиент для работы с сервисом анализа</param>
        public WorksController(
            IFileStoringGatewayClient fileStoringClient,
            IFileAnalysisGatewayClient fileAnalysisClient)
        {
            _fileStoringClient = fileStoringClient;
            _fileAnalysisClient = fileAnalysisClient;
        }

        /// <summary>
        /// Принимает файл работы, регистрирует сдачу и запускает анализ
        /// </summary>
        /// <param name="file">Файл работы</param>
        /// <param name="studentName">Имя и фамилия студента</param>
        /// <param name="studentGroup">Группа студента</param>
        /// <param name="workId">Идентификатор задания</param>
        /// <param name="workTitle">Название задания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Информация о сдаче и сформированном отчёте</returns>
        [HttpPost("submit")]
        [ProducesResponseType(typeof(SubmitWorkResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SubmitAsync(
            IFormFile file,
            [FromForm] string studentName,
            [FromForm] string studentGroup,
            [FromForm] int workId,
            [FromForm] string workTitle,
            CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не передан или пуст");
            }

            var submitRequest = new SubmitWorkRequest
            {
                StudentName = studentName,
                StudentGroup = studentGroup,
                WorkId = workId,
                WorkTitle = workTitle
            };

            await using var stream = file.OpenReadStream();

            // сначала сохраняем файл и фиксируем сдачу
            var uploadResponse = await _fileStoringClient.UploadWorkAsync(
                stream,
                file. FileName,
                string.IsNullOrWhiteSpace(file.ContentType) ? "text/plain" : file.ContentType,
                submitRequest,
                cancellationToken);

            var analyzeRequest = new AnalyzeFileRequest
            {
                SubmissionId = uploadResponse. Submission. SubmissionId,
                FileId = uploadResponse.Submission. FileId,
                WorkId = uploadResponse.Submission.WorkId
            };

            // затем запускаем анализ синхронно
            var analysisResponse = await _fileAnalysisClient.RunAnalysisAsync(
                analyzeRequest,
                cancellationToken);

            var result = new SubmitWorkResponse
            {
                Submission = uploadResponse.Submission,
                Report = analysisResponse.Report
            };

            return Ok(result);
        }

        /// <summary>
        /// Возвращает все отчёты по указанному заданию
        /// </summary>
        /// <param name="workId">Идентификатор задания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список отчётов по заданию</returns>
        [HttpGet("{workId:int}/reports")]
        [ProducesResponseType(typeof(WorkReportsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReportsAsync(int workId, CancellationToken cancellationToken)
        {
            var response = await _fileAnalysisClient.GetReportsByWorkIdAsync(workId, cancellationToken);

            return Ok(response);
        }

        /// <summary>
        /// Возвращает отчёт по конкретной сдаче
        /// </summary>
        /// <param name="submissionId">Идентификатор сдачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Отчёт или 404, если он не найден</returns>
        [HttpGet("submissions/{submissionId:guid}/report")]
        [ProducesResponseType(typeof(AnalysisReportResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReportBySubmissionAsync(Guid submissionId, CancellationToken cancellationToken)
        {
            var report = await _fileAnalysisClient.GetReportBySubmissionAsync(submissionId, cancellationToken);

            if (report == null)
            {
                return NotFound();
            }

            return Ok(report);
        }
    }
}