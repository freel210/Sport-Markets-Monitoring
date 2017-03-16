using Model;
using System.Activities;

namespace ProcessLibrary
{

    public sealed class SaveMonitorinigHistory : CodeActivity
    {
        public InArgument<MarketInformation> CurrentMarket { get; set; }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            var currentMarketArg = new RuntimeArgument("CurrentMarket", typeof(MarketInformation), ArgumentDirection.In);
            metadata.AddArgument(currentMarketArg);
            metadata.Bind(CurrentMarket, currentMarketArg);
        }

        protected override void Execute(CodeActivityContext context)
        {
            CurrentMarket.Get(context).SaveHistory();
        }
    }
}
