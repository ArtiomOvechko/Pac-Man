using System.Collections.Generic;
using System.Data;
using System.Windows.Input;
using PackMan.Interfaces;

namespace Controller.Interface
{
    public interface ICoreController
    {
        void NewGame();

        void NextLevel();

        void PressAction(KeyEventArgs e);

        void ReleaseAction();

        void SetBehavior(string name, string path);

        void StopGameProcess();

        ICommand ResetScore { get; }

        DataTable SelectRecord();

        List<string> GetLibraries { get; }

        IPlayer GetPlayer { get; }
    }
}
