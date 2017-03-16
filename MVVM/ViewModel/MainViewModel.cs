using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using Model;
using View;
using CommonLibrary;
using System.Collections.ObjectModel;

namespace ViewModel
{
    internal sealed class MainViewModel : NotifyPropertyChanger
    {
        private const bool THIS_IS_HISTORY = true;
        private const bool THIS_IS_NOT_HISTORY = false;

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

        //public SolidColorBrush MarketListForegroundColor
        //{
        //    get
        //    {

        //    }
        //}

        public ObservableCollection<MarketInformation> MarketInformationDict
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

        private string _currentCase = "BL1B2";
        public string CurrentCase
        {
            get { return _currentCase; }

            set
            {
                if(_currentCase == value) return;

                _currentCase = value;
                OnPropertyChanged();
            }
        }

        private readonly PropertyChangedEventHandler _currentMarketChangeHandler;
        private MarketInformation _currentMarket;
        public MarketInformation CurrentMarket
        {
            get { return _currentMarket; }

            set
            {
                if (_currentMarket == value) return;

                if (_currentMarket != null)
                    _currentMarket.PropertyChanged -= _currentMarketChangeHandler;

                _currentMarket = value;

                if (_currentMarket != null)
                    _currentMarket.PropertyChanged += _currentMarketChangeHandler;

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

        #endregion

        #region Поля

        private readonly MainModel model;
        private DispatcherTimer OneSecIntervalTimer;
        private object lockObj = new object();

        private bool _getMarketDataFlag    = true;
        private bool _clearSheetFlag       = false;

        private readonly ICommand commandDataFromDF;
        private readonly ICommand commandClearSheet;
        private readonly ICommand commandGetAccountInfo;
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
        
        public ICommand MarketMonitorCommand
        {
            get { return commandMarketMonitor; }
        }

        public ICommand LoadMarketMonitoringReportCommand
        {
            get { return commandLoadMarketMonitoringReport; }
        }
        #endregion
        
        public MainViewModel(MainModel m)
        {
            model = m;
            _currentMarket = new MarketInformation();

            model.PropertyChanged += (sender, e) => OnPropertyChanged(e.PropertyName);

            commandDataFromDF          = new DelegateCommand(o => CanExecuteDataFromBF(),          o => ExecuteDataFromBF());
            commandClearSheet          = new DelegateCommand(o => CanExecuteClearSheet(),          o => ExecuteClearSheet());
            commandGetAccountInfo      = new DelegateCommand(o => CanExecuteGetAccountInfo(),      o => ExecuteGetAccountInfo());
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
        
        private bool CanExecuteMarketMonitor()
        {
            bool emptyCurrentMarket = CurrentMarket == null
                || CurrentMarket.MarketId == string.Empty || CurrentMarket.MarketId == null;

            if(emptyCurrentMarket) return false;

            return !CurrentMarket.IsInMonitoring;
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
                    CurrentMarket = MarketInformationDict[0];
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
        
        private void ExecuteMarketMonitor()
        {
            var vm = new MarketMonitoringViewModel(CurrentMarket, OneSecIntervalTimer, THIS_IS_NOT_HISTORY);
            var win = new MarketMonitoringWindow();
            win.DataContext = vm;

            //подписываемся на событие закрытия окна, для остановки WF
            win.Closed += vm.ViewClosed;

            win.Show();
        }

        private void ExecuteLoadMarketMonitoringReport()
        {
            try
            {
                //Получаем имя открываемого файла 
                string filename = GetFileName();

                //Если ничего не открыли, уходим из метода
                if (filename == string.Empty) return;

                //Десериализируем
                MarketInformation mi = GetMiFromXML(filename);

                //Создаем и показываем окно
                var vm = new MarketMonitoringViewModel(mi, OneSecIntervalTimer, THIS_IS_HISTORY);
                var win = new MarketMonitoringWindow();
                win.DataContext = vm;
                win.Show();
            }
            catch (Exception e)
            {                
                MessageBox.Show(e.ToString());
            }
        }

        private string GetFileName()
        {
            string filename = string.Empty;
            try
            {
                // Create OpenFileDialog 
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                string currentPath = Directory.GetCurrentDirectory();
                string path = Path.Combine(currentPath, "History Data");

                dlg.InitialDirectory = path;

                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".xml";
                dlg.Filter = "Market monitoring reports | *.xml";

                // Display OpenFileDialog by calling ShowDialog method 
                bool? result = dlg.ShowDialog();

                // Get the selected file name and display in a TextBox 
                if (result == true)
                    filename = dlg.FileName;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return filename;
        }

        private MarketInformation GetMiFromXML(string filename)
        {
            MarketInformation result = default (MarketInformation);

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(filename);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(MarketInformation);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        result = (MarketInformation)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return result;
        }
        #endregion
    }
}
