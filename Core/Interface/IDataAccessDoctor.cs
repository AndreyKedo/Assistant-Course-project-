using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IDataAccessDoctor
    {
        List<Entry> GetEntries { get; }
        IEnumerable<Entry> GetEntriesOfCurrDay();
        IEnumerable<Entry> GetEntriesOfBack();
        Task<bool> UploadEntryForDoctor(uint idDoc);
    }
}
