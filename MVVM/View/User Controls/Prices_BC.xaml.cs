using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Linq;
using Model;

namespace View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Prices_BC.xaml
    /// </summary>
    public sealed partial class Prices_BC : UserControl
    {
        private readonly TextBlock[] Grid = new TextBlock[24];

        public Prices_BC()
        {
            InitializeComponent();
            
            Grid[0] = r1_b3p; Grid[1] = r1_b2p; Grid[2] = r1_b1p;
            Grid[3] = r1_b3a; Grid[4] = r1_b2a; Grid[5] = r1_b1a;
            Grid[6] = r1_l3p; Grid[7] = r1_l2p; Grid[8] = r1_l1p;
            Grid[9] = r1_l3a; Grid[10] = r1_l2a; Grid[11] = r1_l1a;

            Grid[12] = r2_b3p; Grid[13] = r2_b2p; Grid[14] = r2_b1p;
            Grid[15] = r2_b3a; Grid[16] = r2_b2a; Grid[17] = r2_b1a;
            Grid[18] = r2_l3p; Grid[19] = r2_l2p; Grid[20] = r2_l1p;
            Grid[21] = r2_l3a; Grid[22] = r2_l2a; Grid[23] = r2_l1a;
        }

        #region CurrentMarketProperty
        public static readonly DependencyProperty CurrentMarketProperty =
            DependencyProperty.Register("CurrentMarket", typeof(MarketInformation), typeof(Prices_BC),
                new FrameworkPropertyMetadata(null, CurrentMarketOnChanged));

        public MarketInformation CurrentMarket
        {
            get { return (MarketInformation)GetValue(CurrentMarketProperty); }
            set { SetValue(CurrentMarketProperty, value); }
        }

        private static void CurrentMarketOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            Prices_BC uc3 = doj as Prices_BC;

            _clearSheet(uc3);

            if (uc3.CurrentMarket != null)
                uc3.UpdateGrid();
        }
        #endregion

        #region CurrentMarketMemberChangeProperty

        public static readonly DependencyProperty CurrentMarketMemberChangeProperty =
            DependencyProperty.Register("CurrentMarketMemberChange", typeof(bool), typeof(Prices_BC),
                new FrameworkPropertyMetadata(false, OnChangedCurrentMarketMemberChange));

        public bool CurrentMarketMemberChange
        {
            get { return (bool)GetValue(CurrentMarketMemberChangeProperty); }
            set { SetValue(CurrentMarketMemberChangeProperty, value); }
        }

        private static void OnChangedCurrentMarketMemberChange(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            Prices_BC uc1 = doj as Prices_BC;

            _clearSheet(uc1);

            if (uc1.CurrentMarket != null)
                uc1.UpdateGrid();
        }

        #endregion

        #region CurentRecordProperty
        public static readonly DependencyProperty CurrentRecordProperty =
            DependencyProperty.Register("CurrentRecord", typeof(int), typeof(Prices_BC),
                new FrameworkPropertyMetadata(-1, OnChangedCurrentRecord));

        public int CurrentRecord
        {
            get { return (int)GetValue(CurrentRecordProperty); }
            set { SetValue(CurrentRecordProperty, value); }
        }

        private static void OnChangedCurrentRecord(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            Prices_BC uc1 = doj as Prices_BC;

            _clearSheet(uc1);

            if (uc1.CurrentMarket != null)
                uc1.UpdateGrid();
        }
        #endregion

        private void UpdateGrid()
        {
            MarketBookInformation mbi;
            if (CurrentRecord == -1 || CurrentMarket.Mbi.Count <= CurrentRecord) // второе условие нужно так как оно, хз почему, иногда не выполняется
                mbi = CurrentMarket.Mbi.Last();
            else
                mbi = CurrentMarket.Mbi[CurrentRecord];

            tm.Text = CurrentMarket.TotalMatched.ToString("N0", CultureInfo.CurrentCulture);
            time.Text = mbi.LastByte.ToString("HH:mm:ss", CultureInfo.CurrentCulture);

            //данные по runner0
            r1.Text = CurrentMarket.Runner0Name;

            r1_b3p.Text = mbi.ExPricesRunner0.AvailableToBack[2].Price.ToString("N2", CultureInfo.CurrentCulture);
            r1_b2p.Text = mbi.ExPricesRunner0.AvailableToBack[1].Price.ToString("N2", CultureInfo.CurrentCulture);
            r1_b1p.Text = mbi.ExPricesRunner0.AvailableToBack[0].Price.ToString("N2", CultureInfo.CurrentCulture);

            r1_l1p.Text = mbi.ExPricesRunner0.AvailableToLay[0].Price.ToString("N2", CultureInfo.CurrentCulture);
            r1_l2p.Text = mbi.ExPricesRunner0.AvailableToLay[1].Price.ToString("N2", CultureInfo.CurrentCulture);
            r1_l3p.Text = mbi.ExPricesRunner0.AvailableToLay[2].Price.ToString("N2", CultureInfo.CurrentCulture);

            r1_b3a.Text = mbi.ExPricesRunner0.AvailableToBack[2].Size.ToString("N0", CultureInfo.CurrentCulture);
            r1_b2a.Text = mbi.ExPricesRunner0.AvailableToBack[1].Size.ToString("N0", CultureInfo.CurrentCulture);
            r1_b1a.Text = mbi.ExPricesRunner0.AvailableToBack[0].Size.ToString("N0", CultureInfo.CurrentCulture);

            r1_l1a.Text = mbi.ExPricesRunner0.AvailableToLay[0].Size.ToString("N0", CultureInfo.CurrentCulture);
            r1_l2a.Text = mbi.ExPricesRunner0.AvailableToLay[1].Size.ToString("N0", CultureInfo.CurrentCulture);
            r1_l3a.Text = mbi.ExPricesRunner0.AvailableToLay[2].Size.ToString("N0", CultureInfo.CurrentCulture);

            //данные по runner1
            r2.Text = CurrentMarket.Runner1Name;

            r2_b3p.Text = mbi.ExPricesRunner1.AvailableToBack[2].Price.ToString("N2", CultureInfo.CurrentCulture);
            r2_b2p.Text = mbi.ExPricesRunner1.AvailableToBack[1].Price.ToString("N2", CultureInfo.CurrentCulture);
            r2_b1p.Text = mbi.ExPricesRunner1.AvailableToBack[0].Price.ToString("N2", CultureInfo.CurrentCulture);

            r2_l1p.Text = mbi.ExPricesRunner1.AvailableToLay[0].Price.ToString("N2", CultureInfo.CurrentCulture);
            r2_l2p.Text = mbi.ExPricesRunner1.AvailableToLay[1].Price.ToString("N2", CultureInfo.CurrentCulture);
            r2_l3p.Text = mbi.ExPricesRunner1.AvailableToLay[2].Price.ToString("N2", CultureInfo.CurrentCulture);

            r2_b3a.Text = mbi.ExPricesRunner1.AvailableToBack[2].Size.ToString("N0", CultureInfo.CurrentCulture);
            r2_b2a.Text = mbi.ExPricesRunner1.AvailableToBack[1].Size.ToString("N0", CultureInfo.CurrentCulture);
            r2_b1a.Text = mbi.ExPricesRunner1.AvailableToBack[0].Size.ToString("N0", CultureInfo.CurrentCulture);

            r2_l1a.Text = mbi.ExPricesRunner1.AvailableToLay[0].Size.ToString("N0", CultureInfo.CurrentCulture);
            r2_l2a.Text = mbi.ExPricesRunner1.AvailableToLay[1].Size.ToString("N0", CultureInfo.CurrentCulture);
            r2_l3a.Text = mbi.ExPricesRunner1.AvailableToLay[2].Size.ToString("N0", CultureInfo.CurrentCulture);
        }
        
        private static void _clearSheet(Prices_BC uc1)
        {
            uc1.tm.Text = "";
            uc1.r1.Text = "";
            uc1.r2.Text = "";
            uc1.time.Text = "";

            foreach (var item in uc1.Grid)
                item.Text = "";
        }
    }
}
