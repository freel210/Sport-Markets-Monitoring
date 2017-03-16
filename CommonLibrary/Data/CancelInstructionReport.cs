using System;
using Newtonsoft.Json;

namespace BetfairNG.Data
{
    public sealed class CancelInstructionReport
    {
        [JsonProperty(PropertyName = "status")]
        public InstructionReportStatus Status { get; set; }

        [JsonProperty(PropertyName = "errorCode")]
        public InstructionReportErrorCode ErrorCode { get; set; }

        [JsonProperty(PropertyName = "instruction")]
        public CancelInstruction Instruction { get; set; }

        [JsonProperty(PropertyName = "sizeCancelled")]
        public double SizeCancelled { get; set; }

        [JsonProperty(PropertyName = "cancelledDate")]
        public DateTime CancelledDate { get; set; }

    }
}
