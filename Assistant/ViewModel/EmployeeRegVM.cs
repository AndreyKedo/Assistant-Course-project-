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

        private DateTime _findEntry;
        public DateTime FindEntry
        {
            get => _findEntry;
            set
            {
                _findEntry = value;
                if(_findEntry != null)
                {
                    foreach (var item in DataAccess.FindEnties(_findEntry))
                    {
                       //НАПИСАТЬ ЛОГИКУ ОТОБРАЖЕНИЯ ЭЛЕМЕНТА ПОИСКА
                    }
                }
                OnChanged(nameof(FindEntry));
            }
        }

        public EmployeeRegVM(IDataAccessRegistrator dataAccess)
        {
            DataAccess = dataAccess;
            Update();
            EntryCreate = new CreateEntry();
            EntryCreate.ClearData();
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
        #endregion

        #region Методы для работы с коллекциями
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
            /*
             *             Patients.Clear();
            Entries.Clear();
            foreach (var item in DataAccess.GetEntries)
            {
                Entries.Add(item);
            }
            foreach (var item in DataAccess.GetPatient)
            {
                Patients.Add(item);
            }
             */
        }

        //Метод для выгрузки данных с data access
        private async void Update()
        {
            if (await DataAccess.UploadData())
            {
                Statys = "Данные обновлены";
                Patients = new ObservableCollection<Patient>(new ObservableCollection<Patient>(DataAccess.GetPatient));
                Entries = new ObservableCollection<Entry>(new ObservableCollection<Entry>(DataAccess.GetEntries));
                TypeServices = new ReadOnlyObservableCollection<TypeService>(new ObservableCollection<TypeService>(DataAccess.GetTypeService));
                Services = new ObservableCollection<Service>();
            }
        }
        #endregion
    }

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

        private string time;
        public string Timepick
        {
            get => time;
            set
            {
                time = value;
                if(time != string.Empty)
                    DateRegistration = DateRegistration.Add(TimeSpan.Parse(time));
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
            DateRegistration = DateTime.Today;
            Birthday = DateTime.Today;
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
                DateRegistration = DateRegistration.ToLocalTime(),
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
