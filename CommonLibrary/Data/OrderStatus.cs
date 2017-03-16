
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BetfairNG.Data
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderStatus
    {
        EXECUTION_COMPLETE,
        EXECUTABLE
    }
}
