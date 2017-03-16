using Newtonsoft.Json;

namespace BetfairNG.Data
{
    public sealed class TimeRangeResult
    {
        [JsonProperty(PropertyName = "timeRange")]
        public TimeRange TimeRange { get; set; }

        [JsonProperty(PropertyName = "marketCount")]
        public int MarketCount { get; set; }
    }
}
