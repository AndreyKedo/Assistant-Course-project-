using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Assistant.Interface;
using Core.Interface;
using Core.Model;
using System.Windows.Input;

namespace Assistant.ViewModel
{
    class EmployeeRegVM : BaseViewModel, IViewModel
    {
        readonly IDataAccessRegistrator DataAccess;

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
                if (_findPatient != string.Empty)
                {
                    Patients.Clear();
                    foreach (var item in DataAccess.FindPatient(_findPatient))
                    {
                        Patients.Add(item);
                    }
                }else
                {
                    UploadPatient();
                }
                OnChanged(nameof(FindPatient));
            }
        }

        public EmployeeRegVM(IDataAccessRegistrator dataAccess)
        {
            DataAccess = dataAccess;
            Services = new ObservableCollection<Service>();
            Entries = new ObservableCollection<Entry>();
            Update();
            EntryCreate = new CreateEntry();
            EntryCreate.ClearData();
            FindEntry = DateTime.UtcNow;
        }

        #region Команды
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
                UpdateLists();
            },(obj)=> 
            {
                return EntryCreate.ValidationData() && SelectedService != null;
            });
        }
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
            if (Entries.Count != 0)
                Entries.Clear();
            foreach (var item in DataAccess.FindEnties(date))
            {
                Entries.Add(item);
            }
        }

        //выгружает из буффера клиентов
        private void UploadPatient()
        {
            if (Patients.Count == 0)
            {
                foreach (var item in DataAccess.GetPatient)
                {
                    Patients.Add(item);
                }
            }
        }

        //выгружает из буффера записи
        private void UploadEntry()
        {
            if (Entries.Count == 0)
            {
                foreach(var item in DataAccess.GetEntries)
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
            }
        }
        #endregion
    }

    /*
     * Класс для создания записи из данных с формы
     * и валидации данных
     */
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
                if(_time != string.Empty && _time.Length == 5)
                {
                    if (char.IsDigit(_time[0]) && char.IsDigit(_time[1]) && char.IsDigit(_time[3]) && char.IsDigit(_time[4]))
                    {
                        char separator = _time[2];
                        if (_time[2] != ':')
                            _time = _time.Replace(separator, ':');
                        DateRegistration = DateRegistration.Add(TimeSpan.Parse(_time));
                    }
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
