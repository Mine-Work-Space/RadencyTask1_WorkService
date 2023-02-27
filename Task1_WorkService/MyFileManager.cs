using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Task1_WorkService.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Task1_WorkService {
    internal static class MyFileManager {
        private const int DefaultBufferSize = 8192; // or 4096
        public static List<string> InvalidFilePath = new List<string>();
        public static long ParsedLines = 0;
        public static long ErrorsCount = 0;
        public static async Task<List<UserTransaction>> ReadAllLinesAsync(FileInfo file) {
            var transactions = new List<UserTransaction>();
            bool isInvalid = false;
            if (!IsFileLocked(file)) {
                using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, DefaultBufferSize)) {
                    using (var reader = new StreamReader(stream, Encoding.UTF8)) {
                        string? line;
                        UserTransaction transaction = new UserTransaction();
                        while ((line = await reader.ReadLineAsync()) != null) {
                            if(RegexTransactionGenerator.IsNormalTransaction(line)) {
                                var groups = RegexTransactionGenerator.SplitTransaction(line);
                                if(groups.Length == 9) {
                                    transaction = new UserTransaction();
                                    // Parsing data
                                    transaction.FirstName = groups[0];
                                    transaction.LastName = groups[1];
                                    transaction.Address = groups[2].Trim('“', '”', '"', '`');
                                    transaction.Service = groups[8];

                                    if(Decimal.TryParse(groups[5].Replace('.', ','), out decimal payment)) {
                                        transaction.Payment = payment;
                                    }
                                    transaction.Date = groups[6];
                                    transaction.AccountNumber = groups[7];
                                    transactions.Add(transaction);
                                    ParsedLines++;
                                }
                                else {
                                    // Errors for meta.log
                                    ErrorsCount++;
                                    isInvalid = true;
                                }
                            }
                            else {
                                ErrorsCount++;
                                isInvalid = true;
                            }
                        }
                    }
                }
            }
            if(isInvalid) {
                // Add invalid file path
                InvalidFilePath.Add(file.FullName);
            }
            return transactions;
        }
        public static bool IsFileLocked(FileInfo file) {
            try {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None)) {
                    stream.Close();
                }
            }
            catch (IOException) {
                //the file is unavailable because it is: still being written to or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            //file is not locked
            return false;
        }
    }
}
