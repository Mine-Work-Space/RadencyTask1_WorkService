using System.Text.RegularExpressions;
using Task1_WorkService.Models;

namespace Task1_WorkService {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly Regex _regex;
        private readonly string path;
        public Worker(ILogger<Worker> logger, IConfiguration configuration) {
            _logger = logger;
            _configuration = configuration;
            _regex = new Regex(@"(?:,\s+)|([“].+[”])");
            path = _configuration.GetValue<string>("DataPath");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                string line = "";
                string currentPath = Path.Combine("D:\\Data\\", "default_file.txt");
                Console.WriteLine(line);
                using (FileStream fs = new FileStream(@"D:\Data\file.txt", FileMode.Open, FileAccess.Read)) {
                    using (StreamReader sr = new StreamReader(fs)) {
                        line = sr.ReadToEnd();
                    }
                }
                if (_regex.IsMatch(line)) {
                    var groups = _regex.Split(line).ToList();
                    var transactions = new List<UserTransaction>();
                    foreach (var item in groups) {
                        if (!String.IsNullOrWhiteSpace(item.Trim())){
                            Console.WriteLine(item.Trim().Replace("“", "").Replace("”", ""));
                        }
                    }
                    //_logger.LogInformation(result);
                }
                else {
                    _logger.LogInformation("Not Match " + line);
                }
                Thread.Sleep(20000);
            }
        }
    }
}