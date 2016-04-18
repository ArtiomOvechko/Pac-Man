﻿using System.Collections.Generic;
using System.Data;
using System.Windows.Input;
using PackMan.Interfaces;
using RecordsDb.Interfaces;

namespace Controller.Interfaces
{
    public interface ICoreController
    {
        void NewGame();
        void NextLevel();
        void PressAction(KeyEventArgs e);
        void ReleaseAction();
        void SetBehavior(string name, string path);
        void StopGameProcess();
        void ResetScore();
        DataTable SelectRecord();
        List<string> GetLibraries { get; }
        IPlayer GetPlayer { get; }
    }
}