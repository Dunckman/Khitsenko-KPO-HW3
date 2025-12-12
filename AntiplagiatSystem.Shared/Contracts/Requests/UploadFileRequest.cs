using AntiplagiatSystem.Shared.DTOs;

namespace AntiplagiatSystem.Shared.Contracts.Requests
{
    /// <summary>
    /// Описывает данные, которые сопровождают файл при его загрузке в систему
    /// </summary>
    public class UploadFileRequest
    {
        /// <summary>
        /// Сведения о студенте, который отправляет работу
        /// </summary>
        public StudentInfoDto Student { get; set; } = new StudentInfoDto();

        /// <summary>
        /// Идентификатор задания, к которому относится загружаемая работа
        /// </summary>
        public int WorkId { get; set; } = 0;

        /// <summary>
        /// Название задания так, как его привыкли называть преподаватели и студенты
        /// </summary>
        public string WorkTitle { get; set; } = string.Empty;
    }
}