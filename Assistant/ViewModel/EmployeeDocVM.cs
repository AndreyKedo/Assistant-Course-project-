using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interface;
using Core.Model;
using Assistant.Interface;
using System.Windows.Input;
using System.Windows.Data;
using System.Globalization;

namespace Assistant.ViewModel
{
    class EmployeeDocVM : BaseViewModel, IViewModel
    {
        private readonly IDataAccessDoctor DataAccess;
        public ObservableCollection<Entry> EntrySource { get; set; }

        private int _isSuccess;
        public int IsSuccess {
            get
            {
                return _isSuccess;
            }
            set {
                _isSuccess = value;
                if(_isSuccess == 0)
                {
                    if (EntrySource.Count != 0)
                        EntrySource.Clear();
                    foreach (var item in DataAccess.GetEntriesOfCurrDay())
                    {
                        EntrySource.Add(item);
                    }
                }else if (_isSuccess == 1)
                {
                    if (EntrySource.Count != 0)
                        EntrySource.Clear();
                    foreach (var item in DataAccess.GetEntriesOfBack())
                    {
                        EntrySource.Add(item);
                    }
                }
                OnChanged(nameof(IsSuccess));
            }
        }

        public EmployeeDocVM(IDataAccessDoctor dataAccess, IEmployees employees) : base(employees)
        {
            DataAccess = dataAccess;
            UpdateDataContext();
            EntrySource = new ObservableCollection<Entry>();
            IsSuccess = 0;
        }

        public ICommand DeleteCom
        {
            get => new DelegateCommand((obj)=> 
            {
                EntrySource.Remove((obj as Entry));
            },(obj) => 
            {
                return obj != null;
            });
        }

        private async void UpdateDataContext()
        {
            await DataAccess.UploadEntryForDoctor(Employees.Id);
        }
    }

    public class RadioBoolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int integer = (int)value;
            if (integer == int.Parse(parameter.ToString()))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
