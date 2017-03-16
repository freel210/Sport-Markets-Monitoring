using System.Windows;
using Model;
using View;
using ViewModel;

namespace MVVM
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    sealed partial class App : Application
    {
        public App()
        {
            ShutdownMode = ShutdownMode.OnMainWindowClose;

            var mw = new MainWindow
            {
                DataContext = new MainViewModel(new MainModel())
            };
            mw.Show();
        }
    }
}
