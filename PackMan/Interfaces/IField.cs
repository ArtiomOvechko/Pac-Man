using System;
using System.Collections.Generic;

namespace PackMan.Interfaces
{
    public interface IField
    {
        IObstacle[,] GameField { get; set; }
        int Height { get; }
        int Width { get; }
        IEnumerable<Tuple<IObstacle, int, int>> GetAllCells();
        bool Completed();
    }
}
