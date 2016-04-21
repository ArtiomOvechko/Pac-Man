using System;
using System.Windows.Input;

namespace Controller.Core
{
    public class RelayCommand: ICommand
    {
        private readonly Action<object> _action;
        private bool _isEnabled;

        public RelayCommand(Action<object> handler)
        {
            _action = handler;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
