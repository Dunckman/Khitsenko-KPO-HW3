using AntiplagiatSystem.Shared.DTOs;

namespace AntiplagiatSystem.Shared. Contracts.Responses
{
    /// <summary>
    /// Ответ, содержащий отчёт об анализе работы
    /// </summary>
    public class AnalysisReportResponse
    {
        /// <summary>
        /// Подробный отчёт об анализе
        /// </summary>
        public ReportDto Report { get; set; } = new ReportDto();
    }
}