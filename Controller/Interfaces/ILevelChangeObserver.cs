using PackMan.Interfaces;

namespace Controller.Interfaces
{
    public interface ILevelChangeObserver
    {
        void Update(IPlayer player);
    }
}
