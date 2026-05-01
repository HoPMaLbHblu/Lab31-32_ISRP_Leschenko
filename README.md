# Внедрение базы данных SQLite и Entity Framework Core в ASP.NET Core

**Студент:** Лещенко Даниил
**Группа:** ИСП-233
**Дата выполнения:** 01.05.2026

---

## Краткое описание работы

 Реализован Code First подход: модель данных описывается на C#, а структура таблиц и начальные данные создаются через миграции. Разработан Web API с контроллером `TasksController`, поддерживающим CRUD операции, поиск, фильтрацию, пагинацию, статистику, а также изменение схемы базы данных (добавление поля `DueDate`). Все запросы к БД выполняются через LINQ с асинхронными методами.

---

## Полезные команды `dotnet ef`

| Команда | Описание |
|---------|----------|
| `dotnet ef migrations add <Name>` | Создать новую миграцию на основе изменений в моделях |
| `dotnet ef database update` | Применить все неприменённые миграции к БД |
| `dotnet ef migrations list` | Показать список миграций и их статус |
| `dotnet ef database update <PreviousMigration>` | Откатить до указанной миграции |
| `dotnet ef migrations remove` | Удалить последнюю миграцию (если не применена) |
| `dotnet ef migrations script` | Сгенерировать SQL-скрипт без применения к БД |
| `dotnet ef database update --verbose` | Применить миграции с подробным выводом SQL |

---

## Список реализованных маршрутов (API)

| Метод | Маршрут | Описание |
|-------|---------|----------|
| GET | `/api/tasks` | Получить все задачи (с фильтрацией по `completed` и `priority`) |
| GET | `/api/tasks/{id}` | Получить задачу по ID |
| POST | `/api/tasks` | Создать новую задачу (принимает `CreateTaskDto`) |
| PUT | `/api/tasks/{id}` | Полностью обновить задачу (принимает `UpdateTaskDto`) |
| PATCH | `/api/tasks/{id}/complete` | Переключить статус выполнения (`IsCompleted`) |
| DELETE | `/api/tasks/{id}` | Удалить задачу по ID |
| GET | `/api/tasks/search?query=&priority=&completed=` | Поиск по тексту (Title/Description), приоритету и статусу |
| GET | `/api/tasks/stats` | Статистика: всего, выполнено, % выполнения, группировка по приоритету, создано за 7 дней |
| GET | `/api/tasks/paged?page=1&pageSize=10` | Пагинация (макс. 50 записей на страницу) |
| GET | `/api/tasks/overdue` | Просроченные задачи (DueDate < сегодня и не выполнены) |
| PATCH | `/api/tasks/complete-all` | Пометить все невыполненные задачи как выполненные (самостоятельное задание) |
| DELETE | `/api/tasks/completed` | Удалить все выполненные задачи (самостоятельное задание) |

---

## Таблица применённых миграций

| Название миграции | Добавленные изменения | Дата применения |
|------------------|----------------------|------------------|
| `InitialCreate` | Создание таблицы `Tasks` со столбцами: `Id`, `Title`, `Description`, `IsCompleted`, `CreatedAt`, `Priority` + начальные данные (3 задачи) | Шаг 4 |
| `AddDueDateToTask` | Добавление столбца `DueDate` (nullable `DateTime`) в таблицу `Tasks` | Шаг 7 |

---

## Сравнительная таблица LINQ vs SQL

| LINQ (C#) | SQL (генерируемый EF Core) |
|-----------|----------------------------|
| `.Where(t => t.IsCompleted == false)` | `WHERE is_completed = 0` |
| `.OrderBy(t => t.CreatedAt)` | `ORDER BY created_at ASC` |
| `.OrderByDescending(t => t.CreatedAt)` | `ORDER BY created_at DESC` |
| `.Take(10)` | `LIMIT 10` |
| `.Skip(20).Take(10)` | `OFFSET 20 LIMIT 10` |
| `.Count()` | `SELECT COUNT(*)` |
| `.Any(t => t.Priority == "High")` | `SELECT EXISTS(... WHERE priority = 'High')` |
| `.Max(t => t.Id)` | `SELECT MAX(id)` |
| `.GroupBy(t => t.Priority).Select(g => new { ... })` | `GROUP BY priority` |
| `.Select(t => t.Title)` | `SELECT title` |
| `.Contains("SQL")` в `Where` | `LIKE '%SQL%'` |

---

## Итоговая сравнительная таблица: Хранение в памяти vs EF Core + SQLite

| Концепция | Хранение в памяти (`static List<T>`) | EF Core + SQLite |
|-----------|--------------------------------------|------------------|
| **Хранение данных** | `static List<T>` в RAM | Файл `.db` на диске |
| **После перезапуска** | Данные пропадают | Данные сохраняются |
| **Поиск по условию** | LINQ to Objects | LINQ to Entities → SQL |
| **Создание структуры** | Не нужно | Миграции (`dotnet ef`) |
| **Начальные данные** | Хардкод в коде | `HasData()` в миграции |
| **Получение данных** | `list.FirstOrDefault(...)` | `await db.Table.FindAsync(id)` |
| **Добавление** | `list.Add(item)` | `db.Table.Add(item)` + `SaveChangesAsync()` |
| **Удаление** | `list.Remove(item)` | `db.Table.Remove(item)` + `SaveChangesAsync()` |
| **Масштабируемость** | Ограничена RAM | Гигабайты данных |
| **Транзакции** | Нет | Встроены в EF Core |

---

## Главные выводы

1. EF Core — это переводчик между C# и SQL. Вы пишете LINQ, он пишет SQL

2. Миграции — это система контроля версий для структуры БД. Так же,как Git для кода

3. Code First удобнее, чем писать SQL вручную: изменил класс →создал миграцию→ база обновлена

4. SaveChangesAsync() — ключевой момент. До него все изменения живут только в памяти.


5. async/await при работе с БД — это не опционально, а стандарт. Блокировать поток на время ожидания БД — плохая практика.