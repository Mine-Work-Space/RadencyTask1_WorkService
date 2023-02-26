using System.Diagnostics;
using System.Text.RegularExpressions;
using Task1_WorkService.Models;

namespace Task1_WorkService {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _pathFolderA;
        private readonly string _pathFolderB;
        public Worker(ILogger<Worker> logger, IConfiguration configuration) {
            _logger = logger;
            _configuration = configuration;
            _pathFolderA = _configuration.GetValue<string>("DataPathFolderA");
            _pathFolderB = _configuration.GetValue<string>("DataPathFolderB");
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                string currentPath = Path.Combine(_pathFolderA, "file_medium.txt");
                var transactionsList = await MyFileReader.ReadAllLinesAsync(currentPath);
                //var cities = transactionsList.DistinctBy(t => t)
                stopWatch.Stop();
                Console.WriteLine(stopWatch.ElapsedMilliseconds.ToString() + " transactionsList Count: " + transactionsList.Count);
            }
        }
    }
}