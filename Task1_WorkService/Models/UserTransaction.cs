using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1_WorkService.Models {
    internal class UserTransaction {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Payment { get; set; } = decimal.Zero;
        public string Date { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
    }
}
