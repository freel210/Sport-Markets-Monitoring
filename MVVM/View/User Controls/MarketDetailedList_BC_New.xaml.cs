using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Model;
using System;
using System.Collections.ObjectModel;

namespace View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для MarketDetailedList_BC_New.xaml
    /// </summary>
    sealed partial class MarketDetailedList_BC_New : UserControl
    {
        private readonly MarketInformation blankMI = new MarketInformation();

        private readonly SolidColorBrush SelectedBackGroundBrush = Brushes.Bisque;
        private readonly SolidColorBrush UnSelectedBackGroundBrush = Brushes.White;

        public MarketDetailedList_BC_New()
        {
            InitializeComponent();            
        }

        #region MarketInformationDictProperty
        public static readonly DependencyProperty MarketInformationDictProperty =
            DependencyProperty.Register("MarketInformationDict",
                typeof(ObservableCollection<MarketInformation>),
                typeof(MarketDetailedList_BC_New),
                new FrameworkPropertyMetadata(null, MarketListWithDetailsOnChanged));

        public ObservableCollection<MarketInformation> MarketInformationDict
        {
            get
            {
                return (ObservableCollection<MarketInformation>)this.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    (DispatcherOperationCallback)delegate
                    {
                        return (ObservableCollection<MarketInformation>)GetValue(MarketInformationDictProperty);
                    },
                    MarketInformationDictProperty);
            }

            set
            {
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    (SendOrPostCallback)delegate { SetValue(MarketInformationDictProperty, value); },
                    value);
            }
        }

        private static void MarketListWithDetailsOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            MarketDetailedList_BC_New ml = doj as MarketDetailedList_BC_New;
            ml.MarketInformationDict.CollectionChanged += (sender, e) =>
            {
                //из-за этого мигает
                ml.Dyorg();
            };
        }

        #endregion

        #region CurrentMarketProperty
        public static readonly DependencyProperty CurrentMarketProperty =
            DependencyProperty.Register("CurrentMarket", typeof(MarketInformation), typeof(MarketDetailedList_BC_New),
                new FrameworkPropertyMetadata(null, CurrentMarketOnChanged));

        public MarketInformation CurrentMarket
        {
            get { return (MarketInformation)GetValue(CurrentMarketProperty); }
            set { SetValue(CurrentMarketProperty, value); }
        }

        private static void CurrentMarketOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            MarketDetailedList_BC_New ml = doj as MarketDetailedList_BC_New;

            //этот метод также срабатывает и когда мы выделяем мышкой. Чтобы дважды не устанавливать строку,
            //мы делаем проверку на выделение мышью
            if(ml.dg.SelectedItem == ml.CurrentMarket) return;

            ml.dg.SelectedItem = ml.CurrentMarket;
        }

        #endregion

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            CurrentMarket = dg.SelectedItem as MarketInformation;
        }

        private void Dyorg()
        {
            dg.ItemsSource = null;
            dg.ItemsSource = MarketInformationDict;

            //Dispatcher.BeginInvoke((Action)(() => dg.ItemsSource = null));
            //Dispatcher.BeginInvoke((Action)(() => dg.ItemsSource = MarketInformationDict));
        }
    }
}
