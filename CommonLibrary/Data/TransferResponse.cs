using Newtonsoft.Json;

namespace BetfairNG.Data
{
    public sealed class TransferResponse
    {
        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }
    }
}
