using AntiplagiatSystem.Shared.DTOs;

namespace AntiplagiatSystem.Shared.Contracts.Responses
{
    /// <summary>
    /// Сводка по всем отчётам, относящимся к одному заданию
    /// </summary>
    public class WorkReportsResponse
    {
        /// <summary>
        /// Идентификатор задания, для которого запрашивались отчёты
        /// </summary>
        public int WorkId { get; set; } = 0;

        /// <summary>
        /// Человеко-читаемое название задания, если оно было указано
        /// </summary>
        public string WorkTitle { get; set; } = string.Empty;

        /// <summary>
        /// Набор отчётов по всем найденным сдачам этого задания
        /// </summary>
        public IReadOnlyCollection<ReportDto> Reports { get; set; } = Array.Empty<ReportDto>();
    }
}