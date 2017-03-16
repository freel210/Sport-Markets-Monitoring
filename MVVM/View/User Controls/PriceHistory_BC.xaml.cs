using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Model.Analisys;
using Model;

namespace View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для PriceHistory.xaml
    /// </summary>
    public sealed partial class PriceHistory_BC : UserControl
    {
        private readonly ObservableCollection<PriceHistoryData> _priceHistoryDataSource;
        public ObservableCollection<PriceHistoryData> PriceHistoryDataSource
        {
            get { return _priceHistoryDataSource; }
        }

        public PriceHistory_BC()
        {
            InitializeComponent();

            _priceHistoryDataSource = new ObservableCollection<PriceHistoryData>();

            dg.ItemsSource = PriceHistoryDataSource;
        }

        #region CurrentMarketProperty
        public static readonly DependencyProperty CurrentMarketProperty =
            DependencyProperty.Register("CurrentMarket", typeof(MarketInformation), typeof(PriceHistory_BC),
                new FrameworkPropertyMetadata(null, CurrentMarketOnChanged));

        public MarketInformation CurrentMarket
        {
            get { return (MarketInformation)GetValue(CurrentMarketProperty); }
            set { SetValue(CurrentMarketProperty, value); }
        }

        private static void CurrentMarketOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            PriceHistory_BC uc3 = doj as PriceHistory_BC;

            if (uc3 == null || uc3.CurrentMarket == null) return;

        }
        #endregion

        #region CurrentMarketMemberChangeProperty

        public static readonly DependencyProperty CurrentMarketMemberChangeProperty =
            DependencyProperty.Register("CurrentMarketMemberChange", typeof(bool), typeof(PriceHistory_BC),
                new FrameworkPropertyMetadata(false, OnChangedCurrentMarketMemberChange));

        public bool CurrentMarketMemberChange
        {
            get { return (bool)GetValue(CurrentMarketMemberChangeProperty); }
            set { SetValue(CurrentMarketMemberChangeProperty, value); }
        }

        private static void OnChangedCurrentMarketMemberChange(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            PriceHistory_BC uc1 = doj as PriceHistory_BC;

            //первоначальная инициализация (в конструкторе рушится, видимо свойства зависимости на том этапе еще не созданы)
            if (uc1?._priceHistoryDataSource.Count == 0 && uc1.CurrentMarket.Mbi.Count > 0)
            {
                for (int i = 1; i < uc1.CurrentMarket.Mbi.Count; i++) //начинаем с i = 1, иначе надо проверять на s.Dt != "0:00:00"
                {
                    PriceHistoryData s = uc1.GetPriceHistoryDataFromCurrentMarcetByIndex(i);
                    //if (s.Dt != "0:00:00") //  в записанном файле первая запись "как бы" такая (хотя это и не так). Можно открыть и убедиться
                                             // Если не нее потом щелкать - все рушится.
                    uc1._priceHistoryDataSource.Add(s);
                }
            }
            else if (uc1?.CurrentMarket != null)
            {
                int i = uc1.CurrentMarket.Mbi.Count - 1;
                PriceHistoryData s = uc1.GetPriceHistoryDataFromCurrentMarcetByIndex(i);
                uc1._priceHistoryDataSource.Add(s);
            }

            //если не стоит галка, то выделяем последнюю запись (ReSharper зачем-то еще на null проверил... ну и пусть оно будет)
            if (uc1?.CurrentMarket != null && uc1.cb.IsChecked != null && (bool)uc1.cb.IsChecked)
                uc1.MoveToLast();
        }

        private void MoveToLast()
        {
            if (PriceHistoryDataSource == null) return;
            var item = PriceHistoryDataSource.LastOrDefault();
            if(item == null) return;

            dg.SelectedItem = item;
            dg.UpdateLayout();
            dg.ScrollIntoView(item);
        }

        private PriceHistoryData GetPriceHistoryDataFromCurrentMarcetByIndex(int i)
        {
            PriceHistoryData data = new PriceHistoryData();
            data.Number   = i; 
            data.Dt       = CurrentMarket.Mbi[i].LastByte.ToLongTimeString();
            data.BestCase = CurrentMarket.Mbi[i].BestCase == TradeCases.empty ? string.Empty : "Ok";

            return data;
        }
        #endregion

        #region CurentRecordProperty
        public static readonly DependencyProperty CurrentRecordProperty =
            DependencyProperty.Register("CurrentRecord", typeof(int), typeof(PriceHistory_BC),
                new FrameworkPropertyMetadata(-1, null));

        public int CurrentRecord
        {
            get { return (int)GetValue(CurrentRecordProperty); }
            set { SetValue(CurrentRecordProperty, value); }
        }
        #endregion

        //событие установки галки
        private void cb_Checked(object sender, RoutedEventArgs e)
        {
            if(cb.IsChecked != null) MoveToLast();
        }

        //выбор новой записи
        private void dg_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //сделать так, чтобы реагировало только на щелчки по нужным столбцам?
            cb.IsChecked = false;
        }

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var priceHistoryData = dg.SelectedItem as PriceHistoryData;
            if (priceHistoryData != null)
            {
                CurrentRecord = priceHistoryData.Number;
                //tb.Text = CurrentRecord.ToString(); // TEST
            }
        }
    }

    public sealed class PriceHistoryData
    {
        public int Number { get; set; }
        public string Dt { get; set; }
        public string BestCase { get; set; }
    }
}
