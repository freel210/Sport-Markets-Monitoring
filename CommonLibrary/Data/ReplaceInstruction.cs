using Newtonsoft.Json;

namespace BetfairNG.Data
{
    public sealed class ReplaceInstruction
    {
        [JsonProperty(PropertyName = "betId")]
        public string BetId { get; set; }

        [JsonProperty(PropertyName = "newPrice")]
        public double NewPrice { get; set; }
    }
}
