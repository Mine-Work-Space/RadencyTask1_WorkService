using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task1_WorkService {
    internal static partial class RegexTransactionGenerator {
        // That way Regex is more faster, .net 7 one love
        [GeneratedRegex(@"(?:,\s+)|((“)(?:,\s+)(”))", RegexOptions.Compiled, 20)]
        private static partial Regex IsNormalTransactionRegex();

        public static string[] SplitTransaction(string transaction) {
            return IsNormalTransactionRegex().Split(transaction);
        }
        public static bool IsNormalTransaction(string transaction) {
            return IsNormalTransactionRegex().IsMatch(transaction);
        }
    }
}
