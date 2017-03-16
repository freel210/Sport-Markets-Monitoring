using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BetfairNG.Data
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IncludeItem
    {
        ALL,
        DEPOSITS_WITHDRAWALS,
        EXCHANGE,
        POKER_ROOM,
    }
}
