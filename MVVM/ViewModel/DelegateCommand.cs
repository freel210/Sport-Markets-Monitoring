using System;
using System.Windows.Input;

namespace ViewModel
{
    public sealed class DelegateCommand : ICommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;

        //Конструктор
        public DelegateCommand(Func<object, bool> canExecute, Action<object> execute)
        {
            this._canExecute = canExecute;
            this._execute = execute;
        }

        //Проверка доступности команды
        public bool CanExecute(object parameter)
        {
            return this._canExecute(parameter);
        }

        //Выполнение команды
        public void Execute(object parameter)
        {
            this._execute(parameter);
        }

        //Служебное событие
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested += value; }
        }
    }
}
