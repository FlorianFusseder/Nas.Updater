using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Nas.Updater.GUI
{
    class RelayCommand : ICommand
    {
        

        #region Felder
        private Action<object> execute;
        private Func<object, bool> canExecute;
        #endregion

        #region Konstruktoren
        public RelayCommand(Action<object> Execute)
            : this(Execute, null)
        {
        }

        public RelayCommand(Action<object> Execute, Func<object, bool> CanExecute)
        {
            this.execute = Execute;
            this.canExecute = CanExecute;
        }
        #endregion

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
