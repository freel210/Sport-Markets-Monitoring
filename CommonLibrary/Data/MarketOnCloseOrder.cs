using System.Text;
using Newtonsoft.Json;

namespace BetfairNG.Data
{
    public sealed class MarketOnCloseOrder
    {
        [JsonProperty(PropertyName = "size")]
        public double Size { get; set; }

        public override string ToString()
        {
            return new StringBuilder()
                        .AppendFormat("Size={0}", Size)
                        .ToString();
        }
    }
}
