using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ArchiveEnterprise.Models
{
    internal class ArchiveCell
    {
        public int Id { get; set; }
        public int Rack { get; set; }
        public int Shelf { get; set; }
        public int Cell { get; set; }
        public string CellCode => $"{Rack}-{Shelf}-{Cell}";
        public bool IsEmpty => Document == null;

        public Document Document { get; set; }

        public override string ToString()
        {
            return $"[Код ячейки: {CellCode}] - {(IsEmpty ? "Пусто" : Document.Title)}";
        }
    }
}
