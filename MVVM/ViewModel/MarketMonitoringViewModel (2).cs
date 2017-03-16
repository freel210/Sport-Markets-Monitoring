using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.ComponentModel;

namespace MVVM
{
    public sealed class MarketMonitoringViewModel : NotifyPropertyChanger, IDisposable
    {
        private readonly MarketControlModel _marketControlModel;
        private readonly PropertyChangedEventHandler _marketChangeHandler;
        public MarketInformation Market
        {
            get { return _marketControlModel.CurrentMarket; }

            set
            {
                OnPropertyChanged();
            }
        }

        private bool _currentMarketMemberChange;
        public bool CurrentMarketMemberChange
        {
            get { return _currentMarketMemberChange; }

            set
            {
                if (_currentMarketMemberChange == value) return;

                _currentMarketMemberChange = value;
                OnPropertyChanged();
            }
        }

        private void _riseCurrentMarketMemberChange()
        {
            CurrentMarketMemberChange = !CurrentMarketMemberChange;
        }

        public void Dispose()
        {
            _marketControlModel.StopMonitoring();
        }

        private string _marketID;
        public string MarketID
        {
            get
            {
                return _marketID;
            }
            
            set
            {
                if (_marketID != value)
                    _marketID = value;

                OnPropertyChanged();
            }
        }

        private string _currentCase = string.Empty;
        public string CurrentCase
        {
            get
            {
                return _currentCase;
            }

            set
            {
                if (_currentCase != value)
                    _currentCase = value;

                OnPropertyChanged();
            }
        }

        //расчетное свойство, в модели отсутствует
        private string _timeLeft = String.Empty;
        public string TimeLeft
        {
            get { return _timeLeft; }

            set
            {
                if (_timeLeft != value)
                    _timeLeft = value;
                OnPropertyChanged();
            }
        }

        public MarketMonitoringViewModel(string marketID, MarketInformation marketInformation, DispatcherTimer oneSecIntervalTimer)
        {
            //установка обработчика событий изменения внутри маркета
            _marketChangeHandler = (sender, e) => _riseCurrentMarketMemberChange();

            MarketID = marketID;
            _marketControlModel = new MarketControlModel(marketInformation, marketID);
            _marketControlModel.CurrentMarket.PropertyChanged += (sender, e) => Market = new MarketInformation(); //любое присвоение, просто чтобы сработало OnPropertyChanged

            oneSecIntervalTimer.Tick += (s, e) =>
            {
                if (Market?.MarketName == null)
                    TimeLeft = "no";
                else
                {
                    TimeSpan dateDifference = Market.StartMarketTime.Subtract(DateTime.Now);
                    TimeLeft = dateDifference.ToString(@"hh\:mm\:ss");
                }
            };          
        }
    }
}
