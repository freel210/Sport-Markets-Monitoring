using System.Collections.Generic;
using Newtonsoft.Json;

namespace BetfairNG.Data
{
    public sealed class ClearedOrderSummaryReport
    {
        [JsonProperty(PropertyName = "clearedOrders")]
        public IList<ClearedOrderSummary> ClearedOrders { get; set; }

        [JsonProperty(PropertyName = "moreAvailable")]
        public bool MoreAvailable { get; set; }
    }
}
