using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveEnterprise.Models
{
    internal class Document
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Topic { get; set; }
        public string InventoryNumber { get; set; }
        public string CellCode { get; set; }
        public int Quantity { get; set; }
        public int DayReceived { get; set; } // День месяца (1-31)
        public List<int> IssueDays { get; set; } = new List<int>();
    }
}
