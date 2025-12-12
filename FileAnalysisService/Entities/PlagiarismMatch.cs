namespace FileAnalysisService.Entities
{
    /// <summary>
    /// Представляет одно найденное совпадение с другой работой
    /// </summary>
    public class PlagiarismMatch
    {
        /// <summary>
        /// Уникальный идентификатор совпадения
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Идентификатор отчёта, к которому относится совпадение
        /// </summary>
        public Guid ReportId { get; set; } = Guid.Empty;

        /// <summary>
        /// Идентификатор сдачи, с которой обнаружено совпадение
        /// </summary>
        public Guid MatchedSubmissionId { get; set; } = Guid.Empty;

        /// <summary>
        /// Имя студента, с работой которого найдено совпадение
        /// </summary>
        public string MatchedStudentName { get; set; } = string.Empty;

        /// <summary>
        /// Процент схожести с найденной работой
        /// </summary>
        public double SimilarityPercentage { get; set; } = 0.0d;

        /// <summary>
        /// Дата и время сдачи работы, с которой обнаружено совпадение
        /// </summary>
        public DateTime MatchedSubmissionDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Навигационное свойство для связи с отчётом
        /// </summary>
        public AnalysisReport Report { get; set; } = null!;
    }
}