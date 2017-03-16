using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace CommonLibrary
{
    public abstract class NotifyPropertyChanger : INotifyPropertyChanged
    {
        //Для поддержка байндинга
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
