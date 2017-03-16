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
    /// Логика взаимодействия для CasesHor_BC.xaml
    /// </summary>
    public sealed partial class CasesHor_BC : UserControl
    {
        private readonly TextBlock[,] Grid;

        private readonly int Row = 2;

        private readonly SolidColorBrush SelectedBackGroundBrush = Brushes.Bisque;
        private readonly SolidColorBrush UnSelectedBackGroundBrush = Brushes.White;

        public CasesHor_BC()
        {
            InitializeComponent();

            Grid = new TextBlock[Row, 4];

            string[] cases = new string[] { "bl1lb1", "bl2lb2"};
            for (int i = 0; i < Row; i++)
            {
                Grid[i, 0] = FindName(cases[i]) as TextBlock;
                Grid[i, 1] = FindName(cases[i] + "_sum") as TextBlock;
                Grid[i, 2] = FindName(cases[i] + "_total") as TextBlock;
                Grid[i, 3] = FindName(cases[i] + "_best") as TextBlock;
            }
        }

        #region CurrentMarketProperty
        public static readonly DependencyProperty CurrentMarketProperty =
            DependencyProperty.Register("CurrentMarket", typeof(MarketInformation), typeof(CasesHor_BC),
                new FrameworkPropertyMetadata(null, CurrentMarketOnChanged));

        public MarketInformation CurrentMarket
        {
            get { return (MarketInformation)GetValue(CurrentMarketProperty); }
            set { SetValue(CurrentMarketProperty, value); }
        }

        private static SolidColorBrush BlueColor = Brushes.Blue;
        private static SolidColorBrush GreenColor = Brushes.Green;
        private static SolidColorBrush BlackColor = Brushes.Black;

        private static void CurrentMarketOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            CasesHor_BC uc3 = doj as CasesHor_BC;

            if (uc3.CurrentMarket != null && uc3.CurrentMarket.Mbi.Last().CasesAnalisys.Count > 0)
                uc3.UpdateGrid2();
        }
        #endregion

        #region CurrentCaseProperty
        public static readonly DependencyProperty CurrentCaseProperty =
            DependencyProperty.Register("CurrentCase", typeof(TradeCases), typeof(CasesHor_BC),
                new FrameworkPropertyMetadata(TradeCases.empty, CurrentCaseOnChanged));

        public TradeCases CurrentCase
        {
            get { return (TradeCases)GetValue(CurrentCaseProperty); }
            set { SetValue(CurrentCaseProperty, value); }
        }

        private static void CurrentCaseOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            CasesHor_BC ml = doj as CasesHor_BC;
            ml.UpdateGrid();
        }

        #endregion

        #region CurrentMarketMemberChangeProperty

        public static readonly DependencyProperty CurrentMarketMemberChangeProperty =
            DependencyProperty.Register("CurrentMarketMemberChange", typeof(bool), typeof(CasesHor_BC),
                new FrameworkPropertyMetadata(false, OnChangedCurrentMarketMemberChange));

        public bool CurrentMarketMemberChange
        {
            get { return (bool)GetValue(CurrentMarketMemberChangeProperty); }
            set { SetValue(CurrentMarketMemberChangeProperty, value); }
        }

        private static void OnChangedCurrentMarketMemberChange(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            CasesHor_BC uc1 = doj as CasesHor_BC;

            if (uc1.CurrentMarket != null)
                uc1.UpdateGrid2();
        }

        #endregion

        #region CurentRecordProperty
        public static readonly DependencyProperty CurrentRecordProperty =
            DependencyProperty.Register("CurrentRecord", typeof(int), typeof(CasesHor_BC),
                new FrameworkPropertyMetadata(-1, OnChangedCurrentRecord));

        public int CurrentRecord
        {
            get { return (int)GetValue(CurrentRecordProperty); }
            set { SetValue(CurrentRecordProperty, value); }
        }

        private static void OnChangedCurrentRecord(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            CasesHor_BC uc1 = doj as CasesHor_BC;

            if (uc1.CurrentMarket != null)
                uc1.UpdateGrid2();
        }
        #endregion

        private void UpdateGrid2()
        {
            MarketBookInformation mbi;
            if (CurrentRecord == -1 || CurrentMarket.Mbi.Count <= CurrentRecord) // второе условие нужно так как оно, хз почему, иногда не выполняется
                mbi = CurrentMarket.Mbi.Last();
            else
                mbi = CurrentMarket.Mbi[CurrentRecord];

            bl1lb1_sum.Text = mbi.CasesAnalisys[TradeCases.BL1LB1].TradeRatio.ToString("N0", CultureInfo.CurrentCulture);
            bl1lb1_total.Text = mbi.CasesAnalisys[TradeCases.BL1LB1].WaitingRatio.ToString("N0", CultureInfo.CurrentCulture);

            bl2lb2_sum.Text = mbi.CasesAnalisys[TradeCases.BL2LB2].TradeRatio.ToString("N0", CultureInfo.CurrentCulture);
            bl2lb2_total.Text = mbi.CasesAnalisys[TradeCases.BL2LB2].WaitingRatio.ToString("N0", CultureInfo.CurrentCulture);
        }

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

                if (Grid[i, 2] == blc)
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
