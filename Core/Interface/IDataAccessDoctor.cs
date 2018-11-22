using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IDataAccessDoctor
    {
        List<Entry> GetEntries { get; }
        List<Patient> GetPatient { get; }
        Task<bool> UploadEntryForDoctor(uint idDoc);
    }
}
