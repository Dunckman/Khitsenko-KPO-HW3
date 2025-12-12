namespace FileAnalysisService.Services.Analysis
{
    /// <summary>
    /// Отвечает за поиск признаков плагиата среди работ по одному заданию
    /// </summary>
    public interface IPlagiarismDetector
    {
        /// <summary>
        /// Анализирует текст новой работы относительно уже сданных по тому же заданию
        /// </summary>
        /// <param name="currentSubmissionId">Идентификатор текущей сдачи</param>
        /// <param name="currentSubmittedAt">Момент сдачи текущей работы</param>
        /// <param name="currentText">Текст текущей работы</param>
        /// <param name="otherSubmissions">Сдачи по этому же заданию, полученные от сервиса хранения</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Найденные совпадения и общий процент схожести</returns>
        Task<PlagiarismDetectionResult> DetectAsync(
            Guid currentSubmissionId,
            DateTime currentSubmittedAt,
            string currentText,
            IReadOnlyCollection<AntiplagiatSystem.Shared.DTOs.WorkSubmissionDto> otherSubmissions,
            CancellationToken cancellationToken);
    }
}