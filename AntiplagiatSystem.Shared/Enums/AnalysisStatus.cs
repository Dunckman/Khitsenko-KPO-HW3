namespace AntiplagiatSystem.Shared.Enums
{
    /// <summary>
    /// Представляет текущий статус анализа загруженной работы
    /// </summary>
    public enum AnalysisStatus
    {
        /// <summary>
        /// Анализ ещё не запускался и находится в очереди
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Анализ выполняется в данный момент
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Анализ успешно завершён
        /// </summary>
        Completed = 2,

        /// <summary>
        /// При выполнении анализа произошла ошибка
        /// </summary>
        Failed = 3
    }
}