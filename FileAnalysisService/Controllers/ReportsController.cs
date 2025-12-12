using AntiplagiatSystem.Shared.Contracts.Responses;
using FileAnalysisService.Services.Analysis;
using Microsoft.AspNetCore.Mvc;

namespace FileAnalysisService.Controllers
{
    /// <summary>
    /// Позволяет получать сохранённые отчёты по анализу
    /// </summary>
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IAnalysisService _analysisService;

        /// <summary>
        /// Создаёт экземпляр контроллера отчётов
        /// </summary>
        /// <param name="analysisService">Сервис, предоставляющий доступ к отчётам</param>
        public ReportsController(IAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        /// <summary>
        /// Возвращает отчёт по его идентификатору
        /// </summary>
        /// <param name="reportId">Идентификатор отчёта</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Отчёт или 404, если он не найден</returns>
        [HttpGet("{reportId:guid}")]
        [ProducesResponseType(typeof(AnalysisReportResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid reportId, CancellationToken cancellationToken)
        {
            var report = await _analysisService. GetReportByIdAsync(reportId, cancellationToken);

            if (report == null)
            {
                return NotFound();
            }

            return Ok(report);
        }

        /// <summary>
        /// Возвращает отчёт по идентификатору сдачи
        /// </summary>
        /// <param name="submissionId">Идентификатор сдачи работы</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Отчёт или 404, если он не найден</returns>
        [HttpGet("by-submission/{submissionId:guid}")]
        [ProducesResponseType(typeof(AnalysisReportResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBySubmissionAsync(Guid submissionId, CancellationToken cancellationToken)
        {
            var report = await _analysisService.GetReportBySubmissionAsync(submissionId, cancellationToken);

            if (report == null)
            {
                return NotFound();
            }

            return Ok(report);
        }

        /// <summary>
        /// Возвращает все отчёты по указанному заданию
        /// </summary>
        /// <param name="workId">Идентификатор задания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список отчётов по заданию</returns>
        [HttpGet("by-work/{workId:int}")]
        [ProducesResponseType(typeof(WorkReportsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByWorkAsync(int workId, CancellationToken cancellationToken)
        {
            var response = await _analysisService.GetReportsByWorkIdAsync(workId, cancellationToken);

            return Ok(response);
        }
    }
}