using BetfairNG.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CommonLibrary;
using System.Collections.ObjectModel;

namespace Model
{
    public sealed class MainModel : ModelCore
    {
        //свойства
        private ThreadSafeObservableCollection<MarketInformation> _marketInformationDict;
        public ThreadSafeObservableCollection<MarketInformation> MarketInformationDict
        {
            get { return _marketInformationDict; }

            set
            {
                if (_marketInformationDict == value) return;

                _marketInformationDict = value;
                OnPropertyChanged();
            }
        }

        public MainModel()
        {
            _marketInformationDict = new ThreadSafeObservableCollection<MarketInformation>();
        }

        public void FillMarketListWithDetailedInformation(CancellationToken ct)
        {
            //получить MarketCatalogue
            List<MarketInformation> marketCatalogue = GetMarketCatalogue(); //может здесь нужно не MarketInformation "недозаполнять", 
                                                                            //а отдельный миникласс сделать? и отправлять его в конструктор MarketInformation   
            //получить MarketBook
            List<string> mIDs = marketCatalogue.Select(x => x.MarketId).ToList();

            List<MarketInformation> marketBook = GetMarketBook(mIDs);

            //объединяем оба Dictionary
            var query = from mldi in marketCatalogue
                        join mb in marketBook
                        on mldi.MarketId equals mb.MarketId
                        where mb.Mbi.Last().NumberOfWinners == 1
                        select new MarketInformation()
                        {
                            MarketId = mldi.MarketId,
                            MarketName = mldi.MarketName,
                            TotalMatched = mldi.TotalMatched,
                            EventType = mldi.EventType,
                            EventName = mldi.EventName,
                            Runner0Name = mldi.Runner0Name,
                            Runner1Name = mldi.Runner1Name,
                            CompetitionName = mldi.CompetitionName,
                            StartMarketTime = mldi.StartMarketTime,

                            Mbi = new ObservableCollection<MarketBookInformation>()
                            {
                                new MarketBookInformation()    
                                {
                                    MarketName = mldi.MarketName, //важно! проброс MarketName из MarketInformation в MarketBookInformation
                                    Runner0Name = mb.Runner0Name,
                                    Runner1Name = mb.Runner1Name,

                                    NumberOfRunners = mb.Mbi.Last().NumberOfRunners,
                                    NumberOfWinners = mb.Mbi.Last().NumberOfWinners,
                                    Status = mb.Mbi.Last().Status,
                                    ExPricesRunner0 = mb.Mbi.Last().ExPricesRunner0,
                                    ExPricesRunner1 = mb.Mbi.Last().ExPricesRunner1,
                                    LastByte = mb.Mbi.Last().LastByte,
                                    IsInplay = mb.Mbi.Last().IsInplay
                                }
                            }
                        };

            //присваиваем полученное значение требуемому Dictionary
            IList<MarketInformation> tempDict = query.ToList();

            MarketInformationDict.Clear();

            foreach (var item in tempDict)
            {
                ct.ThrowIfCancellationRequested();
                //Thread.Sleep(50);
                item.Mbi.Last().FillMissingData();
                MarketInformationDict.Add(item);
            }
        }

        private List<MarketInformation> GetMarketCatalogue()
        {
            List<MarketInformation> result = new List<MarketInformation>();

            //создать фильтр для получения рынков
            var marketFilter = new MarketFilter
            {
                //MarketStartTime = new TimeRange()
                //{
                //    From = DateTime.Now,
                //    To = DateTime.Now.AddDays(3)
                //}
            };

            //задать дополнительно получаемую информацию
            HashSet<MarketProjection> marketProjections = new HashSet<MarketProjection>()
            {
                MarketProjection.EVENT,
                MarketProjection.EVENT_TYPE,
                MarketProjection.RUNNER_DESCRIPTION,
                MarketProjection.COMPETITION
            };

            //получить перечень рынков
            List<MarketCatalogue> marketCatalogues = client.ListMarketCatalogue(
              marketFilter,
              marketProjections,//
              MarketSort.MAXIMUM_TRADED,
              100).Result.Response;

            //Запихиваем результат в Dictionary
            for (int i = 0; i < marketCatalogues.Count; i++)
            {
                //если есть уже 10 рынков, то заканчиваем набирать новые
                if (result.Count >= 10) break;
                
                //если раннеров не 2, то такой рынок мы запоминать не будем
                if (marketCatalogues[i].Runners.Count != 2) continue;

                string CompName;
                if (marketCatalogues[i].Competition.Name != null)
                    CompName = marketCatalogues[i].Competition.Name;
                else
                    CompName = "-";

                result.Add(new MarketInformation()
                {
                    MarketId = marketCatalogues[i].MarketId, //номер рынка тоже запомнили
                    MarketName = marketCatalogues[i].MarketName,
                    TotalMatched = marketCatalogues[i].TotalMatched,
                    EventType = marketCatalogues[i].EventType.Name,
                    EventName = marketCatalogues[i].Event.Name,
                    Runner0Name = marketCatalogues[i].Runners[0].RunnerName,
                    Runner1Name = marketCatalogues[i].Runners[1].RunnerName,
                    CompetitionName = CompName,
                    StartMarketTime = marketCatalogues[i].MarketStartTime.ToLocalTime()
                });
            }

            return result;
        }

        #region Get Betfair Details

        public AccountDetailsModel GetAccountDetails()
        {
            AccountDetailsModel result = new AccountDetailsModel();

            var accountDetails = client.GetAccountDetails().Result;

            if (accountDetails.Response == null)
            {
                result.CountryCode  = "-";
                result.CurrencyCode = "-";
                result.FirstName    = "-";
                result.LastName     = "-";
                result.LocalCode    = "-";
                result.Region       = "-";
                result.TimeZone     = "-";
                result.DiscountRate = "-";
            }
            else if (accountDetails.Error == null)
            {
                result.CountryCode  = accountDetails.Response.CountryCode;
                result.CurrencyCode = accountDetails.Response.CurrencyCode;
                result.FirstName    = accountDetails.Response.FirstName;
                result.LastName     = accountDetails.Response.LastName;
                result.LocalCode    = accountDetails.Response.LocaleCode;
                result.Region       = accountDetails.Response.Region;
                result.TimeZone     = accountDetails.Response.TimeZone;
                result.DiscountRate = accountDetails.Response.DiscountRate.ToString(CultureInfo.CurrentCulture);
            }

            return result;
        }

        public AccountFundsModel GetAccountFunds()
        {
            AccountFundsModel result = new AccountFundsModel();

            var accountFunds = client.GetAccountFunds(Wallet.UK).Result;

            if (accountFunds.Response == null)
            {
                result.AvailableToBetBalance = "-";
                result.DiscountRate          = "-";
                result.Exposure              = "-";
                result.ExposureLimit         = "-";
                result.PointsBalance         = "-";
                result.RetainedCommission    = "-";
            }
            else if (accountFunds.Error == null)
            {
                result.AvailableToBetBalance = accountFunds.Response.AvailableToBetBalance.ToString(CultureInfo.CurrentCulture);
                result.DiscountRate = accountFunds.Response.DiscountRate.ToString(CultureInfo.CurrentCulture);
                result.Exposure = accountFunds.Response.Exposure.ToString(CultureInfo.CurrentCulture);
                result.ExposureLimit = accountFunds.Response.ExposureLimit.ToString(CultureInfo.CurrentCulture);
                result.PointsBalance = accountFunds.Response.PointsBalance.ToString(CultureInfo.CurrentCulture);
                result.RetainedCommission = accountFunds.Response.RetainedCommission.ToString(CultureInfo.CurrentCulture);
            }

            return result;
        }

        #endregion

    }
}
