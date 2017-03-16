using System.Collections.Generic;
using System.Threading.Tasks;
using BetfairNG.Data;

namespace BetfairNG.Client2
{
    public interface IClient
    {
        Task<BetfairServerResponse<List<MarketCatalogue>>> ListMarketCatalogue(
            MarketFilter marketFilter,
            ISet<MarketProjection> marketProjections = null,
            MarketSort? sort = null,
            int maxResult = 1);

        Task<BetfairServerResponse<List<MarketBook>>> ListMarketBook(
            IEnumerable<string> marketIds,
            PriceProjection priceProjection = null,
            OrderProjection? orderProjection = null,
            MatchProjection? matchProjection = null);

        Task<BetfairServerResponse<AccountDetailsResponse>> GetAccountDetails();

        Task<BetfairServerResponse<AccountStatementReport>> GetAccountStatement(
            int? fromRecord = null,
            int? recordCount = null,
            TimeRange itemDateRange = null,
            IncludeItem? includeItem = null,
            Wallet? wallet = null);

        Task<BetfairServerResponse<AccountFundsResponse>> GetAccountFunds(Wallet wallet = Wallet.UK);
    }
}
