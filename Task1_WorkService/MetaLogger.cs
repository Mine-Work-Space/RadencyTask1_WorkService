using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1_WorkService {
    internal static class MetaLogger {
        static ReaderWriterLock locker = new ReaderWriterLock();
        public static void WriteMetaFile(string path, string text) {
            try {
                locker.AcquireWriterLock(int.MaxValue);
                File.WriteAllLines(path, new[] { text });
            }
            finally {
                locker.ReleaseWriterLock();
            }
        }
    }
}
