using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveEnterprise.Models
{
    internal class Subscriber
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Phone { get; set; }
        public int DayOfIssue { get; set; } // День месяца
        public string DocumentTitle { get; set; } // Название документа
    }
}
