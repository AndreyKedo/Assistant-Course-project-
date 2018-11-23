using Core.Interface;
using Core.Model;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System.Linq;
using System;

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

            //Выгрузка записей
            isUpdate = await UploadEntry();

            //Выгрузка пациентов
            isUpdate = await UploadPatient();

            if (connect.IsConnect())
            {
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

        public async Task<bool> UploadEntryForDoctor(uint idDoc)
        {
            bool isUpdate = false;
            if (connect.IsConnect())
            {
                string selectEntryQuery = "SELECT r.id, r.fio, e.date_registration, p.id, p.l_name, p.f_name, p.t_name, " +
                "p.sex, p.birthday, p.address, p.phone_number, " +
                "s.id, s.lable, s.price, s.action_service FROM entry AS e " +
                "JOIN registrator AS r ON r.id = e.id_registrator " +
                "JOIN patient AS p ON p.id = e.id_patient " +
                "JOIN service AS s ON s.id = e.id_service " +
                "WHERE e.id_doctor = @idDoc AND e.date_registration > LAST_DAY(CURDATE()) + INTERVAL 1 DAY - INTERVAL 1 MONTH AND " +
                "e.date_registration < DATE_ADD(LAST_DAY(CURDATE()), INTERVAL 1 DAY)";
                using (MySqlCommand command = new MySqlCommand(selectEntryQuery, connect.GetConnect))
                {
                    command.Parameters.AddWithValue("@idDoc", idDoc);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            isUpdate = true;
                            while (await reader.ReadAsync())
                            {
                                if (reader.GetDateTime(2).Date == DateTime.Now.Date)
                                {
                                    GetEntries.Add(new Entry()
                                    {
                                        RegistratorEntry = new Registrator() { Id = await reader.GetFieldValueAsync<uint>(0), Fio = reader.GetString(1) },
                                        DateRegistration = reader.GetDateTime(2),
                                        PatientEntry = new Patient()
                                        {
                                            Id = await reader.GetFieldValueAsync<uint>(3),
                                            LastName = reader.GetString(4),
                                            FirstName = reader.GetString(5),
                                            ThridName = reader.GetString(6),
                                            Sex = reader.GetString(7),
                                            Birthday = reader.GetDateTime(8),
                                            Address = reader.GetString(9),
                                            PhoneNumber = reader.GetString(10)
                                        },
                                        ServiceEntry = new Service()
                                        {
                                            Id = await reader.GetFieldValueAsync<uint>(11),
                                            Lable = reader.GetString(12),
                                            Price = await reader.GetFieldValueAsync<uint>(13),
                                            Action = reader.GetString(14)
                                        }
                                    });
                                }
                            }
                        }
                        reader.Close();
                    }
                }

                connect.Close();
            }
            return isUpdate;
        }

        public IEnumerable<Entry> GetEntriesOfCurrDay()
        {
            IEnumerable<Entry> entries = from item in GetEntries
                                         where item.DateRegistration.Date == DateTime.Now.Date
                                         select item;
            return entries;
        }

        public IEnumerable<Entry> GetEntriesOfBack()
        {
            IEnumerable<Entry> entries = from item in GetEntries
                                         where item.DateRegistration.Date < DateTime.Now.Date
                                         select item;
            return entries;
         }

        public async Task AddEntry(Entry entry)
        {
            if (connect.IsConnect())
            {
                string insertPatientQuery = "INSERT INTO patient(l_name, f_name, t_name, sex, birthday, address, phone_number) " +
                    "VALUES( @lName, @fName, @tName, @sex, @birthday, @address, @number );";
                using (MySqlCommand command = new MySqlCommand(insertPatientQuery, connect.GetConnect))
                {
                    command.Parameters.AddWithValue("@lName", entry.PatientEntry.LastName);
                    command.Parameters.AddWithValue("@fName", entry.PatientEntry.FirstName);
                    command.Parameters.AddWithValue("@tName", entry.PatientEntry.ThridName);
                    command.Parameters.AddWithValue("@sex", entry.PatientEntry.Sex);
                    command.Parameters.AddWithValue("@birthday", entry.PatientEntry.Birthday);
                    command.Parameters.AddWithValue("@address", entry.PatientEntry.Address);
                    command.Parameters.AddWithValue("@number", entry.PatientEntry.PhoneNumber);
                    await command.ExecuteNonQueryAsync();
                }

                string insertEntryQuery = "INSERT INTO entry(id_registrator, date_registration, id_patient, id_doctor, id_service) " +
                    "VALUES( @idReg, @dateReg, (SELECT id FROM patient WHERE l_name = @lName AND f_name = @fName AND t_name = @tName), @idDoc, @idServ );";
                using (MySqlCommand command = new MySqlCommand(insertEntryQuery, connect.GetConnect))
                {
                    command.Parameters.AddWithValue("@idReg", entry.RegistratorEntry.Id);
                    command.Parameters.AddWithValue("@dateReg", entry.DateRegistration);
                    command.Parameters.AddWithValue("@idPati", entry.PatientEntry.Id);
                    command.Parameters.AddWithValue("@idDoc", entry.DoctorEntry.Id);
                    command.Parameters.AddWithValue("@idServ", entry.ServiceEntry.Id);
                    command.Parameters.AddWithValue("@lName", entry.PatientEntry.LastName);
                    command.Parameters.AddWithValue("@fName", entry.PatientEntry.FirstName);
                    command.Parameters.AddWithValue("@tName", entry.PatientEntry.ThridName);
                    await command.ExecuteNonQueryAsync();
                }
                connect.Close();
            }

            await UploadEntry();
            await UploadPatient();
        }

        public async Task AddEntry(Entry entry, uint idPatient)
        {
            if (connect.IsConnect())
            {
                string insertEntryQuery = "INSERT INTO entry(id_registrator, date_registration, id_patient, id_doctor, id_service) " +
                    "VALUES( @idReg, @dateReg, @idPati, @idDoc, @idServ );";
                using (MySqlCommand command = new MySqlCommand(insertEntryQuery, connect.GetConnect))
                {
                    DateTime t = entry.DateRegistration;
                    command.Parameters.AddWithValue("@idReg", entry.RegistratorEntry.Id);
                    command.Parameters.AddWithValue("@dateReg", t);
                    command.Parameters.AddWithValue("@idPati", idPatient);
                    command.Parameters.AddWithValue("@idDoc", entry.DoctorEntry.Id);
                    command.Parameters.AddWithValue("@idServ", entry.ServiceEntry.Id);
                    await command.ExecuteNonQueryAsync();
                }
                connect.Close();
            }

            await UploadEntry();
            await UploadPatient();
        }

        public IEnumerable<Entry> FindEntries(System.DateTime date)
        {
            IEnumerable<Entry> entries = from entry in GetEntries
                                         where entry.DateRegistration.Day == date.Day
                                         select entry;
            return entries;
        }

        public IEnumerable<Patient> FindPatient(string fio)
        {
            string[] fioSplit = new string[3];
            fio.Split(new char[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries).CopyTo(fioSplit, 0);
            for (int i = 0; i < 3; i++)
            {
                if (fioSplit[i] != null)
                {
                    if (char.IsLower(fioSplit[i][0]))
                    {
                        char[] buff = fioSplit[i].ToCharArray();
                        buff[0] = char.ToUpperInvariant(buff[0]);
                        fioSplit[i] = new string(buff);
                    }
                }
            }
            IEnumerable<Patient> patients = from patient in GetPatient
                                            where patient.LastName == fioSplit[0] || patient.FirstName == fioSplit[1] || patient.ThridName == fioSplit[2]
                                            select patient;
            return patients;
        }

        public async Task<string> GetPatientReport()
        {
            string buffer = string.Empty;
            if (connect.IsConnect())
            {
                string selectServiceOfPatientQuery = "SELECT p.l_name, p.f_name, p.t_name, date_registration, lable, price FROM entry AS e " +
                    "JOIN patient AS p ON p.id = e.id_patient " +
                    "JOIN service AS s ON s.id = e.id_service " +
                    "WHERE e.date_registration > LAST_DAY(CURDATE()) + INTERVAL 1 DAY - INTERVAL 1 MONTH AND " +
                    "e.date_registration<DATE_ADD(LAST_DAY(CURDATE()), INTERVAL 1 DAY)";
                using(MySqlCommand command = new MySqlCommand(selectServiceOfPatientQuery, connect.GetConnect))
                {
                    using(DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                buffer += reader.GetString(0) + ' ' + reader.GetString(1) + ' ' + reader.GetString(2) +
                                    ' ' + reader.GetString(3) + ' ' + reader.GetString(4) + ' ' + reader.GetString(5) + '\n';
                            }
                        }
                        else
                        {
                            buffer = "Нет записей для формирования отчёта";
                        }
                        reader.Close();
                    }
                }
                connect.Close();
            }
            return buffer;
        }

        public async Task<string> GetServiceOfDoctorReport()
        {
            string buffer = string.Empty;
            if (connect.IsConnect())
            {
                string selectServiceOfDoctorQuery = "SELECT d.fio, s.lable, e.date_registration FROM entry AS e " +
                    "JOIN doctor AS d ON d.id = e.id_doctor " +
                    "JOIN service AS s ON s.id = e.id_service " +
                    "WHERE e.date_registration > LAST_DAY(CURDATE()) + INTERVAL 1 DAY - INTERVAL 1 MONTH AND " +
                    "e.date_registration < DATE_ADD(LAST_DAY(CURDATE()), INTERVAL 1 DAY)";
                using (MySqlCommand command = new MySqlCommand(selectServiceOfDoctorQuery, connect.GetConnect))
                {
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                buffer += reader.GetString(0) + ' ' + reader.GetString(1) + ' ' + reader.GetString(2) + '\n';
                            }
                        }
                        else
                        {
                            buffer = "Нет записей для формирования отчёта";
                        }
                        reader.Close();
                    }
                }
                connect.Close();
            }
            return buffer;
        }

        private Dictionary<Service, Doctor> GetDoctorOfService(uint idType)
        {
            Dictionary<Service, Doctor> buffer = new Dictionary<Service, Doctor>();
            if (connect.IsConnect())
            {
                string selectDoctor = "SELECT s.id, s.lable, s.price, s.action_service, d.id, d.fio, d.cabinet_number, d.specialization FROM service_of_doctor AS sod " +
                    "JOIN service AS s ON s.id = sod.id_service " +
                    "JOIN doctor AS d ON d.id = sod.id_doctor " +
                    "WHERE s.id_type = @idType";
                using (MySqlCommand command = new MySqlCommand(selectDoctor, connect.GetConnect))
                {
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
                            }, new Doctor()
                            {
                                Id = reader.GetFieldValue<uint>(4),
                                Fio = reader.GetString(5),
                                CabinetNumber = reader.GetString(6),
                                Specialization = reader.GetString(7)
                            });
                        }
                        reader.Close();
                    }
                }
                connect.Close();
            }
            return new Dictionary<Service, Doctor>(buffer);
        }

        private async Task<bool> UploadPatient()
        {
            bool isUpload = false;
            if (connect.IsConnect())
            {
                string selectPatientQuery = "SELECT * FROM patient;";
                if (GetPatient.Count != 0)
                    GetPatient.Clear();

                using (MySqlCommand command = new MySqlCommand(selectPatientQuery, connect.GetConnect))
                {
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            isUpload = true;
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
                connect.Close();
            }
            return isUpload;
        }

        private async Task<bool> UploadEntry()
        {
            bool isUpdate = false;
            if (connect.IsConnect())
            {
                if (GetEntries.Count != 0)
                    GetEntries.Clear();

                string selectEntryQuery = "SELECT r.id, r.fio, e.date_registration, p.id, p.l_name, p.f_name, p.t_name, " +
                    "p.sex, p.birthday, p.address, p.phone_number, d.id, d.fio, d.cabinet_number, " +
                    "d.specialization, s.id, s.lable, s.price, s.action_service FROM entry AS e " +
                    "JOIN registrator AS r ON r.id = e.id_registrator " +
                    "JOIN patient AS p ON p.id = e.id_patient " +
                    "JOIN doctor AS d ON d.id = e.id_doctor " +
                    "JOIN service AS s ON s.id = e.id_service " +
                    "WHERE e.date_registration > LAST_DAY(CURDATE()) + INTERVAL 1 DAY - INTERVAL 1 MONTH AND " +
                    "e.date_registration<DATE_ADD(LAST_DAY(CURDATE()), INTERVAL 1 DAY)";
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
                                    RegistratorEntry = new Registrator() { Id = await reader.GetFieldValueAsync<uint>(0), Fio = reader.GetString(1) },
                                    DateRegistration = reader.GetDateTime(2),
                                    PatientEntry = new Patient()
                                    {
                                        Id = await reader.GetFieldValueAsync<uint>(3),
                                        LastName = reader.GetString(4),
                                        FirstName = reader.GetString(5),
                                        ThridName = reader.GetString(6),
                                        Sex = reader.GetString(7),
                                        Birthday = reader.GetDateTime(8).ToUniversalTime(),
                                        Address = reader.GetString(9),
                                        PhoneNumber = reader.GetString(10)
                                    },
                                    DoctorEntry = new Doctor()
                                    {
                                        Id = await reader.GetFieldValueAsync<uint>(11),
                                        Fio = reader.GetString(12),
                                        CabinetNumber = reader.GetString(13),
                                        Specialization = reader.GetString(14)
                                    },
                                    ServiceEntry = new Service()
                                    {
                                        Id = await reader.GetFieldValueAsync<uint>(15),
                                        Lable = reader.GetString(16),
                                        Price = await reader.GetFieldValueAsync<uint>(17),
                                        Action = reader.GetString(18)
                                    }
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
    }
}
