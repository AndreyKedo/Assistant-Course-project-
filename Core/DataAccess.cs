using Core.Interface;
using Core.Model;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System.Linq;

namespace Core
{
    public class DataAccess : IDataAccessDoctor, IDataAccessRegistrator
    {
        private readonly IConnect connect;

        public List<Entry> GetEntries { get; private set; }
        public List<Patient> GetPatient { get; private set; }
        public List<TypeService> GetTypeService { get; private set; }

        public DataAccess(IConnect connect)
        {
            this.connect = connect;
            GetPatient = new List<Patient>();
            GetEntries = new List<Entry>();
            GetTypeService = new List<TypeService>();
        }

        public async Task<bool> UploadData()
        {
            bool isUpdate = false;

            if (connect.IsConnect())
            {
                //Выгрузка записей
                isUpdate = await UploadEntry();

                //Выгрузка пациентов
                string selectPatientQuery = "SELECT * FROM patient;";
                using (MySqlCommand command = new MySqlCommand(selectPatientQuery, connect.GetConnect))
                {
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            isUpdate = true;

                            while (await reader.ReadAsync())
                            {
                                GetPatient.Add(new Patient()
                                {
                                    Id = await reader.GetFieldValueAsync<uint>(0),
                                    LastName = reader.GetString(1),
                                    FirstName = reader.GetString(2),
                                    ThridName = reader.GetString(3),
                                    Sex = reader.GetString(4),
                                    Birthday = reader.GetDateTime(5),
                                    Address = reader.GetString(6),
                                    PhoneNumber = reader.GetString(7)
                                });
                            }
                        }
                        reader.Close();
                    }
                }
                //Выгрузка связи Тип услуги-Услуги-Специалист
                string selectServiceQuery = "SELECT * FROM type_service;";
                using (MySqlCommand command = new MySqlCommand(selectServiceQuery, connect.GetConnect))
                {
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            isUpdate = true;
                            while (await reader.ReadAsync())
                            {
                                GetTypeService.Add(new TypeService()
                                {
                                    Lable = reader.GetString(1),
                                    GetServiceOfDoctor = GetDoctorOfService(await reader.GetFieldValueAsync<uint>(0))
                                });
                            }
                        }
                        reader.Close();
                    }
                }
                connect.Close();
            }
            return isUpdate;
        }

        private async Task<bool> UploadEntry()
        {
            bool isUpdate = false;
            if (GetEntries.Count != 0)
            {
                GetEntries.Clear();
                string selectEntryQuery = "SELECT e.id, r.id, r.fio, e.data_registration, p.id, p.l_name, p.f_name, p.t_name, " +
                    "p.sex, p.birthday, p.address, p.phone_number, d.id, d.fio, d.cabinet_number, " +
                    "d.specialization, s.id, s.lable, s.price, s.action_service FROM entry AS e " +
                    "JOIN registrator AS r ON r.id = e.id_registrator " +
                    "JOIN patient AS p ON p.id = e.id_patient " +
                    "JOIN doctor AS d ON d.id = e.id_doctor " +
                    "JOIN service AS s ON s.id = e.id_service";
                using (MySqlCommand command = new MySqlCommand(selectEntryQuery, connect.GetConnect))
                {
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            isUpdate = true;
                            while(await reader.ReadAsync())
                            {
                                GetEntries.Add(new Entry()
                                {
                                    Id = await reader.GetFieldValueAsync<uint>(0),
                                    RegistratorEntry = new Registrator() { Id = await reader.GetFieldValueAsync<uint>(1), Fio = reader.GetString(2) },
                                    DateRegistration = reader.GetDateTime(3),
                                    PatientEntry = new Patient() {
                                        Id = await reader.GetFieldValueAsync<uint>(4),
                                        LastName = reader.GetString(5),
                                        FirstName = reader.GetString(6),
                                        ThridName = reader.GetString(7),
                                        Sex = reader.GetString(8),
                                        Birthday = reader.GetDateTime(9),
                                        Address = reader.GetString(10),
                                        PhoneNumber = reader.GetString(11)
                                    },
                                    DoctorEntry = new Doctor()
                                    {
                                        Id = await reader.GetFieldValueAsync<uint>(12),
                                        Fio = reader.GetString(13),
                                        CabinetNumber = reader.GetString(14),
                                        Specialization = reader.GetString(15)
                                    },
                                    ServiceEntry = new Service()
                                    {
                                        Id = await reader.GetFieldValueAsync<uint>(16),
                                        Lable = reader.GetString(17),
                                        Price = await reader.GetFieldValueAsync<uint>(18),
                                        Action = reader.GetString(19)
                                    }
                                });
                            }
                        }
                        reader.Close();
                    }
                }
            }
            return isUpdate;
        }

        public async Task<bool> UploadEntry(uint idDoc)
        {
            bool isUpdate = false;
            if (GetEntries.Count != 0)
            {
                GetEntries.Clear();
                string selectEntryQuery = "SELECT e.id, r.id, r.fio, e.data_registration, p.id, p.l_name, p.f_name, p.t_name, " +
                    "p.sex, p.birthday, p.address, p.phone_number, " +
                    "s.id, s.lable, s.price, s.action_service FROM entry AS e " +
                    "JOIN registrator AS r ON r.id = e.id_registrator " +
                    "JOIN patient AS p ON p.id = e.id_patient " +
                    "JOIN service AS s ON s.id = e.id_service " +
                    "WHERE e.id_doctor = @idDoc";
                using (MySqlCommand command = new MySqlCommand(selectEntryQuery, connect.GetConnect))
                {
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            isUpdate = true;
                            while (await reader.ReadAsync())
                            {
                                GetEntries.Add(new Entry()
                                {
                                    Id = await reader.GetFieldValueAsync<uint>(0),
                                    RegistratorEntry = new Registrator() { Id = await reader.GetFieldValueAsync<uint>(1), Fio = reader.GetString(2) },
                                    DateRegistration = reader.GetDateTime(3),
                                    PatientEntry = new Patient()
                                    {
                                        Id = await reader.GetFieldValueAsync<uint>(4),
                                        LastName = reader.GetString(5),
                                        FirstName = reader.GetString(6),
                                        ThridName = reader.GetString(7),
                                        Sex = reader.GetString(8),
                                        Birthday = reader.GetDateTime(9),
                                        Address = reader.GetString(10),
                                        PhoneNumber = reader.GetString(11)
                                    },
                                    ServiceEntry = new Service()
                                    {
                                        Id = await reader.GetFieldValueAsync<uint>(16),
                                        Lable = reader.GetString(17),
                                        Price = await reader.GetFieldValueAsync<uint>(18),
                                        Action = reader.GetString(19)
                                    }
                                });
                            }
                        }
                        reader.Close();
                    }
                }
            }
            return isUpdate;
        }

        public IEnumerable<Entry> FindEnties(System.DateTime date)
        {
            IEnumerable<Entry> entries = from entry in GetEntries
                                         where entry.DateRegistration == date
                                         orderby entry
                                         select entry;
            return entries;
        }

        public IEnumerable<Patient> FindPatient(string fio)
        {
            IEnumerable<Patient> patients = from patient in GetPatient
                                            let str = patient.FirstName + patient.LastName + patient.ThridName
                                            where str == fio
                                            select patient;
            return patients;
        }

        private Dictionary<List<Service>, Doctor> GetDoctorOfService(uint id)
        {
            Dictionary<List<Service>, Doctor> buffer = new Dictionary<List<Service>, Doctor>();
            if (connect.IsConnect())
            {
                string selectDoctor = "SELECT id, fio, cabinet_number, specialization FROM doctor";
                using (MySqlCommand command = new MySqlCommand(selectDoctor, connect.GetConnect))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            buffer.Add(GetServices(id, reader.GetFieldValue<uint>(0)), new Doctor()
                            {
                                Id = reader.GetFieldValue<uint>(0),
                                Fio = reader.GetString(1),
                                CabinetNumber = reader.GetString(2),
                                Specialization = reader.GetString(3)
                            });
                        }
                        reader.Close();
                    }
                }
                connect.Close();
            }
            return new Dictionary<List<Service>, Doctor>(buffer);
        }

        private List<Service> GetServices(uint idType, uint idDoc)
        {
            List<Service> buffer = new List<Service>();
            string selectService = "SELECT s.id, s.lable, s.price, s.action_service FROM service_of_doctor AS sod " +
                "JOIN service AS s ON s.id = sod.id_service " +
                "WHERE id_doctor = @idDoc AND s.id_type = @idType";
            if (connect.IsConnect())
            {
                using (MySqlCommand command = new MySqlCommand(selectService, connect.GetConnect))
                {
                    command.Parameters.AddWithValue("@idDoc", idDoc);
                    command.Parameters.AddWithValue("@idType", idType);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            buffer.Add(new Service()
                            {
                                Id = reader.GetFieldValue<uint>(0),
                                Lable = reader.GetString(1),
                                Price = reader.GetFieldValue<uint>(2),
                                Action = reader.GetString(3)
                            });
                        }
                        reader.Close();
                    }
                }
                connect.Close();
            }
            return buffer.GetRange(0, buffer.Count);
        }

        public Task AddEntry(Entry entry)
        {
            throw new System.NotImplementedException();
        }
    }
}
