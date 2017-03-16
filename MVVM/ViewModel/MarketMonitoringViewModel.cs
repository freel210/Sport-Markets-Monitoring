using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.ComponentModel;
using Model;
using CommonLibrary;

namespace ViewModel
{
    public sealed class MarketMonitoringViewModel : NotifyPropertyChanger
    {
        private readonly MarketMonitoring_ProxyModel _marketMonitoring_ProxyModel;
        private readonly PropertyChangedEventHandler _marketChangeHandler;

        private MarketInformation _market;
        public MarketInformation Market
        {
            get { return _market;}

            set
            {
                if (_market == value) return; 
                
                //отписываемся от изменений внутри Маркета
                if (_market != null)
                    _market.PropertyChanged -= _marketChangeHandler;

                _market = value;
                
                //подписываемся на изменения внутри маркета
                if (_market != null)
                    _market.PropertyChanged += _marketChangeHandler;

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

        private int _currentRecord;
        public int CurrentRecord
        {
            get { return _currentRecord;}

            set
            {
                if (_currentRecord == value) return;

                _currentRecord = value;
                OnPropertyChanged();
            }
        }

        private void RiseCurrentMarketMemberChange()
        {
            CurrentMarketMemberChange = !CurrentMarketMemberChange;
        }

        string _marketID;
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

        private string _currentCase = "BL1B2";
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
        private string _timeLeft = string.Empty;
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

        public MarketMonitoringViewModel(MarketInformation marketInformation, DispatcherTimer oneSecIntervalTimer, bool history)
        {
            //установка обработчика событий изменения внутри маркета
            _marketChangeHandler = (sender, e) => RiseCurrentMarketMemberChange();

            MarketID = marketInformation.MarketId;
            Market   = marketInformation;

            //счетчик для вычесления оставшегося до старта рынка времени
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

            //в случае, если окно открыто для посмотра исторических данных,
            //запускаем событие для прорисовки данных в контроле
            //иначе - запускаем WF для мониторинга
            if (history)
                RiseCurrentMarketMemberChange();
            else
                _marketMonitoring_ProxyModel = new MarketMonitoring_ProxyModel(Market);

        }

        internal void ViewClosed(object sender, EventArgs e)
        {
            _marketMonitoring_ProxyModel.CancelMarketMonitoring();
        }
    }
}
