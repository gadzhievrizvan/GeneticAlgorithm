using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GeneticGUI
{
    public sealed class RelayCommand : ICommand
    {
        private readonly Action _action;
        public RelayCommand(Action action)
        {
            _action = action;
        }

        public event EventHandler CanExecuteChanged; //оповещает о изменении доступности команды

        public bool CanExecute(object parameter) //можно ли нажать на кнопку
        {
            return true;
        }

        public void Execute(object parameter) //метод вызываемый при нажатии на кнопку
        {
            _action.Invoke();
        }
    }
}
