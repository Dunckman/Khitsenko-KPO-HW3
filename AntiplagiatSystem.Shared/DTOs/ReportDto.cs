using AntiplagiatSystem.Shared.Enums;

namespace AntiplagiatSystem.Shared.DTOs
{
    /// <summary>
    /// Сводная информация об отчёте по проверке конкретной сдачи
    /// </summary>
    public class ReportDto
    {
        /// <summary>
        /// Идентификатор отчёта в сервисе анализа
        /// </summary>
        public Guid ReportId { get; set; } = Guid.Empty;

        /// <summary>
        /// Идентификатор сдачи, к которой относится данный отчёт
        /// </summary>
        public Guid SubmissionId { get; set; } = Guid.Empty;

        /// <summary>
        /// Идентификатор файла, использованного при анализе
        /// </summary>
        public Guid FileId { get; set; } = Guid.Empty;

        /// <summary>
        /// Идентификатор задания, для которого была сдана работа
        /// </summary>
        public int WorkId { get; set; } = 0;

        /// <summary>
        /// Текущий статус выполнения анализа
        /// </summary>
        public AnalysisStatus Status { get; set; } = AnalysisStatus.Pending;

        /// <summary>
        /// Итоговый вердикт по наличию плагиата
        /// </summary>
        public PlagiarismVerdict Verdict { get; set; } = PlagiarismVerdict.NoPlagiarism;

        /// <summary>
        /// Оценка степени совпадения с другими работами в процентах
        /// </summary>
        public double SimilarityPercentage { get; set; } = 0.0d;

        /// <summary>
        /// Ссылка на сгенерированное облако слов, если оно было построено
        /// </summary>
        public string WordCloudUrl { get; set; } = string.Empty;

        /// <summary>
        /// Момент времени, когда анализ был создан
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Время окончания анализа, если он уже завершился
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Текстовое описание ошибки, если во время анализа что‑то пошло не так
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}