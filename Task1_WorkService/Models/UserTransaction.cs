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
        public DateOnly Date { get; set; } = new DateOnly();
        public long AccountNumber { get; set; } = 0;
        public string Service { get; set; } = string.Empty;
        public override string ToString() {
            return $"{{ {FirstName}, {LastName}, '{Address}', {Payment}, {Date}, {AccountNumber}, {Service}}}";
        }
    }
}
