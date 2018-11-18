using System.Windows.Controls;
using Assistant.Interface;
using Core.Interface;

namespace Assistant.ViewModel
{
    class BaseViewModel : MainViewModel, IViewModel
    {
        private IEmployees _employess;
        public IEmployees Employees
        {
            get => _employess;
            set
            {
                _employess = value;
                OnChanged(nameof(Employees));
            }
        }

        private UserControl _control;
        public UserControl CurrentContent
        {
            get => _control;
            set
            {
                _control = value;
                OnChanged(nameof(CurrentContent));
            }
        }
    }
}
