using ArchiveEnterprise.Models;
using System.Collections.Generic;
using System.Linq;

namespace ArchiveEnterprise.Services
{
    internal class SubscriberService
    {
        private List<Subscriber> subscribers;
        private List<Document> documents;

        public SubscriberService(List<Subscriber> subscribers, List<Document> documents)
        {
            this.subscribers = subscribers;
            this.documents = documents;
        }

        // Выдача документа абоненту
        public bool IssueDocument(string documentTitle, Subscriber subscriber)
        {
            var doc = documents.FirstOrDefault(d => d.Title.Equals(documentTitle, System.StringComparison.OrdinalIgnoreCase));
            if (doc == null)
                return false;

            if (doc.IssueDays.Count >= 4)
                return false;

            doc.IssueDays.Add(subscriber.DayOfIssue);
            subscriber.DocumentTitle = doc.Title;
            subscribers.Add(subscriber);
            return true;
        }

        // Поиск абонентов, получивших документы по теме
        public List<Subscriber> FindSubscribersByTopic(string topic)
        {
            var docTitles = documents
                .Where(d => d.Topic.Equals(topic, System.StringComparison.OrdinalIgnoreCase))
                .Select(d => d.Title)
                .ToList();

            return subscribers
                .Where(s => docTitles.Contains(s.DocumentTitle))
                .ToList();
        }

        // Последний абонент, который брал указанный документ
        public Subscriber GetLastSubscriberOfDocument(string documentTitle)
        {
            return subscribers
                .Where(s => s.DocumentTitle.Equals(documentTitle, System.StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(s => s.DayOfIssue)
                .FirstOrDefault();
        }

        public List<Subscriber> GetSubscribers() => subscribers;
    }
}