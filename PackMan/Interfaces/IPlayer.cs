namespace PackMan.Interfaces
{
    public interface IPlayer
    {
        int Lives { get; set; }

        int Score { get; set; }

        int ScoreTrack { get; set; }

        ILevel Level { get; set; }

        int LevelNumber { get; set; }

        void CheckCondition();
    }
}
