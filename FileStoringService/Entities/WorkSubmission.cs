namespace FileStoringService.Entities
{
    /// <summary>
    /// Представляет факт сдачи работы студентом
    /// </summary>
    public class WorkSubmission
    {
        /// <summary>
        /// Уникальный идентификатор сдачи
        /// </summary>
        public Guid Id { get; init; } = Guid.NewGuid();

        /// <summary>
        /// Имя и фамилия студента
        /// </summary>
        public string StudentName { get; init; } = string.Empty;

        /// <summary>
        /// Учебная группа студента
        /// </summary>
        public string StudentGroup { get; init; } = string.Empty;

        /// <summary>
        /// Идентификатор задания, к которому относится работа
        /// </summary>
        public int WorkId { get; init; } = 0;

        /// <summary>
        /// Название задания в удобочитаемой форме
        /// </summary>
        public string WorkTitle { get; init; } = string.Empty;

        /// <summary>
        /// Идентификатор файла, который был сдан
        /// </summary>
        public Guid FileId { get; init; } = Guid. Empty;

        /// <summary>
        /// Дата и время приёма работы системой
        /// </summary>
        public DateTime SubmittedAt { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Навигационное свойство для связи с файлом
        /// </summary>
        public StoredFile File { get; init; } = null!;
    }
}