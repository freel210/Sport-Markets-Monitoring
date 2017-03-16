using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Model.Analisys;
using Model;
using View.Converters;
using CommonLibrary;
using ViewModel;
using System.Collections.ObjectModel;

namespace View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    sealed partial class MarketDetailedList_BC : UserControl
    {
        private readonly TextBlock[,] Grid;

        private readonly MarketInformation blankMI = new MarketInformation();

        private readonly int Row = 12;

        private readonly SolidColorBrush SelectedBackGroundBrush = Brushes.Bisque;
        private readonly SolidColorBrush UnSelectedBackGroundBrush = Brushes.White;

        private Tuple<TextBlock, TextBlock> _drawSelectedRow;

        public MarketDetailedList_BC()
        {
            InitializeComponent();

            Grid = new TextBlock[Row, 2];
            for (int i = 0; i < Row; i++)
            {
                Grid[i, 0] = FindName("blc_" + i + "_0") as TextBlock;
                Grid[i, 1] = FindName("blc_" + i + "_1") as TextBlock;
            }

            _drawSelectedRow = new Tuple<TextBlock, TextBlock>(Grid[0, 0], Grid[0, 1]);
        }

        #region MarketInformationDictProperty
        public static readonly DependencyProperty MarketInformationDictProperty =
            DependencyProperty.Register("MarketInformationDict", typeof(ObservableCollection<MarketInformation>), typeof(MarketDetailedList_BC),
                new FrameworkPropertyMetadata(null, MarketListWithDetailsOnChanged));

        public ObservableCollection<MarketInformation> MarketInformationDict
        {
            get 
            {
                return (ObservableCollection<MarketInformation>)this.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    (DispatcherOperationCallback)delegate { return (ObservableCollection<MarketInformation>)GetValue(MarketInformationDictProperty); },
                    MarketInformationDictProperty);
            }
            
            set 
            {
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    (SendOrPostCallback)delegate { this.SetValue(MarketInformationDictProperty, value); },
                    value);
            }
        }

        private static void MarketListWithDetailsOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            MarketDetailedList_BC ml = doj as MarketDetailedList_BC;
            ml.MarketInformationDict.CollectionChanged += (sender, e) => ml.UpdateGrid();

            //ml.UpdateGrid();
        }

        private void UpdateGrid()
        {           
            SolidColorBrush BlueColor = Brushes.Blue;
            SolidColorBrush BlackColor = Brushes.Black;
            SolidColorBrush PurpleColor = Brushes.Purple;

            int i = 0;
            List<string> keys = MarketInformationDict.Select(x => x.MarketId).ToList();
            foreach (string key in keys)
            {
                string suffix;
                if (MarketsAreInMonitoring.ContainsKey(key) && key != "")
                    suffix = " *";
                else
                    suffix = "";

                var i1 = i;
                Grid[i, 0].Dispatcher.Invoke(() => Grid[i1, 0].Text = key + suffix);

                var m = MarketInformationDict.Where(x => x.MarketId == key).FirstOrDefault();
                int idx = MarketInformationDict.IndexOf(m);
                var a = MarketInformationDict[idx].Mbi.Last().CasesAnalisys;

                Gaps gaps = Gaps.empty;

                if (a[TradeCases.BL1B2].Gap == 0 && a[TradeCases.BL2B1].Gap == 0)
                    gaps = Gaps.none;

                if (gaps != Gaps.none && a[TradeCases.BL1B2].Gap < 2 && a[TradeCases.BL2B1].Gap < 2)
                    gaps = Gaps.one;

                if (gaps == Gaps.empty)
                    gaps = Gaps.more;

                switch (gaps)
                {
                    case Gaps.one:
                        var i2 = i;
                        Grid[i, 1].Dispatcher.Invoke(() => Grid[i2, 1].Foreground = BlueColor);
                        break;
                    
                    case Gaps.more:
                        var i3 = i;
                        Grid[i, 1].Dispatcher.Invoke(() => Grid[i3, 1].Foreground = PurpleColor);
                        break;
                    
                    default:
                        var i4 = i;
                        Grid[i, 1].Dispatcher.Invoke(() => Grid[i4, 1].Foreground = BlackColor);
                        break;
                }

                string str = MarketInformationDict[idx].TotalMatched.ToString("N0", CultureInfo.CurrentCulture);
                var i5 = i;
                Grid[i, 1].Dispatcher.Invoke(() => Grid[i5, 1].Text = str);

                i++;
            }
        }
        #endregion

        #region MarketsAreInMonitoringProperty
        public static readonly DependencyProperty MarketsAreInMonitoringProperty =
            DependencyProperty.Register("MarketsAreInMonitoring", typeof(ObservableDictionary<string, Tuple<MarketMonitoringWindow, MarketMonitoringViewModel>>), typeof(MarketDetailedList_BC),
                new FrameworkPropertyMetadata(null, MarketsAreInMonitoringOnChanged));

        public ObservableDictionary<string, Tuple<MarketMonitoringWindow, MarketMonitoringViewModel>> MarketsAreInMonitoring
        {
            get
            {
                return (ObservableDictionary<string, Tuple<MarketMonitoringWindow, MarketMonitoringViewModel>>)this.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    (DispatcherOperationCallback)delegate { return (ObservableDictionary<string, Tuple<MarketMonitoringWindow, MarketMonitoringViewModel>>)GetValue(MarketsAreInMonitoringProperty); },
                    MarketsAreInMonitoringProperty);
            }

            set
            {
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    (SendOrPostCallback)delegate { this.SetValue(MarketsAreInMonitoringProperty, value); },
                    value);
            }
        }

        private static MarketDetailedList_BC ml;
        private static void MarketsAreInMonitoringOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            ml = doj as MarketDetailedList_BC;
            ml.MarketsAreInMonitoring.CollectionChanged += (sender, e) => ml.UpdateGrid();

            //ml.UpdateGrid();
        }
        #endregion

        #region CurrentMarketProperty
        public static readonly DependencyProperty CurrentMarketProperty =
            DependencyProperty.Register("CurrentMarket", typeof(MarketInformation), typeof(MarketDetailedList_BC),
                new FrameworkPropertyMetadata(null, null));

        public MarketInformation CurrentMarket
        {
            get { return (MarketInformation)GetValue(CurrentMarketProperty); }
            set { SetValue(CurrentMarketProperty, value); }
        }
        #endregion
       
        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock blc = sender as TextBlock;
            DrawSelectedRow = GetTextBlockRow(blc);
        }

        private Tuple<TextBlock, TextBlock> GetTextBlockRow(TextBlock blc)
        {
            for (int i = 0; i < Row; i++)
            {
                if (Grid[i, 0] == blc)
                    return new Tuple<TextBlock, TextBlock>(blc, Grid[i, 1]);

                if (Grid[i, 1] == blc)
                    return new Tuple<TextBlock, TextBlock>(Grid[i, 0], blc);
            }            
            
            return new Tuple<TextBlock, TextBlock>(Grid[0, 0], Grid[0, 1]); //не должен сюда дойти
        }

        private Tuple<TextBlock, TextBlock> DrawSelectedRow
        {
            set
            {
                if (value != _drawSelectedRow)
                {
                    //стереть предыдущее выделение строки
                    _drawSelectedRow.Item1.Background = UnSelectedBackGroundBrush;
                    _drawSelectedRow.Item2.Background = UnSelectedBackGroundBrush;

                    //выделить новую строку
                    value.Item1.Background = SelectedBackGroundBrush;
                    value.Item2.Background = SelectedBackGroundBrush;

                    //установить новое значение свойства CurrentMarket
                    string currentMarketID = RemoveSuffixIfExist(value.Item1.Text);
                    var m = MarketInformationDict.Where(x => x.MarketId == currentMarketID).FirstOrDefault();
                    int idx = MarketInformationDict.IndexOf(m);

                    if(idx >= 0)
                        CurrentMarket = MarketInformationDict[idx];
                    else
                        CurrentMarket = blankMI;

                    //обновить значение текущей строки
                    _drawSelectedRow = value;
                }
            }
        }

        private string RemoveSuffixIfExist(string s)
        {
            if (s.Length - 2 < 0)
                return s;

            string SubString = s.Substring(s.Length - 2);
            
            if (SubString == " *")
                return s.Substring(0, s.Length - 2);
            
            return s;
        }
        
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                TextBlock_PreviewMouseDown(sender, null);
        }
    }
}
