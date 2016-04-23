using System.Collections.Generic;
using System.Data;

using PackMan.Interfaces;

namespace Controller.Interfaces
{
    public interface ICoreController: IPacManObservable
    {
        void NewGame();

        void NextLevel();

        void Move(bool condition);

        void MoveUp();

        void MoveDown();

        void MoveRight();

        void MoveLeft();

        IExceptionHandler GetExceptionObservable { get; }

        List<string> GetLibraries { get; }

        IPlayer GetPlayer { get; }

        void SetBehavior(int behaviorIndex, string path);

        void StopGameProcess();

        DataTable SelectRecords();

        void DeleteRecords();

        void InsertRecords(IPlayer player);
    }
}
