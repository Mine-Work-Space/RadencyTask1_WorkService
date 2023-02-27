using System.Diagnostics;
using System.Text.RegularExpressions;
using Task1_WorkService.Models;
using System.IO;
using System.ComponentModel;
using System.Text.Json;
using System.Text;

namespace Task1_WorkService {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _pathFolderA;
        private readonly string _pathFolderB;
        private int _fileCounter;
        private readonly DateTime _startTime;
        public Worker(ILogger<Worker> logger, IConfiguration configuration) {
            _logger = logger;
            _configuration = configuration;
            _startTime = DateTime.Now;
            _pathFolderA = _configuration.GetValue<string>("DataPathFolderA")!;
            _pathFolderB = _configuration.GetValue<string>("DataPathFolderB")!;

            if(Directory.Exists(_pathFolderB + DateTime.Now.ToShortDateString().Replace('.', '-') + "\\")) {
                _fileCounter = Directory.GetFiles(_pathFolderB + DateTime.Now.ToShortDateString().Replace('.', '-') + "\\").Length;
            }
            else {
                _fileCounter = 0;
            }
            string path = _pathFolderB + DateTime.Now.ToShortDateString().Replace('.', '-') + "\\";

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            // Create directory Datetime with meta.log if doesn't exist
            if (!directoryInfo.Exists) {
                directoryInfo.Create();
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            try {
                while (!stoppingToken.IsCancellationRequested) {
                    if(DateTime.Now.Date != _startTime.Date) {
                        string path = _pathFolderB + DateTime.Now.ToShortDateString().Replace('.', '-') + "\\" + "meta.log";
                        // Update Meta Log file
                        File.Create(path);
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine($"parsed_files: " +
                            $"{_fileCounter}\r\n" +
                            $"parsed_lines: {MyFileManager.ParsedLines}\r\n" +
                            $"found_errors: {MyFileManager.ErrorsCount}\r\n" +
                            $"invalid_files: [");
                        foreach (var item in MyFileManager.InvalidFilePath)
                            builder.Append(item + " \n");
                        builder.AppendLine("]");
                        UpdateMetaLogFile(path,
                            builder.ToString());
                        Console.WriteLine("Meta log processed.");
                    }
                    // Start program
                    var newFilesList = NewFilesInDirectory(_pathFolderA);
                    if (newFilesList.Count > 0) {
                        foreach (var newFile in newFilesList) {
                            // Subjective
                            // ThreadPool.SetMaxThreads(10, 10);
                            ThreadPool.QueueUserWorkItem(new WaitCallback((obj) => {
                                Stopwatch stopWatch = new Stopwatch();
                                stopWatch.Start();

                                var transactionsList = MyFileManager.ReadAllLinesAsync(newFile);
                                // Make result by Linq
                                var result = transactionsList.Result.GroupBy(
                                    p => p.Address,
                                    (city, transactionsByAddress) => new ResultModel {
                                        City = city,
                                        Services = transactionsByAddress.GroupBy(u => u.Service,
                                            (serviceName, transactionsByServiceAndAddress) =>  new Service() {
                                            ServiceName = serviceName,
                                            Payers = transactionsByServiceAndAddress.Where(s => s.Service == serviceName && s.Address == city)
                                                .Select(y => new Payer() { 
                                                    AccountNumber = y.AccountNumber,
                                                    Date = y.Date,
                                                    PayerName = y.FirstName + " " + y.LastName,
                                                    Payment = y.Payment
                                                }).ToList(),
                                            Total = transactionsByServiceAndAddress.Sum(t => t.Payment)
                                        }).ToList(),
                                        Total = transactionsByAddress.Sum(t => t.Payment)
                                    });
                                if(result.Any()) {
                                    SaveConfig(result);
                                    stopWatch.Stop();
                                    Console.WriteLine($"\n===============================\n" +
                                        $"FilePath: {newFile.FullName} processed " +
                                        $"in {stopWatch.ElapsedMilliseconds} milliseconds.");
                                }
                            }));
                        }
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
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
        private void SaveConfig(IEnumerable<ResultModel> result) {
            string path = _pathFolderB + DateTime.Now.ToShortDateString().Replace('.', '-') + "\\";
            // Write output json file
            try {
            File.WriteAllText(
                Path.Combine(path, $"output{++_fileCounter}.json"), 
                JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true }));
            }
            catch(Exception err) {
                Console.WriteLine(err.Message);
            }
        }
        private void UpdateMetaLogFile(string path, string content) {
            MetaLogger.WriteMetaFile(path, content);
        }
    }
}