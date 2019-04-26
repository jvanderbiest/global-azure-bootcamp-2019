using System.Collections.Generic;
using Newtonsoft.Json;

namespace azure.Domain
{
    public class LuisResult
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("topScoringIntent")]
        public Intent TopScoringIntent { get; set; }

        [JsonProperty("intents")]
        public List<Intent> Intents { get; set; }

        [JsonProperty("entities")]
        public List<object> Entities { get; set; }

        public bool HasSufficientScore()
        {
            if (TopScoringIntent.Score > 0.7)
            {
                return true;
            }

            return false;
        }
    }
}