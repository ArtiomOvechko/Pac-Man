namespace PackMan.Interfaces
{
    public interface ILevel
    {
         IField GameField { get; set; }

         IPac Pacman { get; set; }

         IGhost Blinky { get; }

         IGhost Pinky { get; }

         IGhost Inky { get; }

         IGhost Clyde { get; }

         IPlayer Player { get; set; }

         int FleeTime { get; set; }

         void PutOnDefault();

         void SetFlee();

         void SetNormal();
    }
}
