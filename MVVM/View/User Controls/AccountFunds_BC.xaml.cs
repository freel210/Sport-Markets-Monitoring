using System.Windows;
using System.Windows.Controls;
using Model;

namespace View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AccountFunds_BC.xaml
    /// </summary>
    public sealed partial class AccountFunds_BC : UserControl
    {
        public AccountFunds_BC()
        {
            InitializeComponent();
        }

        #region GetAccountFundsProperty
        public static readonly DependencyProperty GetAccountFundsProperty =
            DependencyProperty.Register("GetAccountFunds", typeof(AccountFundsModel), typeof(AccountFunds_BC),
                new FrameworkPropertyMetadata(null, AccountFundsOnChanged));

        public AccountFundsModel GetAccountFunds
        {
            get { return (AccountFundsModel)GetValue(GetAccountFundsProperty); }
            set { SetValue(GetAccountFundsProperty, value); }
        }

        private static void AccountFundsOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            AccountFunds_BC uc3 = doj as AccountFunds_BC;

            if (uc3.GetAccountFunds != null)
            {
                uc3.blc_0_1.Text = uc3.GetAccountFunds.AvailableToBetBalance;
                uc3.blc_1_1.Text = uc3.GetAccountFunds.DiscountRate;
                uc3.blc_2_1.Text = uc3.GetAccountFunds.Exposure;
                uc3.blc_3_1.Text = uc3.GetAccountFunds.ExposureLimit;
                uc3.blc_4_1.Text = uc3.GetAccountFunds.PointsBalance;
                uc3.blc_5_1.Text = uc3.GetAccountFunds.RetainedCommission;
            }
        }
        #endregion
    }
}
