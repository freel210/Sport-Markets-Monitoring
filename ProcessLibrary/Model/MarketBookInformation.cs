using System;
using System.Collections.Generic;
using System.Linq;
using BetfairNG.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Model.Analisys;
using CommonLibrary;

namespace Model
{
    public sealed class MarketBookInformation : NotifyPropertyChanger
    {
        //информация из MarketCatalogue для выставления ставки (дублируется с данными из MarketInformation)
        public string MarketName { get; set; } //служит для проверки записывалась ли информация в объект или еще нет
        public string Runner0Name { get; set; }
        public string Runner1Name { get; set; }        
        
        //инфомация из MarketBook
        public int NumberOfRunners { get; set; }
        public int NumberOfWinners { get; set; }

        private MarketStatus _status;
        public MarketStatus Status
        {
            get { return _status; }

            set
            {
                if (_status == value) return;

                _status = value;
                OnPropertyChanged();
            }
        }

        private ExchangePrices _exPricesRunner0;
        private readonly PropertyChangedEventHandler _exPrices0ChangeHandler;
        public ExchangePrices ExPricesRunner0
        {
            get { return _exPricesRunner0; }
            set
            {
                if (_exPricesRunner0 == value) return;

                if (_exPricesRunner0 != null)
                    _exPricesRunner0.PropertyChanged -= _exPrices0ChangeHandler;

                _exPricesRunner0 = value;

                if (_exPricesRunner0 != null)
                    _exPricesRunner0.PropertyChanged += _exPrices0ChangeHandler;

                OnPropertyChanged();
            }
        }

        private ExchangePrices _exPricesRunner1;
        private readonly PropertyChangedEventHandler _exPrices1ChangeHandler;
        public ExchangePrices ExPricesRunner1
        {
            get { return _exPricesRunner1; }
            set
            {
                if (_exPricesRunner1 == value) return;

                if (_exPricesRunner1 != null)
                    _exPricesRunner1.PropertyChanged -= _exPrices1ChangeHandler;

                _exPricesRunner1 = value;

                if (_exPricesRunner1 != null)
                    _exPricesRunner1.PropertyChanged += _exPrices1ChangeHandler;

                OnPropertyChanged();
            }
        }

        private DateTime _lastByte;
        public DateTime LastByte
        {
            get { return _lastByte; }

            set
            {
                if (_lastByte == value) return;

                _lastByte = value;
                OnPropertyChanged();
            }
        }

        public bool IsInplay { get; set; }

        //устанавливаемые пользователем свойства
        public double MaxObligation { get; set; }

        //конструктор
        public MarketBookInformation()
        {
            //обработчики внутренних изменений
            _exPrices0ChangeHandler = (sender, e) => OnPropertyChanged("ExPricesRunner0");
            _exPrices1ChangeHandler = (sender, e) => OnPropertyChanged("ExPricesRunner1");

            MaxObligation = 6.00;
        }

        //заполняет недостающие данные заглушками, чтобы не валился интерфейс и аналитика
        public void FillMissingData()
        {
            _fillMissingData(ExPricesRunner0);
            _fillMissingData(ExPricesRunner1);
        }

        private void _fillMissingData(ExchangePrices ep)
        {
            if (ep == null)
                ep = new ExchangePrices();

            if (ep.AvailableToBack == null)
                ep.AvailableToBack = new ObservableCollection<PriceSize>();

            if (ep.AvailableToLay == null)
                ep.AvailableToLay = new ObservableCollection<PriceSize>();

            if (ep.TradedVolume == null)
                ep.TradedVolume = new ObservableCollection<PriceSize>();

            if (ep.AvailableToBack.Count < 3)
                AddPriceSize(ep.AvailableToBack, 1.01, 3 - ep.AvailableToBack.Count);

            if (ep.AvailableToLay.Count < 3)
                AddPriceSize(ep.AvailableToLay, 1000, 3 - ep.AvailableToLay.Count);

            if (ep.TradedVolume.Count < 3)
                AddPriceSize(ep.TradedVolume, 0, 3);
        }

        private void AddPriceSize(ObservableCollection<PriceSize> lst, double price, int q)
        {
            for (int i = 1; i <= q; i++)
                lst.Add(new PriceSize() { Price = price, Size = 0 });
        }

        //расчетные аналитические свойства
        private Dictionary<TradeCases, IAnalisys> _casesAnalisys;

        [XmlIgnore]
        public Dictionary<TradeCases, IAnalisys> CasesAnalisys
        {
            set { _casesAnalisys = value; }
            get { return GetCasesAnalisys(); }
        }

        [XmlIgnore]
        public TradeCases BestCase
        {
            get { return GetBestCase(); }
        }

        private TradeCases GetBestCase()
        {
            if (MarketName == null)
                return TradeCases.empty;

            TradeCases BL1B2 = TradeCases.BL1B2;
            TradeCases BL2B1 = TradeCases.BL2B1;
            TradeCases LB1L2 = TradeCases.LB1L2;
            TradeCases LB2L1 = TradeCases.LB2L1;

            List<CaseInfo> ciList = new List<CaseInfo>();
            ciList.Add(new CaseInfo()
            {
                Case = BL1B2,
                TradeRatio = CasesAnalisys[BL1B2].TradeRatio,
                WaitingRation = CasesAnalisys[BL1B2].WaitingRatio
            });

            ciList.Add(new CaseInfo()
            {
                Case = BL2B1,
                TradeRatio = CasesAnalisys[BL2B1].TradeRatio,
                WaitingRation = CasesAnalisys[BL2B1].WaitingRatio
            });

            ciList.Add(new CaseInfo()
            {
                Case = LB1L2,
                TradeRatio = CasesAnalisys[LB1L2].TradeRatio,
                WaitingRation = CasesAnalisys[LB1L2].WaitingRatio
            });

            ciList.Add(new CaseInfo()
            {
                Case = LB2L1,
                TradeRatio = CasesAnalisys[LB2L1].TradeRatio,
                WaitingRation = CasesAnalisys[LB2L1].WaitingRatio
            });

            TradeCases result = (from c in ciList
                                 where c.TradeRatio < 1 & c.WaitingRation < 1
                                 orderby c.WaitingRation
                                 select c.Case).FirstOrDefault();

            return result;
        }

        private Dictionary<TradeCases, IAnalisys> GetCasesAnalisys()
        {
            _casesAnalisys = new Dictionary<TradeCases, IAnalisys>();

            if (MarketName == null) return _casesAnalisys;

            _casesAnalisys.Add(TradeCases.BL1B2, Get_BL1B2_Analisys());
            _casesAnalisys.Add(TradeCases.BL2B1, Get_BL2B1_Analisys());
            _casesAnalisys.Add(TradeCases.LB1L2, Get_LB1L2_Analisys());
            _casesAnalisys.Add(TradeCases.LB2L1, Get_LB2L1_Analisys());
            _casesAnalisys.Add(TradeCases.BL1LB1, Get_BL1LB1_Analisys());
            _casesAnalisys.Add(TradeCases.BL2LB2, Get_BL2LB2_Analisys());

            return _casesAnalisys;
        }

        private IAnalisys Get_BL1B2_Analisys()
        {
            double _startPrice, _closePrice, _startSize, _closeSize;
            double _b1Price, _b1Size, _b2Price, _b2Size, _l1Price, _l1Size, _l2Price, _l2Size;
            int _gap = getGap_seekingForward(ExPricesRunner0.AvailableToBack[0], ExPricesRunner0.AvailableToLay[0]);

            if (_gap == 0)
            {
                _startPrice = ExPricesRunner0.AvailableToLay[0].Price;
                _startSize = ExPricesRunner0.AvailableToLay[0].Size;

                _l1Price = ExPricesRunner0.AvailableToLay[0].Price;
                _l1Size = ExPricesRunner0.AvailableToLay[0].Size;

            }
            else
            {
                _startPrice = ExPricesRunner0.AvailableToBack[0].NextPrice;
                _startSize = 0;

                _l1Price = ExPricesRunner0.AvailableToBack[0].NextPrice;
                _l1Size = 0;
            }

            _closePrice = ExPricesRunner1.AvailableToBack[0].Price;
            _closeSize = ExPricesRunner1.AvailableToBack[0].Size;

            _b1Price = ExPricesRunner0.AvailableToBack[0].Price;
            _b1Size = ExPricesRunner0.AvailableToBack[0].Size;

            _b2Price = ExPricesRunner1.AvailableToBack[0].Price;
            _b2Size = ExPricesRunner1.AvailableToBack[0].Size;

            _l2Price = ExPricesRunner1.AvailableToLay[0].Price;
            _l2Size = ExPricesRunner1.AvailableToLay[0].Size;

            IAnalisys result = new BackAnalisys
            (
                startRunnerID: Runner0Name,
                closeRunnerID: Runner1Name,

                startPrice: _startPrice,
                closePrice: _closePrice,

                startSize: _startSize,
                closeSize: _closeSize,

                maxObligation: MaxObligation,

                b1Price: _b1Price,
                b1Size: _b1Size,

                b2Price: _b2Price,
                b2Size: _b2Size,

                l1Price: _l1Price,
                l1Size: _l1Size,

                l2Price: _l2Price,
                l2Size: _l2Size,

                gap: _gap
            );

            return result;
        }

        private IAnalisys Get_BL2B1_Analisys()
        {
            double _startPrice, _closePrice, _startSize, _closeSize;
            double _b1Price, _b1Size, _b2Price, _b2Size, _l1Price, _l1Size, _l2Price, _l2Size;
            int _gap = getGap_seekingForward(ExPricesRunner1.AvailableToBack[0], ExPricesRunner1.AvailableToLay[0]);

            if (_gap == 0)
            {
                _startPrice = ExPricesRunner1.AvailableToLay[0].Price;
                _startSize = ExPricesRunner1.AvailableToLay[0].Size;

                _l2Price = ExPricesRunner1.AvailableToLay[0].Price;
                _l2Size = ExPricesRunner1.AvailableToLay[0].Size;
            }
            else
            {
                _startPrice = ExPricesRunner1.AvailableToBack[0].NextPrice;
                _startSize = 0;

                _l2Price = ExPricesRunner1.AvailableToBack[0].NextPrice;
                _l2Size = 0;
            }

            _closePrice = ExPricesRunner0.AvailableToBack[0].Price;
            _closeSize = ExPricesRunner0.AvailableToBack[0].Size;

            _b1Price = ExPricesRunner0.AvailableToBack[0].Price;
            _b1Size = ExPricesRunner0.AvailableToBack[0].Size;

            _b2Price = ExPricesRunner1.AvailableToBack[0].Price;
            _b2Size = ExPricesRunner1.AvailableToBack[0].Size;

            _l1Price = ExPricesRunner0.AvailableToLay[0].Price;
            _l1Size = ExPricesRunner0.AvailableToLay[0].Size;

            IAnalisys result = new BackAnalisys
            (
                startRunnerID: Runner0Name,
                closeRunnerID: Runner1Name,

                startPrice: _startPrice,
                closePrice: _closePrice,

                startSize: _startSize,
                closeSize: _closeSize,

                maxObligation: MaxObligation,

                b1Price: _b1Price,
                b1Size: _b1Size,

                b2Price: _b2Price,
                b2Size: _b2Size,

                l1Price: _l1Price,
                l1Size: _l1Size,

                l2Price: _l2Price,
                l2Size: _l2Size,

                gap: _gap
            );

            return result;
        }

        private IAnalisys Get_LB1L2_Analisys()
        {
            double _startPrice, _closePrice, _startSize, _closeSize;
            double _b1Price, _b1Size, _b2Price, _b2Size, _l1Price, _l1Size, _l2Price, _l2Size;
            int _gap = getGap_seekingBack(ExPricesRunner0.AvailableToLay[0], ExPricesRunner0.AvailableToBack[0]);

            if (_gap == 0)
            {
                _startPrice = ExPricesRunner0.AvailableToBack[0].Price;
                _startSize = ExPricesRunner0.AvailableToBack[0].Size;

                _b1Price = ExPricesRunner0.AvailableToBack[0].Price;
                _b1Size = ExPricesRunner0.AvailableToBack[0].Size;
            }
            else
            {
                _startPrice = ExPricesRunner0.AvailableToLay[0].PreviousPrice;
                _startSize = 0;

                _b1Price = ExPricesRunner0.AvailableToLay[0].PreviousPrice;
                _b1Size = 0;
            }

            _closePrice = ExPricesRunner1.AvailableToLay[0].Price;
            _closeSize = ExPricesRunner1.AvailableToLay[0].Size;

            _b2Price = ExPricesRunner1.AvailableToBack[0].Price;
            _b2Size = ExPricesRunner1.AvailableToBack[0].Size;

            _l1Price = ExPricesRunner0.AvailableToLay[0].Price;
            _l1Size = ExPricesRunner0.AvailableToLay[0].Size;

            _l2Price = ExPricesRunner1.AvailableToLay[0].Price;
            _l2Size = ExPricesRunner1.AvailableToLay[0].Size;

            IAnalisys result = new LayAnalisys
            (
                startRunnerID: Runner0Name,
                closeRunnerID: Runner1Name,

                startPrice: _startPrice,
                closePrice: _closePrice,

                startSize: _startSize,
                closeSize: _closeSize,

                maxObligation: MaxObligation,

                b1Price: _b1Price,
                b1Size: _b1Size,

                b2Price: _b2Price,
                b2Size: _b2Size,

                l1Price: _l1Price,
                l1Size: _l1Size,

                l2Price: _l2Price,
                l2Size: _l2Size,

                gap: _gap
            );

            return result;
        }

        private IAnalisys Get_LB2L1_Analisys()
        {
            double _startPrice, _closePrice, _startSize, _closeSize;
            double _b1Price, _b1Size, _b2Price, _b2Size, _l1Price, _l1Size, _l2Price, _l2Size;
            int _gap = getGap_seekingBack(ExPricesRunner1.AvailableToLay[0], ExPricesRunner1.AvailableToBack[0]);

            if (_gap == 0)
            {
                _startPrice = ExPricesRunner1.AvailableToBack[0].Price;
                _startSize = ExPricesRunner1.AvailableToBack[0].Size;

                _b2Price = ExPricesRunner1.AvailableToBack[0].Price;
                _b2Size = ExPricesRunner1.AvailableToBack[0].Size;
            }
            else
            {
                _startPrice = ExPricesRunner1.AvailableToLay[0].PreviousPrice;
                _startSize = 0;

                _b2Price = ExPricesRunner1.AvailableToLay[0].PreviousPrice;
                _b2Size = 0;
            }

            _closePrice = ExPricesRunner0.AvailableToLay[0].Price;
            _closeSize = ExPricesRunner0.AvailableToLay[0].Size;

            _b1Price = ExPricesRunner0.AvailableToBack[0].Price;
            _b1Size = ExPricesRunner0.AvailableToBack[0].Size;

            _l1Price = ExPricesRunner0.AvailableToLay[0].Price;
            _l1Size = ExPricesRunner0.AvailableToLay[0].Size;

            _l2Price = ExPricesRunner1.AvailableToLay[0].Price;
            _l2Size = ExPricesRunner1.AvailableToLay[0].Size;

            IAnalisys result = new LayAnalisys
            (
                startRunnerID: Runner0Name,
                closeRunnerID: Runner1Name,

                startPrice: _startPrice,
                closePrice: _closePrice,

                startSize: _startSize,
                closeSize: _closeSize,

                maxObligation: MaxObligation,

                b1Price: _b1Price,
                b1Size: _b1Size,

                b2Price: _b2Price,
                b2Size: _b2Size,

                l1Price: _l1Price,
                l1Size: _l1Size,

                l2Price: _l2Price,
                l2Size: _l2Size,

                gap: _gap
            );

            return result;
        }

        private IAnalisys Get_BL1LB1_Analisys()
        {
            double _startPrice, _closePrice, _startSize, _closeSize;
            double _r1_b1Price, _r1_b1Size, _r2_b1Price, _r2_b1Size, _r1_l1Price, _r1_l1Size, _r2_l1Price, _r2_l1Size;
            double _r1_b2Size, _r1_b3Size, _r1_l2Size, _r1_l3Size;

            int _gap = getGap_seekingForward(ExPricesRunner0.AvailableToBack[0], ExPricesRunner0.AvailableToLay[0]);

            //ставки открытия - закрытия
            _startPrice = ExPricesRunner0.AvailableToLay[0].Price;
            _startSize = ExPricesRunner0.AvailableToLay[0].Size;

            _closePrice = ExPricesRunner0.AvailableToBack[0].Price;
            _closeSize = ExPricesRunner0.AvailableToBack[0].Size;

            //данные для отрисовки интерфейса
            _r1_b1Price = ExPricesRunner0.AvailableToBack[0].Price;
            _r1_b1Size = ExPricesRunner0.AvailableToBack[0].Size;

            _r2_b1Price = ExPricesRunner1.AvailableToBack[0].Price;
            _r2_b1Size = ExPricesRunner1.AvailableToBack[0].Size;

            _r1_l1Price = ExPricesRunner0.AvailableToLay[0].Price;
            _r1_l1Size = ExPricesRunner0.AvailableToLay[0].Size;

            _r2_l1Price = ExPricesRunner1.AvailableToLay[0].Price;
            _r2_l1Size = ExPricesRunner1.AvailableToLay[0].Size;

            //данные для расчета Sum и Total
            _r1_b2Size = ExPricesRunner0.AvailableToBack[1].Size;
            _r1_b3Size = ExPricesRunner0.AvailableToBack[2].Size;

            _r1_l2Size = ExPricesRunner0.AvailableToLay[1].Size;
            _r1_l3Size = ExPricesRunner0.AvailableToLay[2].Size;

            IAnalisys result = new HorizontalAnalisys
            (
                startRunnerID: Runner0Name,
                closeRunnerID: Runner1Name,

                startPrice: _startPrice,
                closePrice: _closePrice,

                startSize: _startSize,
                closeSize: _closeSize,

                maxObligation: MaxObligation,

                r1_b1Price: _r1_b1Price,
                r1_b1Size: _r1_b1Size,

                r2_b1Price: _r2_b1Price,
                r2_b1Size: _r2_b1Size,

                r1_l1Price: _r1_l1Price,
                r1_l1Size: _r1_l1Size,

                r2_l1Price: _r2_l1Price,
                r2_l1Size: _r2_l1Size,

                gap: _gap,

                r1_b2Size: _r1_b2Size,
                r1_b3Size: _r1_b3Size,

                r1_l2Size: _r1_l2Size,
                r1_l3Size: _r1_l3Size
            );

            return result;
        }

        private IAnalisys Get_BL2LB2_Analisys()
        {
            double _startPrice, _closePrice, _startSize, _closeSize;
            double _r1_b1Price, _r1_b1Size, _r2_b1Price, _r2_b1Size, _r1_l1Price, _r1_l1Size, _r2_l1Price, _r2_l1Size;
            double _r1_b2Size, _r1_b3Size, _r1_l2Size, _r1_l3Size;

            int _gap = getGap_seekingForward(ExPricesRunner1.AvailableToBack[0], ExPricesRunner1.AvailableToLay[0]);

            //ставки открытия - закрытия
            _startPrice = ExPricesRunner1.AvailableToLay[0].Price;
            _startSize = ExPricesRunner1.AvailableToLay[0].Size;

            _closePrice = ExPricesRunner1.AvailableToBack[0].Price;
            _closeSize = ExPricesRunner1.AvailableToBack[0].Size;

            //данные для отрисовки интерфейса
            _r1_b1Price = ExPricesRunner0.AvailableToBack[0].Price;
            _r1_b1Size = ExPricesRunner0.AvailableToBack[0].Size;

            _r2_b1Price = ExPricesRunner1.AvailableToBack[0].Price;
            _r2_b1Size = ExPricesRunner1.AvailableToBack[0].Size;

            _r1_l1Price = ExPricesRunner0.AvailableToLay[0].Price;
            _r1_l1Size = ExPricesRunner0.AvailableToLay[0].Size;

            _r2_l1Price = ExPricesRunner1.AvailableToLay[0].Price;
            _r2_l1Size = ExPricesRunner1.AvailableToLay[0].Size;

            //данные для расчета Sum и Total
            _r1_b2Size = ExPricesRunner1.AvailableToBack[1].Size;
            _r1_b3Size = ExPricesRunner1.AvailableToBack[2].Size;

            _r1_l2Size = ExPricesRunner1.AvailableToLay[1].Size;
            _r1_l3Size = ExPricesRunner1.AvailableToLay[2].Size;

            IAnalisys result = new HorizontalAnalisys
            (
                startRunnerID: Runner0Name,
                closeRunnerID: Runner1Name,

                startPrice: _startPrice,
                closePrice: _closePrice,

                startSize: _startSize,
                closeSize: _closeSize,

                maxObligation: MaxObligation,

                r1_b1Price: _r1_b1Price,
                r1_b1Size: _r1_b1Size,

                r2_b1Price: _r2_b1Price,
                r2_b1Size: _r2_b1Size,

                r1_l1Price: _r1_l1Price,
                r1_l1Size: _r1_l1Size,

                r2_l1Price: _r2_l1Price,
                r2_l1Size: _r2_l1Size,

                gap: _gap,

                r1_b2Size: _r1_b2Size,
                r1_b3Size: _r1_b3Size,

                r1_l2Size: _r1_l2Size,
                r1_l3Size: _r1_l3Size
            );

            return result;
        }

        private int getGap_seekingForward(PriceSize b1, PriceSize l1)
        {
            int result = 0;
            PriceSize tempPrice = b1.GetNextPriceSize();

            while (tempPrice.Price != l1.Price)
            {
                result++;
                tempPrice = tempPrice.GetNextPriceSize();
            }

            return result;
        }

        private int getGap_seekingBack(PriceSize l1, PriceSize b1)
        {
            int result = 0;
            PriceSize tempPrice = l1.GetPreviousPriceSize();

            while (tempPrice.Price != b1.Price)
            {
                result++;
                tempPrice = tempPrice.GetPreviousPriceSize();
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            MarketBookInformation other = obj as MarketBookInformation;

            if(other == null) return false;

            bool runners0AreEqual = ExPricesRunner0.Equals(other.ExPricesRunner0);
            bool runners1AreEqual = ExPricesRunner1.Equals(other.ExPricesRunner1);

            return runners0AreEqual && runners1AreEqual;
        }

        public static bool operator ==(MarketBookInformation ep1, MarketBookInformation ep2)
        {
            if(object.ReferenceEquals(ep1, null))
                return object.ReferenceEquals(ep2, null);

            return ep1.Equals(ep2);
        }

        public static bool operator !=(MarketBookInformation ep1, MarketBookInformation ep2)
        {
            return !(ep1 == ep2);
        }

        public override int GetHashCode()
        {
            return new { ExPricesRunner0, ExPricesRunner1 }.GetHashCode();
        }
    }

}
