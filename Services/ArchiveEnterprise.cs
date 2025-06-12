using ArchiveEnterprise.Models;
using System.Collections.Generic;
using System.Linq;

namespace ArchiveEnterprise.Services
{
    internal class ArchiveService
    {
        private List<ArchiveCell> archive;
        private List<Document> documents;

        public ArchiveService(List<ArchiveCell> archive, List<Document> documents)
        {
            this.archive = archive;
            this.documents = documents;
        }

        public void AddDocument(Document doc, int rack, int shelf, int cell)
        {
            string code = $"{rack}-{shelf}-{cell}";
            doc.CellCode = code;
            documents.Add(doc);

            var archiveCell = archive.FirstOrDefault(c => c.CellCode == code);
            if (archiveCell == null)
            {
                archiveCell = new ArchiveCell
                {
                    Rack = rack,
                    Shelf = shelf,
                    Cell = cell,
                    Document = doc
                };
                archive.Add(archiveCell);
            }
            else
            {
                archiveCell.Document = doc;
            }
        }

        public bool RemoveDocument(string code)
        {
            var doc = documents.FirstOrDefault(d => d.CellCode == code);
            if (doc != null)
            {
                documents.Remove(doc);
                var cell = archive.FirstOrDefault(c => c.CellCode == code);
                if (cell != null)
                {
                    cell.Document = null;
                }
                return true;
            }
            return false;
        }

        public List<ArchiveCell> GetArchiveCells() => archive;
        public List<Document> GetDocuments() => documents;
    }
}