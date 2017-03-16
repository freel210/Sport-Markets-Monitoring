using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using System.IO;
using System.ComponentModel;

namespace MVVM
{
    internal sealed class ViewModel : NotifyPropertyChanger
    {
        
        #region Свойства
        private double _progress;
        public double Progress
        {
            get { return _progress; }
            set
            {
                if (_progress != value)
                    _progress = value;

                OnPropertyChanged();
            }
        }
      
        public ObservableDictionary<string, MarketInformation> MarketInformationDict
        {
            get { return model.MarketInformationDict; }
        }

        private bool _clearSheet;
        public bool ClearSheet
        {
            get { return _clearSheet; }
            set
            {
                if(_clearSheet != value)
                    _clearSheet = value;
                
                OnPropertyChanged();
            }
        }

        private bool _accountInfoWindowButtonIsCheked;
        public bool AccountInfoWindowButtonIsCheked
        {
            get { return _accountInfoWindowButtonIsCheked; }
            set
            {
                if (_accountInfoWindowButtonIsCheked != value)
                    _accountInfoWindowButtonIsCheked = value;

                OnPropertyChanged();
            }
        }

        public string CurrentMarketID
        {
            get { return model.CurrentMarketID; }

            set
            {
                model.CurrentMarketID = value;
                OnPropertyChanged();
            }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                    _status = value;

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

        //нужен ли CurrentCase в модели? Там же все варианты должны быть посчитаны, а выбрать "один из" можно и здесь
        public string CurrentCase
        {
            get { return model.CurrentCase; }

            set
            {
                model.CurrentCase = value;
                OnPropertyChanged();
            }
        }

        private readonly PropertyChangedEventHandler _currentMarketChangeHandler;
        public MarketInformation CurrentMarket
        {
            get { return model.CurrentMarket; }

            set
            {
                if (model.CurrentMarket == value) return;

                if (model.CurrentMarket != null)
                    model.CurrentMarket.PropertyChanged -= _currentMarketChangeHandler;

                model.CurrentMarket = value;

                if (model.CurrentMarket != null)
                    model.CurrentMarket.PropertyChanged += _currentMarketChangeHandler;

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

        public ObservableDictionary<string, Tuple<MarketMonitoringWindow, MarketMonitoringViewModel>> MarketsAreInMonitoring { get; set; }
        #endregion

        #region Поля

        private readonly MainModel model;
        private readonly DispatcherTimer OneSecIntervalTimer;
        private readonly object lockObj = new object();

        private bool _getMarketDataFlag    = true;
        private bool _updateMarketDataFlag = false;
        private bool _clearSheetFlag       = false;

        private readonly ICommand commandDataFromDF;
        private readonly ICommand commandClearSheet;
        private readonly ICommand commandGetAccountInfo;
        private readonly ICommand commandUpdatePrice;
        private readonly ICommand commandMarketMonitor;
        private readonly ICommand commandLoadMarketMonitoringReport;

        #endregion

        #region Команды
        public ICommand DataFromBFCommand
        {
            get { return commandDataFromDF; }
        }

        public ICommand ClearSheetCommand
        {
            get { return commandClearSheet;}
        }

        public ICommand GetAccountInfoCommand
        {
            get { return commandGetAccountInfo; }
        }
        
        public ICommand UpdatePriceCommand
        {
            get { return commandUpdatePrice; }
        }

        public ICommand MarketMonitorCommand
        {
            get { return commandMarketMonitor; }
        }

        public ICommand LoadMarketMonitoringReportCommand
        {
            get { return commandLoadMarketMonitoringReport; }
        }
        #endregion
        
        public ViewModel(MainModel m)
        {
            model = m;
            MarketsAreInMonitoring = new ObservableDictionary<string, Tuple<MarketMonitoringWindow, MarketMonitoringViewModel>>();

            model.PropertyChanged += (sender, e) => OnPropertyChanged(e.PropertyName);

            commandDataFromDF          = new DelegateCommand(o => CanExecuteDataFromBF(),          o => ExecuteDataFromBF());
            commandClearSheet          = new DelegateCommand(o => CanExecuteClearSheet(),          o => ExecuteClearSheet());
            commandGetAccountInfo      = new DelegateCommand(o => CanExecuteGetAccountInfo(),      o => ExecuteGetAccountInfo());
            commandUpdatePrice         = new DelegateCommand(o => CanExecuteUpdatePrice(),         o => ExecuteUpdatePrice());
            commandMarketMonitor       = new DelegateCommand(o => CanExecuteMarketMonitor(),       o => ExecuteMarketMonitor());
            commandLoadMarketMonitoringReport = new DelegateCommand(o => CanExecuteLoadMarketMonitoringReport(), o => ExecuteLoadMarketMonitoringReport());

            OneSecIntervalTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(1000)};
            OneSecIntervalTimer.Tick += (s, e) =>
            {
                if (CurrentMarket?.MarketName == null)
                    TimeLeft = "no";
                else
                {
                    TimeSpan dateDifference = CurrentMarket.StartMarketTime.Subtract(DateTime.Now);
                    TimeLeft = dateDifference.ToString(@"hh\:mm\:ss");
                }
            };
            OneSecIntervalTimer.Start();

            Status = "Ок.";
            Progress = 0;

            _currentMarketChangeHandler = (sender, e) => _riseCurrentMarketMemberChange();
        }

        #region CanExecute
        private bool CanExecuteDataFromBF()
        {
            return _getMarketDataFlag;
        }
        
        private bool CanExecuteClearSheet()
        {
            return _clearSheetFlag;
        }
       
        private bool CanExecuteGetAccountInfo()
        {
            return true;
        }

        private bool CanExecuteUpdatePrice()
        {
            return _updateMarketDataFlag;
        }
        
        private bool CanExecuteMarketMonitor()
        {
            bool emptyCurrentMarketID = CurrentMarketID == "";
            bool alreadyInMonitoring = MarketsAreInMonitoring.ContainsKey(CurrentMarketID);

            return !emptyCurrentMarketID && !alreadyInMonitoring;
        }

        private bool CanExecuteLoadMarketMonitoringReport()
        {
            return true;
        }
        #endregion

        #region Execute
        private void ExecuteDataFromBF()
        {
            ExecuteClearSheet();
            Status = "Получение рынков...";

            var ts = new CancellationTokenSource();
            CancellationToken ct = ts.Token;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            
            Task.Run(() =>
            {
                _getMarketDataFlag = false;
                Progress = 0;

                try
                {
                    model.FillMarketListWithDetailedInformation(ct);
                    Status = "Ок.";
                }
                catch(Exception e)
                {
                    Status = "Прервано.";
                    Progress = 100;
                    MessageBox.Show(e.ToString(), e.Message);
                }
                finally
                {
                    ts.Dispose();
                }

                _updateMarketDataFlag = true;
                _getMarketDataFlag    = true;
                _clearSheetFlag       = true;

                lock (lockObj)
                {
                    timer.Stop();
                    Progress = 100;
                }
            }, ct);

            timer.Tick += (s, e) =>
            {
                lock (lockObj)
                    Progress++;

                if (Progress == 100)
                {
                    timer.Stop();
                    ts.Cancel();
                }
            };
            timer.Start();
        }

        private void ExecuteClearSheet()
        {
            _updateMarketDataFlag = false;
            _clearSheetFlag = false;
            ClearSheet = !ClearSheet; //просто меняем значение
        }

        private AccountInfoWindow _win;
        private void ExecuteGetAccountInfo()
        {
            if (AccountInfoWindowButtonIsCheked)
            {
                _win = new AccountInfoWindow();
                _win.DataContext = new AccountInfoViewModel(model);
                _win.Closed += (sender, e) => AccountInfoWindowButtonIsCheked = false;

                _win.Show();
            }
            else
                _win.Close();
        }
        
        private void ExecuteUpdatePrice()
        {
            CurrentMarket = null;
            CurrentMarket = model.UpdateMarket();
        }

        private void ExecuteMarketMonitor()
        {
            if (MarketsAreInMonitoring.ContainsKey(CurrentMarketID))
                return;
            
            MarketMonitoringWindow win = new MarketMonitoringWindow();

            MarketInformation m = model.MarketInformationDict[CurrentMarketID];
            MarketMonitoringViewModel vm = new MarketMonitoringViewModel(CurrentMarketID, m, OneSecIntervalTimer);

            win.DataContext = vm;
            
            //добавляем рынок в список окон мониторинга
            MarketsAreInMonitoring.Add(CurrentMarketID, new Tuple<MarketMonitoringWindow, MarketMonitoringViewModel>(win, vm));

            //при закрытии окна удаляем CurrentMarketID из списка мониторинга
            win.Closed += (sender, e) =>
            {
                MarketMonitoringWindow window = sender as MarketMonitoringWindow;

                string s = string.Empty;
                foreach (var item in MarketsAreInMonitoring)
                {
                    if (item.Value.Item1 != window) continue;

                    s = item.Key;
                    break;
                }

                if (s == string.Empty) return;

                MarketsAreInMonitoring[s].Item2.Dispose();
                MarketsAreInMonitoring.Remove(s);
            };
            
            win.Show();
        }

        private void ExecuteLoadMarketMonitoringReport()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            string startupPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string initialDirectory = startupPath + @"\View\Icons";
            dlg.InitialDirectory = initialDirectory;

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "Market monitoring reports | *.png";

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result != true) return;

            // Open document 
            string filename = dlg.FileName;

            MarketMonitoringWindow win = new MarketMonitoringWindow
            {
                DataContext =
                    new MarketMonitoringViewModel(CurrentMarketID, model.MarketInformationDict[CurrentMarketID],
                        OneSecIntervalTimer)
            };

            win.Show();
        }
        #endregion
    }
}
