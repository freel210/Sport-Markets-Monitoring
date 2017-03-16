using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BetfairNG.Data.Exceptions
{
    public sealed class Exception
    {
        // exception in json-rpc format
        [JsonProperty(PropertyName = "data")]
        public JObject Data { get; set; }		// actual exception details

        // exception in rescript format
        [JsonProperty(PropertyName = "detail")]
        public JObject Detail { get; set; }		// actual exception details
    }
}
