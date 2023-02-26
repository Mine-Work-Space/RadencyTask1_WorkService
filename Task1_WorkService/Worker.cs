using System.Diagnostics;
using System.Text.RegularExpressions;
using Task1_WorkService.Models;
using System.IO;

namespace Task1_WorkService {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _pathFolderA;
        private readonly string _pathFolderB;
        public Worker(ILogger<Worker> logger, IConfiguration configuration) {
            _logger = logger;
            _configuration = configuration;
            _pathFolderA = _configuration.GetValue<string>("DataPathFolderA")!;
            _pathFolderB = _configuration.GetValue<string>("DataPathFolderB")!;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                // Start program
                var newFilesList = NewFilesInDirectory(_pathFolderA);
                if (newFilesList.Count > 0) {
                    foreach (var newFile in newFilesList) {
                        ThreadPool.QueueUserWorkItem(new WaitCallback((obj) => {
                            Stopwatch stopWatch = new Stopwatch();
                            stopWatch.Start();
                            var transactionsList = MyFileReader.ReadAllLinesAsync(newFile);

                            Console.WriteLine($"\n===============================\nTransaction list {transactionsList.Result.Count}\n\tFilePath: {newFile.FullName}");
                            if(transactionsList.IsCompletedSuccessfully && transactionsList.Result.Count > 0) {
                                List<ResultModel> result = new List<ResultModel>();
                                ResultModel resultModel = new ResultModel();
                                // To DO
                            }
                            stopWatch.Stop();
                            Console.WriteLine("StopWatch: " + stopWatch.ElapsedMilliseconds.ToString() + "\n====================");
                        }));
                    }
                }
            }
        }
        private List<FileInfo> NewFilesInDirectory(string directoryPath) {
            try {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                if(directoryInfo != null && directoryInfo.Exists) {
                    return directoryInfo.GetFiles()
                        .Where(file => !file.Attributes.HasFlag(FileAttributes.Normal)
                            && (file.Extension == ".txt" || file.Extension == ".csv"))
                        .Select(file => { File.SetAttributes(directoryPath + file.Name, FileAttributes.Normal); return file; })
                        .ToList();
                }
            }
            catch(Exception) { 
                return new List<FileInfo>();
            }
            return new List<FileInfo>();
        }
    }
}