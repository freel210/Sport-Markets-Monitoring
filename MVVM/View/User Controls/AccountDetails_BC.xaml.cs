using System.Windows;
using System.Windows.Controls;
using Model;

namespace View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AccountDetails_BC.xaml
    /// </summary>
    public sealed partial class AccountDetails_BC : UserControl
    {
        public AccountDetails_BC()
        {
            InitializeComponent();
        }

        #region GetAccountDetailsProperty
        public static readonly DependencyProperty GetAccountDetailsProperty =
            DependencyProperty.Register("GetAccountDetails", typeof(AccountDetailsModel), typeof(AccountDetails_BC),
                new FrameworkPropertyMetadata(null, AccountDetailsOnChanged));

        public AccountDetailsModel GetAccountDetails
        {
            get { return (AccountDetailsModel)GetValue(GetAccountDetailsProperty); }
            set { SetValue(GetAccountDetailsProperty, value); }
        }

        private static void AccountDetailsOnChanged(DependencyObject doj, DependencyPropertyChangedEventArgs dp)
        {
            AccountDetails_BC uc3 = doj as AccountDetails_BC;

            if (uc3.GetAccountDetails != null)
            {
                uc3.blc_0_1.Text = uc3.GetAccountDetails.CountryCode;
                uc3.blc_1_1.Text = uc3.GetAccountDetails.CurrencyCode;
                uc3.blc_2_1.Text = uc3.GetAccountDetails.FirstName;
                uc3.blc_3_1.Text = uc3.GetAccountDetails.LastName;
                uc3.blc_4_1.Text = uc3.GetAccountDetails.LocalCode;
                uc3.blc_5_1.Text = uc3.GetAccountDetails.Region;
                uc3.blc_6_1.Text = uc3.GetAccountDetails.TimeZone;
                uc3.blc_7_1.Text = uc3.GetAccountDetails.DiscountRate;
            }
        }
        #endregion
    }
}
