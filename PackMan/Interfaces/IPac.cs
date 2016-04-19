namespace PackMan.Interfaces
{
    public interface IPac
    {
        int X { get; }

        int Y { get; }

        ILevel Level { get; }

        int Direction { get; set; }

        bool Moving { get; set; }

        void Move();

        void EatPoint(int a, int b);

        void PutOnDefault();
    }
}
