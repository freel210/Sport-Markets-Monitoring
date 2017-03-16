using BetfairNG.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BetfairNG.Client2
{
    public sealed class TestClient : IClient
    {
        private readonly miniBF bf = miniBF.Instance;

        public Task<BetfairServerResponse<List<MarketCatalogue>>> ListMarketCatalogue(
                MarketFilter marketFilter                = null,
                ISet<MarketProjection> marketProjections = null,
                MarketSort? sort                         = null,
                int maxResult                            = 1)
        {
            var bsr = new BetfairServerResponse<List<MarketCatalogue>>();
            bsr.Response = bf.GetMarketCatalogueResponse();
            bsr.RequestStart = DateTime.Now;
            bsr.LastByte     = DateTime.Now;
            bsr.LatencyMS    = 0L;
            bsr.HasError     = false;

            bsr.Response.Sort((x, y) => x.TotalMatched > y.TotalMatched ? -1 : 1);

            return Task.Run(() => bsr);
        }

        public Task<BetfairServerResponse<List<MarketBook>>> ListMarketBook(
                IEnumerable<string> mIds,
                PriceProjection priceProjection  = null,
                OrderProjection? orderProjection = null,
                MatchProjection? matchProjection = null)
        {
            var bsr = new BetfairServerResponse<List<MarketBook>>();
            bsr.RequestStart = DateTime.Now;
            bsr.LastByte     = DateTime.Now;
            bsr.LatencyMS    = 0L;
            bsr.HasError     = false;
            bsr.Response = bf.GetMarketBookResponse(mIds);

            return Task.Run(() => bsr);
        }
        
        public Task<BetfairServerResponse<AccountDetailsResponse>> GetAccountDetails()
        {
            var bsr = new BetfairServerResponse<AccountDetailsResponse>();

            return Task.Run(() => bsr);
        }

        public Task<BetfairServerResponse<AccountStatementReport>> GetAccountStatement(
            int? fromRecord = null,
            int? recordCount = null,
            TimeRange itemDateRange = null,
            IncludeItem? includeItem = null,
            Wallet? wallet = null)
        {
            var bsr = new BetfairServerResponse<AccountStatementReport>();

            return Task.Run(() => bsr);
        }

        public Task<BetfairServerResponse<AccountFundsResponse>> GetAccountFunds(Wallet wallet = Wallet.UK)
        {
            var bsr = new BetfairServerResponse<AccountFundsResponse>();

            return Task.Run(() => bsr);
        }
    }
}
