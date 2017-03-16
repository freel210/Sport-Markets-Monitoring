using BetfairNG.Client2;
using BetfairNG.Data;
using System.Collections.Generic;
using CommonLibrary;
using System.Collections.ObjectModel;

namespace Model
{
    public class ModelCore : NotifyPropertyChanger
    {
        //protected readonly IClient client = new BetfairClient();
        protected readonly IClient client = new TestClient();

        public List<MarketInformation> GetMarketBook(List<string> mIDs)
        {
            List<MarketInformation> result = new List<MarketInformation>();
            BetfairServerResponse<List<MarketBook>> receivedData = ListMarketBook(mIDs);

            List<MarketBook> marketbook = receivedData.Response;
            foreach (var mb in marketbook)
            {
                MarketBookInformation mbi = new MarketBookInformation()
                {
                    NumberOfRunners = mb.NumberOfRunners,
                    NumberOfWinners = mb.NumberOfWinners,
                    Status = mb.Status,
                    ExPricesRunner0 = mb.Runners[0].ExchangePrices,
                    ExPricesRunner1 = mb.Runners[1].ExchangePrices,
                    LastByte = receivedData.LastByte,
                    IsInplay = mb.IsInplay,
                };
                    
                result.Add(new MarketInformation()
                {
                    MarketId = mb.MarketId,
                    Mbi = new ObservableCollection<MarketBookInformation>{ mbi }
                });

                // TEST !!!
                //удаляем для рынка "123464" AvailableToBack[2] для тестовых нужд
                //if (mb.MarketId == "123464")
                //    mb.Runners[1].ExchangePrices.AvailableToBack.RemoveAt(2);
            }

            return result;
        }

        protected BetfairServerResponse<List<MarketBook>> ListMarketBook(List<string> mIDs)
        {
            PriceProjection pp = new PriceProjection()
            {
                PriceData = new HashSet<PriceData>() { PriceData.EX_ALL_OFFERS }
            };

            return client.ListMarketBook(mIDs, pp).Result;
        }
    }
}
