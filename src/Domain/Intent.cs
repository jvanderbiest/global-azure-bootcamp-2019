using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace azure.Domain
{
    public class Intent
    {
        [JsonProperty("intent")]
        public string IntentIntent { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }
    }
}
