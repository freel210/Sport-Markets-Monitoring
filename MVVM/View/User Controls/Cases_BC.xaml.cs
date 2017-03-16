using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using Model.Analisys;
using Model;
using MVVM;

namespace View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Cases_BC.xaml
    /// </summary>

    public sealed partial class Cases_BC : UserControl
    {
        private static readonly TradeCases BL1B2 = TradeCases.BL1B2;
        private static readonly TradeCases BL2B1 = TradeCases.BL2B1;
        private static readonly TradeCases LB1L2 = TradeCases.LB1L2;
        private static readonly TradeCases LB2L1 = TradeCases.LB2L1;

        private static readonly TradeCases[] arr = new TradeCases[] { BL1B2, BL2B1, LB1L2, LB2L1 };

        private readonly TextBlock[,] Grid;

        private readonly int Row = 4;

        private readonly SolidColorBrush SelectedBackGroundBrush = Brushes.Bisque;
        private readonly SolidColorBrush UnSelectedBackGroundBrush = Brushes.White;

        public Cases_BC()
        {
            InitializeComponent();

            Grid = new TextBlock[Row, 4];

            string[] cases = new string[] { "bl1b2", "bl2b1", "lb1l2", "lb2l1" };
            for (int i = 0; i < Row; i++)
            {
                Grid[i, 0] = FindName(cases[i])          as TextBlock;
                Grid[i, 1] = FindName(cases[i] + "_tr")  as TextBlock;
                Grid[i, 2] = FindName(cases[i] + "_wr")  as TextBlock;
                Grid[i, 3] = FindName(cases[i] + "_mwr") as TextBlock;
            }
        }

        #region CurrentMarketProperty
        public static readonly DependencyProperty CurrentMarketProperty =
            DependencyProperty.Register("CurrentMarket", typeof(MarketInformation), typeof(Cases_BC),
                new FrameworkPropertyMetadata(null, CurrentMarketOnChanged));

        public MarketInformation CurrentMarket
        {
            get { return (MarketInformation)GetValue(CurrentMarketProperty); }
            set { SetValue(CurrentMarketProperty, value); }
        }

        private static readonly SolidColorBrush BlueColor = Brushes.Blue;
        private static readonly SolidColorBrush GreenColor = Brushes.Green;
        private static readonly SolidColorBrush BlackColor = Brushes.Black;

        private static void CurrentMarketOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            Cases_BC uc3 = doj as Cases_BC;

            if (uc3.CurrentMarket != null)
                uc3.UpdateGrid2();
        }

        private void UpdateGrid2()
        {
            string[] cases = new string[] { "bl1b2", "bl2b1", "lb1l2", "lb2l1" };
            for (int i = 0; i < Row; i++)
            {
                MarketBookInformation mbi;
                if (CurrentRecord == -1 || CurrentMarket.Mbi.Count <= CurrentRecord) // второе условие нужно так как оно, хз почему, иногда не выполняется
                    mbi = CurrentMarket.Mbi.Last();
                else
                    mbi = CurrentMarket.Mbi[CurrentRecord];

                if (mbi.CasesAnalisys.ContainsKey(arr[i]))
                {
                    var tr = mbi.CasesAnalisys[arr[i]].TradeRatio;
                    var wr = mbi.CasesAnalisys[arr[i]].WaitingRatio;
                    bool _isGapExist = mbi.CasesAnalisys[arr[i]].Gap > 0;

                    SolidColorBrush fc = GetForegroundColor(tr, _isGapExist);

                    TextBlock blc;
                    //заполняем TR
                    blc = FindName(cases[i] + "_tr") as TextBlock;
                    blc.Foreground = fc;
                    blc.Text = tr.ToString("N4", CultureInfo.CurrentCulture);

                    //заполняем WR
                    blc = FindName(cases[i] + "_wr") as TextBlock;
                    blc.Foreground = fc;
                    blc.Text = wr.ToString("N2", CultureInfo.CurrentCulture);

                    //заполняем ok для BestCase
                    blc = FindName(cases[i] + "_mwr") as TextBlock;
                    blc.Foreground = fc;

                    TradeCases currentCase = cases[i].ToUpper().ToTradeCase();
                    TradeCases bestCase = mbi.BestCase;

                    if (currentCase == bestCase && currentCase != TradeCases.empty)
                        blc.Text = "Ок";
                    else
                        blc.Text = "";
                }
                if (mbi.CasesAnalisys.ContainsKey(arr[i]))
                {
                    var tr = mbi.CasesAnalisys[arr[i]].TradeRatio;
                    var wr = mbi.CasesAnalisys[arr[i]].WaitingRatio;
                    bool _isGapExist = mbi.CasesAnalisys[arr[i]].Gap > 0;

                    SolidColorBrush fc = GetForegroundColor(tr, _isGapExist);

                    TextBlock blc;
                    //заполняем TR
                    blc = FindName(cases[i] + "_tr") as TextBlock;
                    blc.Foreground = fc;
                    blc.Text = tr.ToString("N4", CultureInfo.CurrentCulture);

                    //заполняем WR
                    blc = FindName(cases[i] + "_wr") as TextBlock;
                    blc.Foreground = fc;
                    blc.Text = wr.ToString("N2", CultureInfo.CurrentCulture);

                    //заполняем ok для BestCase
                    blc = FindName(cases[i] + "_mwr") as TextBlock;
                    blc.Foreground = fc;

                    TradeCases currentCase = cases[i].ToUpper().ToTradeCase();
                    TradeCases bestCase = mbi.BestCase;

                    if (currentCase == bestCase && currentCase != TradeCases.empty)
                        blc.Text = "Ок";
                    else
                        blc.Text = "";
                }
            }
        }

        private static SolidColorBrush GetForegroundColor(double tr, bool isGapExist)
        {
            if (tr >= 1)    return BlackColor;
            if (isGapExist) return BlueColor;

            return GreenColor;
        }
        #endregion

        #region CurrentCaseProperty
        public static readonly DependencyProperty CurrentCaseProperty =
            DependencyProperty.Register("CurrentCase", typeof(TradeCases), typeof(Cases_BC),
                new FrameworkPropertyMetadata(TradeCases.empty, CurrentCaseOnChanged));

        public TradeCases CurrentCase
        {
            get { return (TradeCases)GetValue(CurrentCaseProperty); }
            set { SetValue(CurrentCaseProperty, value); }
        }

        private static void CurrentCaseOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            Cases_BC ml = doj as Cases_BC;
            ml.UpdateGrid();
        }

        #endregion

        #region CurrentMarketMemberChangeProperty

        public static readonly DependencyProperty CurrentMarketMemberChangeProperty =
            DependencyProperty.Register("CurrentMarketMemberChange", typeof(bool), typeof(Cases_BC),
                new FrameworkPropertyMetadata(false, OnChangedCurrentMarketMemberChange));

        public bool CurrentMarketMemberChange
        {
            get { return (bool)GetValue(CurrentMarketMemberChangeProperty); }
            set { SetValue(CurrentMarketMemberChangeProperty, value); }
        }

        private static void OnChangedCurrentMarketMemberChange(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            Cases_BC uc1 = doj as Cases_BC;

            if (uc1.CurrentMarket != null)
                uc1.UpdateGrid2();
        }

        #endregion

        #region CurentRecordProperty
        public static readonly DependencyProperty CurrentRecordProperty =
            DependencyProperty.Register("CurrentRecord", typeof(int), typeof(Cases_BC),
                new FrameworkPropertyMetadata(-1, OnChangedCurrentRecord));

        public int CurrentRecord
        {
            get { return (int)GetValue(CurrentRecordProperty); }
            set { SetValue(CurrentRecordProperty, value); }
        }

        private static void OnChangedCurrentRecord(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            Cases_BC uc1 = doj as Cases_BC;

            if (uc1.CurrentMarket != null)
                uc1.UpdateGrid2();
        }
        #endregion

        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock blc = sender as TextBlock;
            CurrentCase = GetSelectedRow(blc).Item1.Text.ToUpper().ToTradeCase();
        }

        private Tuple<TextBlock, TextBlock, TextBlock, TextBlock> GetSelectedRow(TextBlock blc)
        {
            for (int i = 0; i < Row; i++)
            {
                if (Grid[i, 0] == blc)
                    return new Tuple<TextBlock, TextBlock, TextBlock, TextBlock>(blc, Grid[i, 1], Grid[i, 2], Grid[i, 3]);

                if (Grid[i, 1] == blc)
                    return new Tuple<TextBlock, TextBlock, TextBlock, TextBlock>(Grid[i, 0], blc, Grid[i, 2], Grid[i, 3]);

                if (Grid[i,2] ==blc)
                    return new Tuple<TextBlock, TextBlock, TextBlock, TextBlock>(Grid[i, 0], Grid[i, 1], blc, Grid[i, 3]);

                if (Grid[i, 3] == blc)
                    return new Tuple<TextBlock, TextBlock, TextBlock, TextBlock>(Grid[i, 0], Grid[i, 1], Grid[i, 2], blc);
            }

            return new Tuple<TextBlock, TextBlock, TextBlock, TextBlock>(Grid[0, 0], Grid[0, 1], Grid[0, 2], Grid[0, 3]); //не должен сюда дойти
        }

        private void UpdateGrid()
        {
            for (int i = 0; i < Row; i++)
            {
                if (CurrentCase == Grid[i, 0].Text.ToTradeCase()) //раскрашиваем строку, если она выделена
                {
                    Grid[i, 0].Background = SelectedBackGroundBrush;
                    Grid[i, 1].Background = SelectedBackGroundBrush;
                    Grid[i, 2].Background = SelectedBackGroundBrush;
                    Grid[i, 3].Background = SelectedBackGroundBrush;
                }
                else // раскрашиваем строку, если она не выделена
                {
                    Grid[i, 0].Background = UnSelectedBackGroundBrush;
                    Grid[i, 1].Background = UnSelectedBackGroundBrush;
                    Grid[i, 2].Background = UnSelectedBackGroundBrush;
                    Grid[i, 3].Background = UnSelectedBackGroundBrush;
                }
            }
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                TextBlock_PreviewMouseDown(sender, null);
        }
    }
}
