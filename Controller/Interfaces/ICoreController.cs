using System.Collections.Generic;
using System.Data;
using System.Windows.Input;

using PackMan.Interfaces;

namespace Controller.Interfaces
{
    public interface ICoreController
    {
        void NewGame();

        void NextLevel();

        void MoveUp();

        void MoveDown();

        void MoveRight();

        void MoveLeft();

        //void PressAction(KeyEventArgs e);

        //void ReleaseAction();

        void SetBehavior(int behaviorIndex, string path);

        void StopGameProcess();

        DataTable SelectRecords();

        List<string> GetLibraries { get; }

        IPlayer GetPlayer { get; }
    }
}
