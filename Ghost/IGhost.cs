using PackMan.Interfaces;

namespace Ghost
{
    public interface IGhost
    {
        int X { get; set; }
        int Y { get; set; }
        int OldX { get; set; }
        int OldY { get; set; }
        string Condition { get; set; }
        ILevel Level { get; }
        void Move();
        void PutOn(int x, int y);
    }
}
