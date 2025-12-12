using System;

namespace AntiplagiatSystem. Shared.Exceptions
{
    /// <summary>
    /// Исключение, возникающее при попытке обратиться к отсутствующему файлу
    /// </summary>
    public class FileMissingException :  Exception
    {
        /// <summary>
        /// Создаёт исключение с сообщением по умолчанию
        /// </summary>
        public FileMissingException()
            : base("Файл с указанным идентификатором не найден в хранилище")
        {
        }

        /// <summary>
        /// Создаёт исключение с заданным описанием проблемы
        /// </summary>
        /// <param name="message">Текст, поясняющий, что именно пошло не так</param>
        public FileMissingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Создаёт исключение с сообщением и внутренней ошибкой
        /// </summary>
        /// <param name="message">Краткое описание контекста ошибки</param>
        /// <param name="innerException">Исключение, ставшее причиной текущей ошибки</param>
        public FileMissingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}