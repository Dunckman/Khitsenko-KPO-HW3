namespace AntiplagiatSystem.Shared.DTOs
{
    /// <summary>
    /// Описывает факт сдачи конкретной работы студентом
    /// </summary>
    public class WorkSubmissionDto
    {
        /// <summary>
        /// Идентификатор сдачи работы в системе
        /// </summary>
        public Guid SubmissionId { get; set; } = Guid.Empty;

        /// <summary>
        /// Идентификатор задания, к которому относится работа
        /// </summary>
        public int WorkId { get; set; } = 0;

        /// <summary>
        /// Человеко-читаемое название задания для удобства
        /// </summary>
        public string WorkTitle { get; set; } = string.Empty;

        /// <summary>
        /// Информация о студенте, который загрузил работу
        /// </summary>
        public StudentInfoDto Student { get; set; } = new StudentInfoDto();

        /// <summary>
        /// Идентификатор файла, под которым работа хранится в файловом сервисе
        /// </summary>
        public Guid FileId { get; set; } = Guid.Empty;

        /// <summary>
        /// Дата и время, когда работа была принята системой
        /// </summary>
        public DateTime SubmittedAt { get; set; } = DateTime.MinValue;
    }
}