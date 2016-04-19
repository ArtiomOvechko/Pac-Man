using System.Data;

using PackMan.Interfaces;

namespace RecordsDb.Interface
{
    public interface IRecordsDatabase
    {
        DataTable SelectRecords();

        void AddRecord(IPlayer player);

        void DeleteRecords();
    }
}
