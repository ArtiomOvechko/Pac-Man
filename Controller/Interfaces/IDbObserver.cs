using System.Data;
using RecordsDb.Interface;

namespace Controller.Interfaces
{
    public interface IDbObserver
    {
        void Update(DataTable data);
    }
}
