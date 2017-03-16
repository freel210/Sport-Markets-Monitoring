using System.Collections.Generic;
using System.Activities;
using ProcessLibrary;

namespace Model
{
    public sealed class MarketMonitoring_ProxyModel
    {
        private const int REQUEST_DELAY = 500;
        private WorkflowApplication workflow;

        public MarketMonitoring_ProxyModel(MarketInformation currentMarket)
        {
            IDictionary<string, object> input = new Dictionary<string, object>
            {
                { "Market" , currentMarket },
                { "RequestDelay" , REQUEST_DELAY }
            };

            workflow = new WorkflowApplication(new MarketMonitoring(), input);
            workflow.Run();
        }

        public void CancelMarketMonitoring()
        {
            workflow.Cancel();
        }
    }
}
