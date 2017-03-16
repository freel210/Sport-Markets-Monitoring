using Newtonsoft.Json;

namespace BetfairNG.Data
{
    public sealed class RunnerProfitAndLoss
    {
        [JsonProperty(PropertyName = "selectionId")]
        public long SelectionId { get; set; }

        [JsonProperty(PropertyName = "ifWin")]
        public double IfWin { get; set; }

        [JsonProperty(PropertyName = "ifLose")]
        public double IfLose { get; set; }
    }
}
