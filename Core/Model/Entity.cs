using Core.Interface;
using System.Collections.Generic;

namespace Core.Model
{
    public class Doctor : IEmployees
    {
        public uint Id { get; set; }
        public string Fio { get; set; }
        public string CabinetNumber { get; set; }
        public string Specialization { get; set; }
    }

    public class Registrator : IEmployees
    {
        public uint Id { get; set; }
        public string Fio { get; set; }
    }

    public class Patient
    {
        public uint Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string ThridName { get; set; }
        public string Sex { get; set; }
        public System.DateTime Birthday { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class Service
    {
        public uint Id { get; set; }
        public string Lable { get; set; }
        public uint Price { get; set; }
        public string Action { get; set; }
    }

    public class TypeService
    {
        public Dictionary<List<Service>, Doctor> GetServiceOfDoctor { get; set; }
        public string Lable { get; set; }
    }

    public class Entry
    {
        public uint Id { get; set; }
        public IEmployees RegistratorEntry { get; set; }
        public System.DateTime DateRegistration { get; set; }
        public Patient PatientEntry { get; set; }
        public IEmployees DoctorEntry { get; set; }
        public Service ServiceEntry { get; set; }
    }
}
