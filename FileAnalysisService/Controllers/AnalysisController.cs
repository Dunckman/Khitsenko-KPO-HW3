using AntiplagiatSystem. Shared.Contracts.Requests;
using AntiplagiatSystem.Shared.Contracts. Responses;
using FileAnalysisService.Services.Analysis;
using Microsoft.AspNetCore.Mvc;

namespace FileAnalysisService.Controllers
{
    /// <summary>
    /// Позволяет запускать анализ конкретных работ
    /// </summary>
    [ApiController]
    [Route("api/analysis")]
    public class AnalysisController : ControllerBase
    {
        private readonly IAnalysisService _analysisService;

        /// <summary>
        /// Создаёт экземпляр контроллера анализа
        /// </summary>
        /// <param name="analysisService">Сервис, выполняющий анализ работ</param>
        public AnalysisController(IAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        /// <summary>
        /// Запускает анализ указанной сдачи с синхронным ожиданием результата
        /// </summary>
        /// <param name="request">Параметры анализа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Сформированный отчёт</returns>
        [HttpPost("run")]
        [ProducesResponseType(typeof(AnalysisReportResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RunAsync(
            [FromBody] AnalyzeFileRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _analysisService.RunAnalysisAsync(request, cancellationToken);

            return Ok(response);
        }
    }
}