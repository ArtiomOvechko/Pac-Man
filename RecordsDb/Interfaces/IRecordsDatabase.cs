using System.Data;
using PackMan.Interfaces;

namespace RecordsDb.Interfaces
{
    public interface IRecordsDatabase
    {
        DataTable SelectRecords();
        void AddRecord(IPlayer player);
        void DeleteRecords();
    }
}
