namespace Controller.Interfaces
{
    public interface IPacManObservable
    {
        void RegisterDbObserver(IDbObserver observer);

        void RegisterLevelChangeObserver(ILevelChangeObserver observer);

        void RegisterMovingObserver(IMovingObserver observer);

        void RegisterPluginsObserver(IPluginsObserver observer);

        void RemoveDbObserver(IDbObserver observer);

        void RemoveLevelChangeObserver(ILevelChangeObserver observer);

        void RemoveMovingObservers(IMovingObserver observer);

        void RemovePluginsObservers(IPluginsObserver observer);

        void NotifyDbObservers();

        void NotifyMovingObservers();

        void NotifyLevelChangeObservers();

        void NotifyPluginsObservers();
    }
}
