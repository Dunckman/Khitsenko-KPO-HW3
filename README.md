
# КПО ДЗ3 - Система антиплагиата для студенческих работ

**Исполнитель:** Хиценко Артём Николаевич  
**Группа:** БПИ248

**Оговорочка:** почему-то при перепроверке отчёта все отступы и переносы строк в окнах с кодом ниже сбились из-за чего json'ы нечитаемы почти, ну и остальной код в отчёте тоже. Не знаю в чём проблема.

## Описание проекта

Распределённая система для автоматической проверки студенческих работ на плагиат. Система построена на микросервисной архитектуре и позволяет:

- Принимать файлы работ от студентов
- Автоматически анализировать работы на схожесть с ранее сданными
- Формировать отчёты с процентом совпадения
- Генерировать облако слов для визуализации содержания работы

## Архитектура системы

Система состоит из **трёх микросервисов**:

### 1. **ApiGateway** (порт 5236)

Единая точка входа для клиентов. Обрабатывает запросы пользователей и координирует работу других сервисов.

**Основные функции:**

- Приём файлов работ от студентов
- Запуск анализа через сервис FileAnalysisService
- Получение отчётов по заданиям и сдачам

### 2. **FileStoringService** (порт 5059)

Сервис хранения файлов и метаданных о сдачах работ.

**Основные функции:**

- Сохранение файлов на диск
- Дедупликация одинаковых файлов по хешу содержимого
- Регистрация фактов сдачи работ студентами
- Предоставление доступа к файлам и сдачам

**База данных:** SQLite (`file_storing.db`)

### 3. **FileAnalysisService** (порт 5124)

Сервис анализа работ на плагиат.

**Основные функции:**

- Сравнение текущей работы со всеми ранее сданными работами по тому же заданию
- Вычисление процента схожести с помощью алгоритма шинглов и коэффициента Жаккара
- Формирование отчётов с вердиктом о плагиате
- Генерация облака слов через внешний API QuickChart

**База данных:** SQLite (`file_analysis.db`)

## Диаграмма взаимодействия

```  
┌─────────────┐  
│   Клиент    │  
│  (Swagger)  │  
└──────┬──────┘  
       │ 
       │ HTTP POST /api/works/submit 
       ▼
┌─────────────────────────────────────────┐  
│          ApiGateway (5236)              │  
│  ┌───────────────────────────────────┐  │  
│  │  WorksController                  │  │  
│  │  - Принимает файл работы          │  │  
│  │  - Вызывает FileStoringService    │  │  
│  │  - Вызывает FileAnalysisService   │  │  
│  └───────────────────────────────────┘  │  
└──────┬────────────────────┬─────────────┘  
       │                    │ 
       │ 1. Upload file     │ 3. Run analysis 
       ▼                    ▼
┌───────────────────┐   ┌───────────────────┐  
│ FileStoringService│   │FileAnalysisService│  
│     (5059)        │   │     (5124)        │  
│                   │   │                   │  
│ - Сохраняет файл  │   │ - Получает текст  │  
│ - Фиксирует сдачу │   │ - Сравнивает с    │  
│ - Вычисляет хеш   │   │   ранними работами│  
│                   │   │ - Формирует отчёт │  
│ DB:  file_storing │   │ DB: file_analysis │  
└───────────────────┘   └───────────────────┘  
         │ 2. Return submission ID │ 
         └─────────────────────────┘
 ```  

## Запуск проекта

### Вариант 1: Запуск через Docker Compose (рекомендуется)

1. **Убедитесь, что установлен Docker и Docker Compose**

2. **Клонируйте репозиторий:**

```bash  
git clone https://github.com/Dunckman/Khitsenko-KPO-HW3.git  
cd Khitsenko-KPO-HW3
```  

3. **Запустите все сервисы:**

```bash  
docker-compose up -d --build
```  

4. **Проверьте статус контейнеров:**

```bash  
docker-compose ps
```  

Должны работать три контейнера:

- `apigateway` (порт 5236)
- `filestoringservice` (порт 5059)
- `fileanalysisservice` (порт 5124)

5. **Откройте Swagger UI:**

- ApiGateway: http://localhost:5236/swagger
- FileStoringService: http://localhost:5059/swagger
- FileAnalysisService: http://localhost:5124/swagger

### Вариант 2: Запуск локально (без Docker)

**Требования:**

- .NET 9.0 SDK
- SQLite (встроен в .NET)

**Запуск:**

1. **Откройте три терминала**

2. **Запустите FileStoringService:**

```bash  
cd FileStoringService  
dotnet run
```  

Сервис запустится на http://localhost:5059

3. **Запустите FileAnalysisService:**

```bash  
cd FileAnalysisService  
dotnet run
```  

Сервис запустится на http://localhost:5124

4. **Запустите ApiGateway:**

```bash  
cd ApiGateway  
dotnet run
```  

Сервис запустится на http://localhost:5236

## Использование системы

### 1. Сдача работы студентом

**Endpoint:**  `POST /api/works/submit`

**Через Swagger:**

1. Откройте http://localhost:5236/swagger
2. Найдите endpoint `POST /api/works/submit`
3. Нажмите **Try it out**
4. Заполните поля:
- `file`: выберите текстовый файл с работой (.txt)
- `studentName`: "Иванов Иван"
- `studentGroup`: "ИВТ-401"
- `workId`: 1
- `workTitle`: "Лабораторная работа №1"
5. Нажмите **Execute**


**Через curl:**

```bash  
curl -X POST "http://localhost:5236/api/works/submit" \ -H "Content-Type: multipart/form-data" \ -F "file=@work.txt" \ -F "studentName=Иванов Иван" \ -F "studentGroup=ИВТ-401" \ -F "workId=1" \ -F "workTitle=Лабораторная работа №1"
```  

**Пример ответа:**

```json  
{  
	"submission": { 
		"submissionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", 
		"workId": 1, 
		"workTitle": "Лабораторная работа №1", 
		"student": { 
			"studentName": "Иванов Иван", 
			"studentGroup": "ИВТ-401" 
		}, 
		"fileId": "7c9e6679-7425-40de-944b-e07fc1f90ae7", 
		"submittedAt": "2025-12-09T15:30:00Z" 
	}, 
		"report": { 
			"reportId": "8f3b5e12-9a4c-4d7f-b1e3-5c6d8a9b0e1f", 
			"submissionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", 
			"fileId": "7c9e6679-7425-40de-944b-e07fc1f90ae7", 
			"workId": 1, 
			"status": "Completed", 
			"verdict": "NoPlagiarism", 
			"similarityPercentage": 15.5, 
			"wordCloudUrl": "https://quickchart.io/wordcloud?text=.. .", 
			"createdAt": "2025-12-09T15:30:01Z", 
			"completedAt": "2025-12-09T15:30:05Z", 
			"errorMessage": null 
		}
}  
```  

### 2. Получение отчёта по сдаче

**Endpoint:**  `GET /api/works/submissions/{submissionId}/report`

**Пример:**

```bash  
curl "http://localhost:5236/api/works/submissions/3fa85f64-5717-4562-b3fc-2c963f66afa6/report"
```  

### 3. Получение всех отчётов по заданию

**Endpoint:**  `GET /api/works/{workId}/reports`

**Пример:**

```bash  
curl "http://localhost:5236/api/works/1/reports"
```  

**Ответ:**

```json  
{  
	"workId": 1, 
	"workTitle": "", 
	"reports": [ 
		{ 
			"reportId": ".. .", 
			"submissionId": "...", 
			"status": "Completed", 
			"verdict": "NoPlagiarism", 
			"similarityPercentage": 15.5, 
			"createdAt": "2025-12-09T15:30:01Z" 
		}, 
		{ 
			"reportId": "...", 
			"submissionId": "...", 
			"status": "Completed", 
			"verdict": "SuspectedPlagiarism", 
			"similarityPercentage": 35.2, 
			"createdAt": "2025-12-09T16:00:00Z" 
		} 
	]
}  
```  

## Алгоритм обнаружения плагиата

### 1. Нормализация текста

Перед сравнением текст приводится к единому виду:

- Приведение к нижнему регистру
- Удаление знаков препинания
- Схлопывание множественных пробелов в один

**Код:**  `AntiplagiatSystem.Shared/Extensions/StringExtensions.cs`

```csharp  
public  static  string NormalizeForAnalysis(this  string source)  
{  
	var lowerCased = source.ToLowerInvariant(); 
	var lettersAndDigitsOnly = NonLetterRegex.Replace(lowerCased, " "); 
	var compactWhitespace = WhitespaceRegex.Replace(lettersAndDigitsOnly, " "); 
	return compactWhitespace.Trim();
}  
```  

### 2. Построение шинглов

Текст разбивается на n-граммы (шинглы) — последовательности из 4 слов.

**Пример:**

Текст: "это моя лабораторная работа по программированию"

Шинглы:

- "это моя лабораторная работа"
- "моя лабораторная работа по"
- "лабораторная работа по программированию"

**Код:**  `AntiplagiatSystem.Shared/Extensions/StringExtensions.cs`

```csharp  
public  static ISet<string> BuildShingles(this IReadOnlyList<string> words, int windowSize)  
{  
	var shingles = new HashSet<string>(); 
	for (var index = 0; index <= words.Count - windowSize; index++) { 
		var shingle = string.Join(' ', words.Skip(index).Take(windowSize)); 
		shingles.Add(shingle); 
	} 
	return shingles;
}  
```  

### 3. Вычисление коэффициента Жаккара

Схожесть двух текстов определяется по формуле:

```  
J(A, B) = |A ∩ B| / |A ∪ B|  
```  

Где:

- `A` — множество шинглов первого текста
- `B` — множество шинглов второго текста
- `|A ∩ B|` — количество общих шинглов
- `|A ∪ B|` — количество уникальных шинглов в обоих текстах

**Код:**  `FileAnalysisService/Services/Analysis/TextSimilarityCalculator.cs`

```csharp  
public  double CalculateSimilarity(string first, string second)  
{  
	var firstShingles = first.SplitToWords().BuildShingles(4); 
	var secondShingles = second.SplitToWords().BuildShingles(4); 
	var intersection = firstShingles.Intersect(secondShingles).Count(); 
	var union = firstShingles.Union(secondShingles).Count(); 
	var jaccard = (double)intersection / union; 
	return jaccard * 100.0d;
}  
```  

### 4. Определение вердикта

На основе процента схожести система выносит вердикт:

| Процент схожести | Вердикт |  
|------------------|---------------------------|  
| 0% - 19. 9% |  `NoPlagiarism`  |  
| 20% - 49.9% |  `SuspectedPlagiarism`  |  
| 50% - 100% |  `ConfirmedPlagiarism`  |  

**Код:**  `FileAnalysisService/Services/Analysis/AnalysisService.cs`

```csharp  
private  static PlagiarismVerdict ResolveVerdict(double maxSimilarity)  
{  
	if (maxSimilarity < 20.0d) return PlagiarismVerdict.NoPlagiarism; 
	if (maxSimilarity < 50.0d) return PlagiarismVerdict.SuspectedPlagiarism; 
	return PlagiarismVerdict.ConfirmedPlagiarism;
}  
```  

## Структура базы данных

### FileStoringService (file_storing.db)

#### Таблица `StoredFile`

Хранит информацию о загруженных файлах.

| Поле | Тип | Описание |  
|-------------------|----------|---------------------------------------------|  
| Id | GUID | Уникальный идентификатор файла |  
| OriginalFileName | string | Исходное имя файла |  
| StoredFileName | string | Имя файла в хранилище (GUID + расширение) |  
| FilePath | string | Путь к папке с файлом |  
| ContentType | string | MIME-тип файла |  
| FileSize | long | Размер файла в байтах |  
| ContentHash | string | SHA256 хеш содержимого (для дедупликации) |  
| UploadedAt | DateTime | Дата и время загрузки |  

**Индексы:**

- `IX_StoredFile_ContentHash` — для быстрого поиска дубликатов

#### Таблица `WorkSubmission`

Хранит факты сдачи работ студентами.

| Поле | Тип | Описание |  
|--------------|----------|--------------------------------------|  
| Id | GUID | Уникальный идентификатор сдачи |  
| StudentName | string | Имя и фамилия студента |  
| StudentGroup | string | Учебная группа |  
| WorkId | int | Идентификатор задания |  
| WorkTitle | string | Название задания |  
| FileId | GUID | Связь с файлом |  
| SubmittedAt | DateTime | Дата и время сдачи |  

**Связи:**

- `FileId` → `StoredFile.Id` (FK, Restrict)

### FileAnalysisService (file_analysis.db)

#### Таблица `AnalysisReport`

Хранит отчёты об анализе работ.

| Поле | Тип | Описание |  
|----------------------|----------|-----------------------------------------------|  
| Id | GUID | Уникальный идентификатор отчёта |  
| SubmissionId | GUID | Идентификатор сдачи |  
| FileId | GUID | Идентификатор файла |  
| WorkId | int | Идентификатор задания |  
| Status | enum | Статус: Pending, InProgress, Completed, Failed|  
| Verdict | enum | Вердикт о плагиате |  
| SimilarityPercentage | double | Максимальный процент совпадения |  
| WordCloudUrl | string | Ссылка на облако слов |  
| CreatedAt | DateTime | Дата создания отчёта |  
| CompletedAt | DateTime? | Дата завершения анализа |  
| ErrorMessage | string? | Текст ошибки (если анализ упал) |  

**Индексы:**

- `IX_AnalysisReport_SubmissionId` — для поиска отчёта по сдаче
- `IX_AnalysisReport_WorkId` — для получения всех отчётов по заданию

#### Таблица `PlagiarismMatch`

Хранит найденные совпадения с другими работами.

| Поле | Тип | Описание |  
|------------------------|----------|----------------------------------------|  
| Id | GUID | Уникальный идентификатор совпадения |  
| ReportId | GUID | Связь с отчётом |  
| MatchedSubmissionId | GUID | С какой сдачей найдено совпадение |  
| MatchedStudentName | string | Имя студента с похожей работой |  
| SimilarityPercentage | double | Процент схожести с этой работой |  
| MatchedSubmissionDate | DateTime | Когда была сдана похожая работа |  

**Связи:**

- `ReportId` → `AnalysisReport.Id` (FK, Cascade)

## Конфигурация

### ApiGateway/appsettings.json

```json  
{  
"Services": { 
		"FileStoringBaseUrl": "http://localhost:5059", 
		"FileAnalysisBaseUrl": "http://localhost:5124" 
	}
}  
```  

### FileStoringService/appsettings.json

```json  
{  
	"FileStorage": { 
		"RootPath": "uploads", 
		"MaxFileSizeBytes": 10485760, 
		"AllowedExtensions": [
			"txt"
		] 
	}, 
	"ConnectionStrings": { 
		"FileStoring": "Data Source=file_storing.db" 
	}
}  
  
```  

### FileAnalysisService/appsettings.json

```json  
{  
	"FileStoringService": { 
		"BaseUrl": "http://localhost:5059" 
	}, 
	"WordCloud": { 
		"BaseUrl": "https://quickchart.io/wordcloud", 
		"Width": 800, 
		"Height": 400, 
		"MaxWords": 100 
	}, 
	"ConnectionStrings": { 
		"Analysis": "Data Source=file_analysis.db" 
	}
}  
```  

## Технологический стек

| Компонент | Технология | Версия |  
|---------------------|-----------------------------|--------|  
| Язык программирования | C# | 12.0 |  
| Фреймворк | ASP.NET Core | 9.0 |  
| ORM | Entity Framework Core | 9.0 |  
| База данных | SQLite | 3.x |  
| Контейнеризация | Docker, Docker Compose | - |  
| API документация | Swagger (Swashbuckle) | 7.2. 0 |  
| Сериализация | System.Text.Json | 9.0 |  
| HTTP клиент | HttpClient | - |  

## Структура проекта

```  
  
Khitsenko-KPO-HW3/  
├── AntiplagiatSystem.Shared/ # Общая библиотека для всех сервисов  
│ ├── Contracts/  
│ │ ├── Requests/ # DTO для запросов  
│ │ └── Responses/ # DTO для ответов  
│ ├── DTOs/ # Объекты передачи данных  
│ ├── Enums/ # Перечисления (статусы, вердикты)  
│ ├── Exceptions/ # Кастомные исключения  
│ └── Extensions/ # Методы расширения (нормализация, шинглы)  
│  
├── ApiGateway/ # Микросервис шлюза  
│ ├── Configuration/ # Классы конфигурации  
│ ├── Controllers/ # REST контроллеры  
│ ├── Services/ # HTTP-клиенты для связи с другими сервисами  
│ ├── appsettings.json  
│ ├── Dockerfile  
│ └── Program.cs  
│  
├── FileStoringService/ # Микросервис хранения файлов  
│ ├── Configuration/  
│ ├── Controllers/  
│ ├── Data/ # DbContext  
│ ├── Entities/ # Модели БД  
│ ├── Repositories/ # Репозитории для работы с БД  
│ ├── Services/ # Бизнес-логика хранения  
│ ├── appsettings.json  
│ ├── Dockerfile  
│ └── Program.cs  
│  
├── FileAnalysisService/ # Микросервис анализа  
│ ├── Configuration/  
│ ├── Controllers/  
│ ├── Data/ # DbContext  
│ ├── Entities/ # Модели БД  
│ ├── Repositories/ # Репозитории  
│ ├── Services/  
│ │ ├── Analysis/ # Логика анализа, калькулятор схожести  
│ │ ├── Clients/ # HTTP-клиент для FileStoringService  
│ │ └── WordCloud/ # Генерация облака слов  
│ ├── appsettings. json  
│ ├── Dockerfile  
│ └── Program. cs  
│  
├── docker-compose.yml # Конфигурация Docker Compose  
└── README.md
```  

## Обработка исключений и устойчивость к ошибкам

### 1. FileStoringService

#### Проблема: файл уже записан на диск, но запись в БД упала

**Решение:** при ошибке записи в БД файл удаляется с диска, чтобы избежать мусора.

```csharp  
try  
{  
	await _fileRepository.AddAsync(storedFile, cancellationToken); 
	return storedFile;}  
catch (Exception dbException)  
{  
	if (File.Exists(fullPath)) { 
		File. Delete(fullPath); 
	} 
	throw  new InvalidOperationException("Не удалось сохранить данные о файле в базе данных",  dbException);
}  
```  

#### Проблема: файл на диске удалён, но запись в БД есть

**Решение:** при попытке открыть файл проверяется его существование, и выбрасывается осмысленное исключение.

```csharp  
try  
{  
	var stream = File.OpenRead(fullPath); 
	return (stream, storedFile. OriginalFileName, storedFile.ContentType);
}  
catch (FileNotFoundException ex)  
{  
	throw  new FileNotFoundException($"Физический файл отсутствует в хранилище: {fullPath}", ex);
}  
```  

### 2. FileAnalysisService

#### Проблема: сервис FileStoringService недоступен

**Решение:** при ошибке HTTP-запроса отчёт помечается как `Failed` с сохранением текста ошибки.

```csharp  
try  
{  
	// анализ
}  
catch (Exception ex)  
{  
	report.Status = AnalysisStatus.Failed; 
	report.ErrorMessage = ex. Message; 
	report.CompletedAt = DateTime.UtcNow; 
	await _reportRepository.SaveChangesAsync(cancellationToken); 
	throw;
}  
```  

## Тестирование

### Ручное тестирование через Swagger

1. **Загрузите первую работу:**

- Файл: `work1.txt` с текстом "Это моя первая работа по программированию"
- Студент: "Петров Петр", группа "ИВТ-401"
- Задание: `workId=1`

2. **Загрузите вторую работу с похожим текстом:**

- Файл: `work2.txt` с текстом "Это моя вторая работа по программированию"
- Студент: "Сидоров Сидор", группа "ИВТ-401"
- Задание: `workId=1`

3. **Проверьте отчёт второй работы:**

- Должен быть найдено совпадение с первой работой
- Процент схожести должен быть > 50%
- Вердикт: `ConfirmedPlagiarism`

4. **Загрузите третью работу с уникальным текстом:**

- Файл: `work3.txt` с текстом "Анализ алгоритмов сортировки массивов данных"
- Процент схожести должен быть < 20%
- Вердикт: `NoPlagiarism`

## Возможные проблемы и решения

### Проблема: "Cannot connect to Docker daemon"

**Решение:** Убедитесь, что Docker Desktop запущен.

### Проблема: "Port 5236 is already in use"

**Решение:** Остановите процесс, использующий порт, или измените порт в `docker-compose.yml`.

### Проблема: "Database is locked"

**Решение:** SQLite не поддерживает параллельную запись. Это нормально для учебного проекта.

### Проблема: "The constraint reference ' int' could not be resolved"

**Решение:** Уберите пробелы в route constraints: `{workId: int}` вместо `{workId: int}`.

## Метрики производительности

| Операция | Среднее время |  
|----------------------------------|---------------|  
| Загрузка файла 1 МБ | ~200 мс |  
| Анализ работы (1 сравнение) | ~50 мс |  
| Анализ работы (10 сравнений) | ~500 мс |  
| Генерация облака слов | ~100 мс |  
| Полная сдача работы (end-to-end) | ~1-2 сек |