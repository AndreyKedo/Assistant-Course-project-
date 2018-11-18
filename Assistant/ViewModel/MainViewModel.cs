using System;
using System.Windows.Input;
using System.ComponentModel;
using Assistant.Interface;

namespace Assistant.ViewModel
{
    class MainViewModel : IMainViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnChanged(string str)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }
    }
    class DelegateCommand : ICommand
    {
        Action<object> action;
        Func<object, bool> func;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action<object> action) : this(action, null)
        {
            this.action = action;
        }

        public DelegateCommand(Action<object> action, Func<object, bool> func)
        {
            this.action = action;
            this.func = func;
        }

        public bool CanExecute(object parameter)
        {
            return func != null ? func.Invoke(parameter) : true;
        }

        public void Execute(object parameter)
        {
            action?.Invoke(parameter);
        }
    }
}
