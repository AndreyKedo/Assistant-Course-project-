﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Model;

namespace Core.Interface
{
    public interface IDataAccessRegistrator
    {
        List<Entry> GetEntries { get; }
        List<Patient> GetPatient { get; }
        List<TypeService> GetTypeService { get; }
        Task AddEntry(Entry entry);
        Task AddEntry(Entry entry, uint idPatient);
        Task<string> GetPatientReport();
        Task<string> GetServiceOfDoctorReport();
        Task<bool> UploadData();
        IEnumerable<Entry> FindEntries(DateTime date);
        IEnumerable<Patient> FindPatient(string fio);
    }
}
