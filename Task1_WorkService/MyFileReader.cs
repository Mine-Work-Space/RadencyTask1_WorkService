using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Task1_WorkService.Models;

namespace Task1_WorkService {
    internal static class MyFileReader {
        private const int DefaultBufferSize = 8192; // or 4096
        public static async Task<List<UserTransaction>> ReadAllLinesAsync(string path) {
            var transactions = new List<UserTransaction>();

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize)) {
                using (var reader = new StreamReader(stream, Encoding.UTF8)) {
                    string line;
                    UserTransaction transaction = new UserTransaction();
                    while ((line = await reader.ReadLineAsync()) != null) {
                        if(RegexTransactionGenerator.IsNormalTransaction(line)) {
                            var groups = RegexTransactionGenerator.SplitTransaction(line);
                            transaction.FirstName = groups[0];
                            transaction.LastName = groups[1];
                            transaction.Address = groups[2].Trim('“', '”', '"', '`');
                            transaction.Service = groups[8];

                            if (Decimal.TryParse(groups[5], out decimal payment)) {
                                transaction.Payment = payment;
                            }
                            if (DateOnly.TryParse(groups[6], out DateOnly date)) {
                                transaction.Date = date;
                            }
                            if (long.TryParse(groups[7], out long accountNumber)) {
                                transaction.AccountNumber = accountNumber;
                            }
                            transactions.Add(transaction);
                        }
                    }
                }
            }

            return transactions;
        }
    }
}
