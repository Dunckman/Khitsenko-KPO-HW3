using AntiplagiatSystem.Shared. Enums;

namespace FileAnalysisService.Entities
{
    /// <summary>
    /// Представляет результат анализа одной работы на плагиат
    /// </summary>
    public class AnalysisReport
    {
        /// <summary>
        /// Уникальный идентификатор отчёта
        /// </summary>
        public Guid Id { get; init; } = Guid.NewGuid();

        /// <summary>
        /// Идентификатор сдачи, к которой относится отчёт
        /// </summary>
        public Guid SubmissionId { get; init; } = Guid.Empty;

        /// <summary>
        /// Идентификатор файла, который был проанализирован
        /// </summary>
        public Guid FileId { get; init; } = Guid.Empty;

        /// <summary>
        /// Идентификатор задания, по которому сдана работа
        /// </summary>
        public int WorkId { get; init; } = 0;

        /// <summary>
        /// Статус выполнения анализа
        /// </summary>
        public AnalysisStatus Status { get; set; } = AnalysisStatus. Pending;

        /// <summary>
        /// Итоговый вердикт о наличии плагиата
        /// </summary>
        public PlagiarismVerdict Verdict { get; set; } = PlagiarismVerdict.NoPlagiarism;

        /// <summary>
        /// Максимальный процент совпадения с другими работами
        /// </summary>
        public double SimilarityPercentage { get; set; } = 0.0d;

        /// <summary>
        /// URL сгенерированного облака слов
        /// </summary>
        public string WordCloudUrl { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время создания отчёта
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата и время завершения анализа
        /// </summary>
        public DateTime?  CompletedAt { get; set; }

        /// <summary>
        /// Текст ошибки, если анализ завершился неудачно
        /// </summary>
        public string?  ErrorMessage { get; set; }

        /// <summary>
        /// Набор найденных совпадений с другими работами
        /// </summary>
        public ICollection<PlagiarismMatch> Matches { get; set; } = new List<PlagiarismMatch>();
    }
}