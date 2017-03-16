using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Model.Analisys;
using Model;

namespace View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Analysedaten_BC.xaml
    /// </summary>
    public sealed partial class Analysedaten_BC : UserControl
    {
        private readonly SolidColorBrush BackColor = Brushes.LightBlue;
        private readonly SolidColorBrush LayColor = Brushes.LightCoral;
        private readonly SolidColorBrush GreyColor = Brushes.Gray;
        private readonly SolidColorBrush BlackColor = Brushes.Black;
        private readonly SolidColorBrush WhiteColor = Brushes.White;

        private readonly SolidColorBrush RedColor = Brushes.Red;
        private readonly SolidColorBrush GreenColor = Brushes.Green;

        public Analysedaten_BC()
        {
            InitializeComponent();
        }

        #region CurrentCaseProperty
        public static readonly DependencyProperty CurrentCaseProperty =
            DependencyProperty.Register("CurrentCase", typeof(TradeCases), typeof(Analysedaten_BC),
                new FrameworkPropertyMetadata(TradeCases.empty, CurrentCaseOnChanged));

        public TradeCases CurrentCase
        {
            get { return (TradeCases)GetValue(CurrentCaseProperty); }
            set { SetValue(CurrentCaseProperty, value); }
        }

        private static void CurrentCaseOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            Analysedaten_BC ml = doj as Analysedaten_BC;
            ml.UpdateGrid();
        }
        #endregion

        #region CurrentMarketProperty
        public static readonly DependencyProperty CurrentMarketProperty =
            DependencyProperty.Register("CurrentMarket", typeof(MarketInformation), typeof(Analysedaten_BC),
                new FrameworkPropertyMetadata(null, CurrentMarketOnChanged));

        public MarketInformation CurrentMarket
        {
            get { return (MarketInformation)GetValue(CurrentMarketProperty); }
            set { SetValue(CurrentMarketProperty, value); }
        }

        private static void CurrentMarketOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            Analysedaten_BC ml = doj as Analysedaten_BC;
            ml.UpdateGrid();
        }
        #endregion        
        
        #region CurrentMarketMemberChangeProperty

        public static readonly DependencyProperty CurrentMarketMemberChangeProperty =
            DependencyProperty.Register("CurrentMarketMemberChange", typeof(bool), typeof(Analysedaten_BC),
                new FrameworkPropertyMetadata(false, OnChangedCurrentMarketMemberChange));

        public bool CurrentMarketMemberChange
        {
            get { return (bool)GetValue(CurrentMarketMemberChangeProperty); }
            set { SetValue(CurrentMarketMemberChangeProperty, value); }
        }

        private static void OnChangedCurrentMarketMemberChange(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            Analysedaten_BC uc1 = doj as Analysedaten_BC;

            uc1._clearSheet();

            if (uc1.CurrentMarket != null)
                uc1.UpdateGrid();
        }

        #endregion

        #region CurentRecordProperty
        public static readonly DependencyProperty CurrentRecordProperty =
            DependencyProperty.Register("CurrentRecord", typeof(int), typeof(Analysedaten_BC),
                new FrameworkPropertyMetadata(-1, OnChangedCurrentRecord));

        public int CurrentRecord
        {
            get { return (int)GetValue(CurrentRecordProperty); }
            set { SetValue(CurrentRecordProperty, value); }
        }

        private static void OnChangedCurrentRecord(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            Analysedaten_BC uc1 = doj as Analysedaten_BC;

            uc1._clearSheet();

            if (uc1.CurrentMarket != null)
                uc1.UpdateGrid();
        }
        #endregion

        private void UpdateGrid()
        {
            _clearSheet();

            MarketBookInformation mbi = new MarketBookInformation();
            if (CurrentMarket != null)
            {
                if (CurrentRecord == -1 || CurrentMarket.Mbi.Count <= CurrentRecord) // второе условие нужно так как оно, хз почему, иногда не выполняется
                    mbi = CurrentMarket.Mbi.Last();
                else
                    mbi = CurrentMarket.Mbi[CurrentRecord];
            }

            // ставки открытия и закрытия
            if (CurrentMarket != null && mbi.CasesAnalisys.Count > 0 && CurrentCase != TradeCases.empty && 
                CurrentCase != TradeCases.BL2B1 && CurrentCase != TradeCases.LB2L1)
            {
                r1_bet_teor.Text = mbi.CasesAnalisys[CurrentCase].StartBet.ToString("N2", CultureInfo.CurrentCulture);
                r2_bet_teor.Text = mbi.CasesAnalisys[CurrentCase].CloseBet.ToString("N2", CultureInfo.CurrentCulture);
            }

            if (CurrentMarket != null && mbi.CasesAnalisys.Count > 0 && CurrentCase != TradeCases.empty && 
                (CurrentCase == TradeCases.BL2B1 || CurrentCase == TradeCases.LB2L1))
            {
                r1_bet_teor.Text = mbi.CasesAnalisys[CurrentCase].CloseBet.ToString("N2", CultureInfo.CurrentCulture);
                r2_bet_teor.Text = mbi.CasesAnalisys[CurrentCase].StartBet.ToString("N2", CultureInfo.CurrentCulture);
            }

            if (CurrentMarket != null && mbi.CasesAnalisys.Count > 0 && CurrentCase != TradeCases.empty)
            {
                // цены и размеры Back
                r1_b1p.Text = mbi.CasesAnalisys[CurrentCase].B1Price.ToString("N2", CultureInfo.CurrentCulture);
                r1_b1a.Text = mbi.CasesAnalisys[CurrentCase].B1Size.ToString("N0", CultureInfo.CurrentCulture);
                r2_b1p.Text = mbi.CasesAnalisys[CurrentCase].B2Price.ToString("N2", CultureInfo.CurrentCulture);
                r2_b1a.Text = mbi.CasesAnalisys[CurrentCase].B2Size.ToString("N0", CultureInfo.CurrentCulture);

                // цены и размеры Lay
                r1_l1p.Text = mbi.CasesAnalisys[CurrentCase].L1Price.ToString("N2", CultureInfo.CurrentCulture);
                r1_l1a.Text = mbi.CasesAnalisys[CurrentCase].L1Size.ToString("N0", CultureInfo.CurrentCulture);
                r2_l1p.Text = mbi.CasesAnalisys[CurrentCase].L2Price.ToString("N2", CultureInfo.CurrentCulture);
                r2_l1a.Text = mbi.CasesAnalisys[CurrentCase].L2Size.ToString("N0", CultureInfo.CurrentCulture);

                // прибыль по каждому раннеру
                // даем короткие имена
                double sp = mbi.CasesAnalisys[CurrentCase].StartRunnerProfit;
                double cp = mbi.CasesAnalisys[CurrentCase].CloseRunnerProfit;

                //задаем цвет
                if (sp < 0 || cp <0)
                {
                    r1_income_teor.Foreground = RedColor;
                    r2_income_teor.Foreground = RedColor;
                }
                else
                {
                    r1_income_teor.Foreground = GreenColor;
                    r2_income_teor.Foreground = GreenColor;
                }

                //задаем значение
                if (CurrentCase == TradeCases.BL1B2 || CurrentCase == TradeCases.LB1L2)
                {
                    r1_income_teor.Text = sp.ToString("N4", CultureInfo.CurrentCulture);
                    r2_income_teor.Text = cp.ToString("N4", CultureInfo.CurrentCulture);
                }
                else
                {
                    r1_income_teor.Text = cp.ToString("N4", CultureInfo.CurrentCulture);
                    r2_income_teor.Text = sp.ToString("N4", CultureInfo.CurrentCulture);
                }
            }

            // раскрасска ячеек в зависимости от типа торговли
            List<TextBlock> B1 = new List<TextBlock>() { r1_b1p, r1_b1a};
            List<TextBlock> B2 = new List<TextBlock>() { r2_b1p, r2_b1a};
           
            List<TextBlock> L1 = new List<TextBlock>() { r1_l1p, r1_l1a};
            List<TextBlock> L2 = new List<TextBlock>() { r2_l1p, r2_l1a};

            switch (CurrentCase)
            {
                case TradeCases.BL1B2:
                    _setBackgroundColor(B2.Concat(L1), BackColor);
                    _setBackgroundColor(B1.Concat(L2), WhiteColor);

                    _setForegroundColor(L1, GreenColor);
                    _setForegroundColor(B2, BlackColor);
                    _setForegroundColor(B1.Concat(L2), GreyColor);
                    break;

                case TradeCases.BL2B1:
                    _setBackgroundColor(B1.Concat(L2), BackColor);
                    _setBackgroundColor(B2.Concat(L1), WhiteColor);

                    _setForegroundColor(L2, GreenColor);
                    _setForegroundColor(B1, BlackColor);
                    _setForegroundColor(B2.Concat(L1), GreyColor);
                    break;

                case TradeCases.LB1L2:
                    _setBackgroundColor(B1.Concat(L2), LayColor);
                    _setBackgroundColor(B2.Concat(L1), WhiteColor);

                    _setForegroundColor(B1, GreenColor);
                    _setForegroundColor(L2, BlackColor);
                    _setForegroundColor(B2.Concat(L1), GreyColor);
                    break;

                case TradeCases.LB2L1:
                    _setBackgroundColor(B2.Concat(L1), LayColor);
                    _setBackgroundColor(B1.Concat(L2), WhiteColor);

                    _setForegroundColor(B2, GreenColor);
                    _setForegroundColor(L1, BlackColor);
                    _setForegroundColor(B1.Concat(L2), GreyColor);
                    break;

                case TradeCases.BL1LB1:
                    _setBackgroundColor(B1, LayColor);
                    _setBackgroundColor(L1, BackColor);
                    _setBackgroundColor(B2.Concat(L2), WhiteColor);

                    _setForegroundColor(B1.Concat(L1), GreenColor);
                    _setForegroundColor(B2.Concat(L2), GreyColor);
                    break;

                case TradeCases.BL2LB2:
                    _setBackgroundColor(B2, LayColor);
                    _setBackgroundColor(L2, BackColor);
                    _setBackgroundColor(B1.Concat(L1), WhiteColor);

                    _setForegroundColor(B2.Concat(L2), GreenColor);
                    _setForegroundColor(B1.Concat(L1), GreyColor);
                    break;

                default:
                    _setBackgroundColor(B1.Concat(B2).Concat(L1).Concat(L2), WhiteColor);
                    _setForegroundColor(B1.Concat(B2).Concat(L1).Concat(L2), GreyColor);
                    break;
            }
        }

        private void _setBackgroundColor(IEnumerable<TextBlock> source, SolidColorBrush color)
        {
            foreach (var item in source)
                item.Background = color;
        }

        private void _setForegroundColor(IEnumerable<TextBlock> source, SolidColorBrush color)
        {
            foreach (var item in source)
                item.Foreground = color;
        }

        private void _clearSheet()
        {
            r1_bet_teor.Text = "";
            r2_bet_teor.Text = "";
            
            r1_b1p.Text = "";
            r1_b1a.Text = "";

            r1_l1p.Text = "";
            r1_l1a.Text = "";

            r2_b1p.Text = "";
            r2_b1a.Text = "";

            r2_l1p.Text = "";
            r2_l1a.Text = "";
        }
    }
}
