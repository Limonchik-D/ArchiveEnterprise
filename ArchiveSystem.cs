using ArchiveEnterprise.Models;
using ArchiveEnterprise.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchiveEnterprise
{
    internal class ArchiveSystem
    {
        private List<ArchiveCell> archive = new List<ArchiveCell>();
        private List<Document> documents = new List<Document>();
        private List<Subscriber> subscribers = new List<Subscriber>();

        private ArchiveService archiveService;
        private SubscriberService subscriberService;

        public ArchiveSystem()
        {
            archiveService = new ArchiveService(archive, documents);
            subscriberService = new SubscriberService(subscribers, documents);
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                PrintHeader("ТЕХНИЧЕСКИЙ АРХИВ ПРЕДПРИЯТИЯ");
                Console.WriteLine(" 1. Добавить документ");
                Console.WriteLine(" 2. Удалить документ");
                Console.WriteLine(" 3. Выдать документ абоненту");
                Console.WriteLine(" 4. В каких ячейках хранятся востребованные документы");
                Console.WriteLine(" 5. Кто получил документы по теме");
                Console.WriteLine(" 6. Самая заполненная ячейка");
                Console.WriteLine(" 7. Последний абонент по документу");
                Console.WriteLine(" 8. Проверка пустых ячеек");
                Console.WriteLine(" 9. Документы не востребованные N дней");
                Console.WriteLine(" 0. Выход");
                PrintLine();
                Console.Write("Выберите пункт: ");

                var choice = Console.ReadLine();

                Console.Clear();
                switch (choice)
                {
                    case "1": AddDocument(); break;
                    case "2": RemoveDocument(); break;
                    case "3": IssueDocument(); break;
                    case "4": FindCellsOfRequestedDocuments(); break;
                    case "5": FindSubscribersByTopic(); break;
                    case "6": FindMostFilledCell(); break;
                    case "7": LastSubscriberOfDocument(); break;
                    case "8": CheckEmptyLocations(); break;
                    case "9": DocumentsNotRequestedForNDays(); break;
                    case "0": return;
                    default:
                        PrintError("Неверный ввод.");
                        break;
                }

                PrintLine();
                Console.WriteLine("Нажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }

        private int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            int value;
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out value) && value >= min && value <= max)
                    return value;
                PrintWarning("Пожалуйста, введите корректное число.");
            }
        }

        private void AddDocument()
        {
            PrintSection("Добавление документа");
            Console.Write("Название документа: ");
            string title = Console.ReadLine();

            Console.Write("Тема: ");
            string topic = Console.ReadLine();

            Console.Write("Инвентарный номер: ");
            string invNumber = Console.ReadLine();

            int quantity = ReadInt("Количество экземпляров: ", 1);
            int day = ReadInt("День поступления (1–31): ", 1, 31);
            int rack = ReadInt("Стеллаж: ", 1);
            int shelf = ReadInt("Полка: ", 1);
            int cell = ReadInt("Ячейка: ", 1);

            var doc = new Document
            {
                Title = title,
                Topic = topic,
                InventoryNumber = invNumber,
                Quantity = quantity,
                DayReceived = day
            };

            archiveService.AddDocument(doc, rack, shelf, cell);

            PrintSuccess("Документ успешно добавлен.");
        }

        private void RemoveDocument()
        {
            PrintSection("Удаление документа");
            Console.Write("Введите код ячейки (например, 1-2-3): ");
            string code = Console.ReadLine();

            if (archiveService.RemoveDocument(code))
                PrintSuccess("Документ успешно удалён.");
            else
                PrintError("Документ не найден.");
        }

        private void IssueDocument()
        {
            PrintSection("Выдача документа абоненту");
            Console.Write("Название документа: ");
            string title = Console.ReadLine();

            var doc = documents.FirstOrDefault(d => d.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (doc == null)
            {
                PrintError("Документ не найден.");
                return;
            }

            if (doc.IssueDays.Count >= 4)
            {
                PrintWarning("Документ уже был выдан 4 раза.");
                return;
            }

            Console.Write("ФИО абонента: ");
            string name = Console.ReadLine();

            Console.Write("Отдел: ");
            string department = Console.ReadLine();

            Console.Write("Телефон: ");
            string phone = Console.ReadLine();

            int day = ReadInt("День получения: ", 1, 31);

            var subscriber = new Subscriber
            {
                FullName = name,
                Department = department,
                Phone = phone,
                DayOfIssue = day
            };

            if (subscriberService.IssueDocument(title, subscriber))
                PrintSuccess("Документ успешно выдан.");
            else
                PrintError("Не удалось выдать документ (возможно, превышен лимит выдач).");
        }

        private void FindCellsOfRequestedDocuments()
        {
            PrintSection("Ячейки с востребованными документами");
            var requestedTitles = subscribers.Select(s => s.DocumentTitle).Distinct();
            var cells = archive.Where(c => c.Document != null && requestedTitles.Contains(c.Document.Title)).ToList();

            if (cells.Count == 0)
            {
                PrintWarning("Нет востребованных документов.");
                return;
            }

            foreach (var cell in cells)
            {
                Console.WriteLine($" ├─ Код ячейки: {cell.CellCode,-8} │ Документ: {cell.Document.Title}");
            }
        }

        private void FindSubscribersByTopic()
        {
            PrintSection("Поиск абонентов по теме");
            Console.Write("Введите тему: ");
            string topic = Console.ReadLine();

            var subs = subscriberService.FindSubscribersByTopic(topic);

            if (subs.Count == 0)
            {
                PrintWarning("Нет абонентов по данной теме.");
                return;
            }

            foreach (var sub in subs)
            {
                Console.WriteLine($" ├─ {sub.FullName} │ {sub.Department} │ {sub.Phone} │ день {sub.DayOfIssue}");
            }
        }

        private void FindMostFilledCell()
        {
            PrintSection("Самая заполненная ячейка");
            var maxCell = archive.Where(c => c.Document != null)
                                 .OrderByDescending(c => c.Document.Quantity)
                                 .FirstOrDefault();
            if (maxCell != null)
                Console.WriteLine($" ├─ {maxCell.CellCode} │ {maxCell.Document.Title} │ {maxCell.Document.Quantity} экз.");
            else
                PrintWarning("Нет заполненных ячеек.");
        }

        private void LastSubscriberOfDocument()
        {
            PrintSection("Последний абонент по документу");
            Console.Write("Введите название документа: ");
            string title = Console.ReadLine();

            var last = subscriberService.GetLastSubscriberOfDocument(title);
            if (last != null)
                Console.WriteLine($" ├─ {last.FullName} │ {last.Department} │ {last.Phone} │ день {last.DayOfIssue}");
            else
                PrintWarning("Документ не выдавался.");
        }

        private void CheckEmptyLocations()
        {
            PrintSection("Проверка пустых ячеек, полок и стеллажей");
            var emptyCells = archive.Where(c => c.IsEmpty).ToList();
            Console.WriteLine($"Пустых ячеек: {emptyCells.Count}");
            foreach (var cell in emptyCells)
            {
                Console.WriteLine($" ├─ {cell.CellCode}");
            }

            var racks = archive.GroupBy(c => c.Rack);
            int emptyRacks = racks.Count(g => g.All(c => c.IsEmpty));
            Console.WriteLine($"Пустых стеллажей: {emptyRacks}");

            var shelves = archive.GroupBy(c => new { c.Rack, c.Shelf });
            int emptyShelves = shelves.Count(g => g.All(c => c.IsEmpty));
            Console.WriteLine($"Пустых полок: {emptyShelves}");
        }

        private void DocumentsNotRequestedForNDays()
        {
            PrintSection("Документы не востребованные N дней");
            int daysAgo = ReadInt("Введите срок (N дней): ", 1);
            int today = 30; // Условно, последний день месяца

            var oldDocs = documents
                .Where(d => d.IssueDays.Count == 0 || d.IssueDays.All(day => today - day >= daysAgo))
                .ToList();

            if (oldDocs.Count == 0)
            {
                PrintWarning("Таких документов нет.");
                return;
            }

            foreach (var doc in oldDocs)
            {
                Console.WriteLine($" ├─ {doc.Title} │ Поступил: день {doc.DayReceived}");
            }
        }

        // ===== Оформление =====

        private void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            PrintLine();
            Console.WriteLine($"│{title.PadLeft((title.Length + 36) / 2).PadRight(36)}│");
            PrintLine();
            Console.ResetColor();
        }

        private void PrintSection(string title)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintLine();
            Console.WriteLine($"│ {title.PadRight(34)}│");
            PrintLine();
            Console.ResetColor();
        }

        private void PrintLine()
        {
            Console.WriteLine(new string('─', 36));
        }

        private void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✔ " + message);
            Console.ResetColor();
        }

        private void PrintWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠ " + message);
            Console.ResetColor();
        }

        private void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ " + message);
            Console.ResetColor();
        }
    }
}