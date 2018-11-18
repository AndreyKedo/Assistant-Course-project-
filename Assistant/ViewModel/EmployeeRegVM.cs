using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Assistant.Interface;
using Core.Interface;
using Core.Model;

namespace Assistant.ViewModel
{
    class EmployeeRegVM : BaseViewModel, IViewModel
    {
        readonly IDataAccessRegistrator DataAccess;

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

        private List<Service> _services;
        public List<Service> Services
        {
            get => _services;
            set
            {
                _services = value;
                OnChanged(nameof(Services));
            }
        }

        private TypeService _selectService;
        public TypeService SelectService
        {
            get => _selectService;
            set
            {
                _selectService = value;
                foreach (List<Service> services in _selectService.GetServiceOfDoctor.Keys)
                {
                    Services = services;
                }
                OnChanged(nameof(SelectService));
            }
        }

        public ReadOnlyObservableCollection<Patient> Patients { get; set; }
        public ReadOnlyObservableCollection<Entry> Entries { get; set; }
        public List<TypeService> TypeServices { get; set; }

        public EmployeeRegVM(IDataAccessRegistrator dataAccess)
        {
            DataAccess = dataAccess;
            Update();
        }


        private async void Update()
        {
            if(await DataAccess.UploadData())
            {
                Statys = "Данные обновлены";
                Patients = new ReadOnlyObservableCollection<Patient>(new ObservableCollection<Patient>(DataAccess.GetPatient));
                Entries = new ReadOnlyObservableCollection<Entry>(new ObservableCollection<Entry>(DataAccess.GetEntries));
                TypeServices = new List<TypeService>(DataAccess.GetTypeService);
            }
        }
    }
}
