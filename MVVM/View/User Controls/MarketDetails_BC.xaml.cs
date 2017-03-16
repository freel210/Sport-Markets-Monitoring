using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;
using Model;

namespace View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для MarketDetails_BC.xaml
    /// </summary>
    public sealed partial class MarketDetails_BC : UserControl
    {
        private readonly SolidColorBrush BlackColor = Brushes.Black;
        private readonly SolidColorBrush GreenColor = Brushes.Green;

        private readonly TextBlock[] Grid;
        private readonly int Row = 12;
        
        public MarketDetails_BC()
        {
            InitializeComponent();

            Grid = new TextBlock[Row];
            for (int i = 0; i < Row; i++)
                Grid[i] = FindName("blc_" + i + "_1") as TextBlock;
        }

        #region CurrentMarketProperty
        public static readonly DependencyProperty CurrentMarketProperty =
            DependencyProperty.Register("CurrentMarket", typeof(MarketInformation), typeof(MarketDetails_BC),
                new FrameworkPropertyMetadata(null, CurrentMarketOnChanged));

        public MarketInformation CurrentMarket
        {
            get { return (MarketInformation)GetValue(CurrentMarketProperty); }
            set { SetValue(CurrentMarketProperty, value); }
        }

        private static void CurrentMarketOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            MarketDetails_BC uc3 = doj as MarketDetails_BC;

            if (uc3.CurrentMarket != null)
            {
                uc3.Grid[0].Text = uc3.CurrentMarket.MarketId;
                uc3.Grid[1].Text = uc3.CurrentMarket.TotalMatched.ToString("N0", CultureInfo.CurrentCulture);
                uc3.Grid[2].Text = uc3.CurrentMarket.CompetitionName;
                uc3.Grid[3].Text = uc3.CurrentMarket.EventType;
                uc3.Grid[4].Text = uc3.CurrentMarket.EventName;
                uc3.Grid[5].Text = uc3.CurrentMarket.MarketName;
                uc3.Grid[6].Text = uc3.CurrentMarket.Mbi.Last().Status.ToString();
                uc3.Grid[7].Text = uc3.CurrentMarket.Mbi.Last().NumberOfRunners.ToString();
                uc3.Grid[8].Text = uc3.CurrentMarket.Mbi.Last().NumberOfWinners.ToString();
                uc3.Grid[9].Text = uc3.CurrentMarket.StartMarketTime.ToString();

                bool b = uc3.CurrentMarket.Mbi.Last().IsInplay;
                if (b) uc3.Grid[11].Foreground = uc3.GreenColor;
                else   uc3.Grid[11].Foreground = uc3.BlackColor;

                uc3.Grid[11].Text = b.ToString();
            }
        }
        #endregion

        #region TimeLeftProperty
        public static readonly DependencyProperty TimeLeftProperty =
            DependencyProperty.Register("TimeLeft", typeof(string), typeof(MarketDetails_BC),
                new FrameworkPropertyMetadata(String.Empty, TimeLeftOnChanged));

        public string TimeLeft
        {
            get { return (string)GetValue(TimeLeftProperty); }
            set { SetValue(TimeLeftProperty, value); }
        }

        private static void TimeLeftOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            MarketDetails_BC uc3 = doj as MarketDetails_BC;
            uc3.Grid[10].Text = uc3.TimeLeft;
        }
        #endregion
    }
}
