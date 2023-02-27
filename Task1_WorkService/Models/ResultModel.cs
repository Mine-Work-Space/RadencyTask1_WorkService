using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Task1_WorkService.Models {
    internal class ResultModel {
        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;
        [JsonPropertyName("services")]
        public List<Service> Services { get; set; } = new List<Service>();
        public decimal Total { get; set; } = decimal.Zero;
    }
    internal class Service {
        [JsonPropertyName("name")]
        public string ServiceName { get; set; } = string.Empty;
        [JsonPropertyName("payers")]
        public List<Payer> Payers { get; set; } = new List<Payer>();
        public decimal Total { get; set; } = decimal.Zero;
    }
    internal class Payer {
        [JsonPropertyName("name")]
        public string PayerName { get; set; } = string.Empty;
        [JsonPropertyName("payment")]
        public decimal Payment { get; set; } = decimal.Zero;
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;
        [JsonPropertyName("account_number")]
        public string AccountNumber { get; set; } = string.Empty;
        public override string ToString() {
            return $"{{ {PayerName}, {Payment}, {Date}, {AccountNumber} }}";
        }
    }
}
