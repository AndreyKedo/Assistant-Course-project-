using System;
using System.Collections.ObjectModel;
using Assistant.Interface;
using Core.Interface;
using Core.Model;
using System.Windows.Input;

using Assistant.View;

namespace Assistant.ViewModel
{
    /// <summary>
    /// В будущем написать диалоговый сервис что бы отвязать VM от view
    /// Нужны диалоги отчётов
    /// </summary>
    class EmployeeRegVM : BaseViewModel, IViewModel
    {
        readonly IDataAccessRegistrator DataAccess;
        //статус доступа
        private string _statys;
        public string Statys
        {
            get => _statys;
            set
            {
                _statys = value;
                OnChanged(nameof(Statys));
            }
        }
        //Объект порождающий записи
        private CreateEntry _entry;
        public CreateEntry EntryCreate
        {
            get => _entry;
            set
            {
                _entry = value;
                OnChanged(nameof(CreateEntry));
            }
        }

        public EmployeeRegVM(IDataAccessRegistrator dataAccess)
        {
            DataAccess = dataAccess;
            Update();
        }

        #region Коллекции пацментов и записей
        public ObservableCollection<Patient> Patients { get; set; }
        public ObservableCollection<Entry> Entries { get; set; }
        #endregion

        #region Список тип услуг и выбранный тип
        public ReadOnlyObservableCollection<TypeService> TypeServices { get; set; }

        private TypeService _selectType;
        public TypeService SectedType
        {
            get => _selectType;
            set
            {
                _selectType = value;
                if (_selectType != null)
                {
                    Services.Clear();
                    Doc = null;
                    foreach (var item in _selectType.GetServiceOfDoctor)
                    {
                        Services.Add(item.Key);
                    }
                }
                OnChanged(nameof(SectedType));
            }
        }
        #endregion

        #region Список услуг и выбраная услуга
        public ObservableCollection<Service> Services { get; set; }

        private Service _selectService;
        public Service SelectedService
        {
            get => _selectService;
            set
            {
                _selectService = value;
                if (_selectService != null)
                {
                    Doctor buffer;
                    SectedType.GetServiceOfDoctor.TryGetValue(SelectedService, out buffer);
                    Doc = buffer;
                }
                OnChanged(nameof(SelectedService));
            }
        }
        #endregion

        #region Диалог отчётов
        private IWindow ReportView;

        private string _textReport;
        public string TextReport
        {
            get => _textReport;
            set
            {
                _textReport = value;
                OnChanged(nameof(TextReport));
            }
        }
        #endregion
        
        #region Сущности хранящие данные для клиента и специалиста
        //Текущий специалист который оказывате выбранную услугу
        private Doctor _doc;
        public Doctor Doc
        {
            get => _doc;
            set
            {
                _doc = value;
                OnChanged(nameof(Doc));
            }
        }
        //Выбранный клиент
        private Patient _patient;
        public Patient SelectPatient
        {
            get => _patient;
            set
            {
                _patient = value;
                if(_patient != null)
                    EntryCreate.SetPatient(ref _patient);
                OnChanged(nameof(SelectPatient));
            }
        }
        #endregion
        
        #region Свойства для поиска
        //Свойство поиска записей и 
        private DateTime _findEntry;
        public DateTime FindEntry
        {
            get => _findEntry;
            set
            {
                _findEntry = value;
                GetEntriesToDate(_findEntry);
                OnChanged(nameof(FindEntry));
            }
        }
        //Свойство поиска клиента
        private string _findPatient;
        public string FindPatient
        {
            get => _findPatient;
            set
            {
                _findPatient = value;
                if (_findPatient != string.Empty && DataAccess.GetPatient.Count != 0)
                {
                    Patients.Clear();
                    foreach (var item in DataAccess.FindPatient(_findPatient))
                    {
                        Patients.Add(item);
                    }
                }else if(DataAccess.GetPatient.Count != 0)
                {
                    GetEntriesToDate(DateTime.UtcNow);
                }
                OnChanged(nameof(FindPatient));
            }
        }
        #endregion

        #region Команды
        //создает отчет по "Оказанные услуги по специалистам"
        public ICommand ReportService
        {
            get
            {
                return new DelegateCommand(async (obj)=> 
                {
                    ReportView = new ReportWindow()
                    {
                        DataContext = this,
                        Title = "Оказанные услуги по специалистам"
                    };
                    TextReport = await DataAccess.GetServiceOfDoctorReport();
                    ReportView.Show();
                });
            }
        }
        //создает отчет по "Оказанные услуги клиентам"
        public ICommand ReportCommand
        {
            get => new DelegateCommand(async (obj) => 
            {
                ReportView = new ReportWindow
                {
                    Title = "Оказанные услуги по клиентам",
                    DataContext = this
                };
                TextReport = await DataAccess.GetPatientReport();
                ReportView.Show();
            }, (obj)=> 
            {
                return Patients.Count != 0;
            });
        }
        //создает запись
        public ICommand WriteEntry
        {
            get => new DelegateCommand(async(obj)=> 
            {
                if (SelectPatient != null)
                {
                    await DataAccess.AddEntry(EntryCreate.GetEntry(SelectedService, Doc, Employees), SelectPatient.Id);
                }
                else
                {
                    await DataAccess.AddEntry(EntryCreate.GetEntry(SelectedService, Doc, Employees));
                }
                Services.Clear();
                SelectedService = null;
                SectedType = null;
                Doc = null;
                EntryCreate.ClearData();
                GetEntriesToDate(DateTime.UtcNow);
            },(obj)=> 
            {
                return EntryCreate.ValidationData() && SelectedService != null;
            });
        }
        //очищает форму
        public ICommand CleanForm
        {
            get => new DelegateCommand((obj) =>
            {
                Services.Clear();
                Doc = null;
                EntryCreate.ClearData();
            }, (obj) => 
            {
                return EntryCreate.ValidationData();
            });
        }
        #endregion

        #region Методы для работы с коллекциями

        //выгружает записи на текущий день
        private void GetEntriesToDate(DateTime date)
        {
            if (DataAccess.GetPatient.Count != 0)
            {
                if (Entries.Count != 0)
                    Entries.Clear();
                foreach (var item in DataAccess.FindEntries(date))
                {
                    Entries.Add(item);
                }
            }
        }

        //добавляет в списоки элементы
        private void UpdateLists()
        {
            if (DataAccess.GetPatient.Count > Patients.Count)
            {
                Patients.Add(DataAccess.GetPatient[DataAccess.GetPatient.Count - 1]);
            }
            if (DataAccess.GetEntries.Count > Entries.Count)
            {
                Entries.Add(DataAccess.GetEntries[DataAccess.GetEntries.Count - 1]);
            }
        }

        //Метод для выгрузки данных с data access
        private async void Update()
        {
            if (await DataAccess.UploadData())
            {
                Statys = "Данные обновлены";
                TypeServices = new ReadOnlyObservableCollection<TypeService>(new ObservableCollection<TypeService>(DataAccess.GetTypeService));
                Patients = new ObservableCollection<Patient>(DataAccess.GetPatient);
                Entries = new ObservableCollection<Entry>(DataAccess.GetEntries);
                Services = new ObservableCollection<Service>();
                EntryCreate = new CreateEntry();
                EntryCreate.ClearData();
                FindEntry = DateTime.UtcNow;
            }
        }
        #endregion
    }

/// <summary>
///  Класс для создания записи из данных с формы и валидации данных
/// </summary>
    class CreateEntry : MainViewModel
    {
        private DateTime _dateReg;
        public DateTime DateRegistration
        {
            get => _dateReg;
            set
            {
                _dateReg = value;
                OnChanged(nameof(DateRegistration));
            }
        }

        private string l_name;
        public string LName
        {
            private get => l_name;
            set
            {
                l_name = value;
                OnChanged(nameof(LName));
            }
        }

        private string _time;
        public string Timepick
        {
            get => _time;
            set
            {
                _time = value;
                if(_time != string.Empty)
                {
                    if (char.IsDigit(_time[0]) && char.IsDigit(_time[2]) && char.IsDigit(_time[3]) && _time.Length == 4)
                    {
                        char separator = _time[1];
                        if (_time[1] != ':')
                            _time = _time.Replace(separator, ':');
                    }
                    else if ((char.IsDigit(_time[0]) && char.IsDigit(_time[1]) && char.IsDigit(_time[3]) && char.IsDigit(_time[4])) && _time.Length == 5)
                    {
                        char separator = _time[2];
                        if (_time[2] != ':')
                            _time = _time.Replace(separator, ':');
                    }
                    DateRegistration = DateRegistration.Date;
                    DateRegistration = DateRegistration.Add(TimeSpan.Parse(_time)).ToUniversalTime();
                }
                OnChanged(nameof(Timepick));
            }
        }

        private string f_name;
        public string FName
        {
            private get => f_name;
            set
            {
                f_name = value;
                OnChanged(nameof(FName));
            }
        }

        private string t_name;
        public string TName
        {
            private get => t_name;
            set
            {
                t_name = value;
                OnChanged(nameof(TName));
            }
        }

        private string _sex;
        public string Sex
        {
            private get => _sex;
            set
            {
                _sex = value;
                OnChanged(nameof(Sex));
            }
        }

        private string _address;
        public string Address
        {
            private get => _address;
            set
            {
                _address = value;
                OnChanged(nameof(Address));
            }
        }

        private DateTime _birthDate;
        public DateTime Birthday
        {
            private get => _birthDate;
            set
            {
                _birthDate = value;
                OnChanged(nameof(Birthday));
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            private get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnChanged(nameof(PhoneNumber));
            }
        }

        public void ClearData()
        {
            LName = string.Empty;
            FName = string.Empty;
            TName = string.Empty;
            Sex = string.Empty;
            Address = string.Empty;
            PhoneNumber = string.Empty;
            Timepick = string.Empty;
            DateRegistration = DateTime.UtcNow;
            Birthday = DateTime.UtcNow;
        }

        public bool ValidationData()
        {
            return (LName != string.Empty && FName != string.Empty && TName != string.Empty && Sex != string.Empty && PhoneNumber != string.Empty && Timepick != string.Empty);
        }

        public void SetPatient(ref Patient patient)
        {
            LName = patient.LastName;
            FName = patient.FirstName;
            TName = patient.ThridName;
            Sex = patient.Sex;
            Birthday = patient.Birthday;
            Address = patient.Address;
            PhoneNumber = patient.PhoneNumber;
        }

        public Entry GetEntry(Service service, IEmployees doc, IEmployees reg)
        {
            return new Entry() {
                RegistratorEntry = reg,
                DateRegistration = DateRegistration,
                PatientEntry = new Patient()
                {
                    LastName = LName,
                    FirstName = FName,
                    ThridName =TName,
                    Sex = Sex,
                    Birthday = Birthday,
                    Address = Address,
                    PhoneNumber = PhoneNumber
                },
                DoctorEntry = doc,
                ServiceEntry = service
            };
        }
    }
}
