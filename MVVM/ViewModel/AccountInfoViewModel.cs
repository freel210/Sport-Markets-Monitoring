using System.Threading.Tasks;
using Model;
using CommonLibrary;

namespace ViewModel
{
    internal sealed class AccountInfoViewModel : NotifyPropertyChanger
    {
        private readonly MainModel model;

        private AccountDetailsModel _getAccountDetails;
        public AccountDetailsModel GetAccountDetails
        {
            get { return _getAccountDetails; }
            set
            {
                _getAccountDetails = value;
                OnPropertyChanged();
            }
        }

        private AccountFundsModel _getAccountFunds;
        public AccountFundsModel GetAccountFunds
        {
            get { return _getAccountFunds; }
            set
            {
                _getAccountFunds = value;
                OnPropertyChanged();
            }
        }

        public AccountInfoViewModel(MainModel m)
        {
            model = m;

            Task.Run(() => GetAccountDetails = model.GetAccountDetails());
            Task.Run(() => GetAccountFunds = model.GetAccountFunds());
        }
    }
}
