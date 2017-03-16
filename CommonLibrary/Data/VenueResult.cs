using Newtonsoft.Json;

namespace BetfairNG.Data
{
    public sealed class VenueResult
    {
        [JsonProperty(PropertyName = "venue")]
        public string Venue { get; set; }

        [JsonProperty(PropertyName = "marketCount")]
        public int MarketCount { get; set; }
    }
}
