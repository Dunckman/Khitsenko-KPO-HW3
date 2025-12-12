namespace AntiplagiatSystem.Shared.Exceptions
{
    /// <summary>
    /// Используется в тех ситуациях, когда один из внутренних сервисов временно недоступен
    /// </summary>
    public class ServiceUnavailableException : Exception
    {
        /// <summary>
        /// Создаёт исключение с сообщением по умолчанию
        /// </summary>
        public ServiceUnavailableException()
            : base("Один из внутренних сервисов сейчас не отвечает. Попробуйте повторить запрос позже.")
        {
        }

        /// <summary>
        /// Создаёт исключение с переданным сообщением об ошибке
        /// </summary>
        /// <param name="message">Текст, поясняющий причину недоступности сервиса</param>
        public ServiceUnavailableException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Создаёт исключение с сообщением и более подробной внутренней ошибкой
        /// </summary>
        /// <param name="message">Краткое описание проблемы</param>
        /// <param name="innerException">Исключение, вызвавшее эту ошибку</param>
        public ServiceUnavailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}