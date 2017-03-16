using System.Activities;
using Model;

namespace ProcessLibrary
{

    public sealed class UpdateMarket : CodeActivity
    {
        public InArgument<MarketInformation> CurrentMarket { get; set; }

        private MarketInformation _m;

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            var currentMarketArg = new RuntimeArgument("CurrentMarket", typeof(MarketInformation), ArgumentDirection.In);
            metadata.AddArgument(currentMarketArg);
            metadata.Bind(CurrentMarket, currentMarketArg);
        }

        protected override void Execute(CodeActivityContext context)
        {
            if(_m == null)
                _m = CurrentMarket.Get(context);

            _m.UpdateMarket();
        }
    }
}
