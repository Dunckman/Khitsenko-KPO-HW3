using AntiplagiatSystem.Shared.DTOs;

namespace AntiplagiatSystem.Shared.Contracts.Responses
{
    /// <summary>
    /// Ответ шлюза на успешную сдачу работы и запуск анализа
    /// </summary>
    public class SubmitWorkResponse
    {
        /// <summary>
        /// Информация о зафиксированной сдаче
        /// </summary>
        public WorkSubmissionDto Submission { get; set; } = new WorkSubmissionDto();

        /// <summary>
        /// Краткая информация об отчёте, который был сформирован при анализе
        /// </summary>
        public ReportDto Report { get; set; } = new ReportDto();
    }
}