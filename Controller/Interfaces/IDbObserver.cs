using RecordsDb.Interface;

namespace Controller.Interfaces
{
    public interface IDbObserver
    {
        void Update(IRecordsDatabase database);
    }
}
