    namespace NeplayGame.BagChal
{
    public enum GameMode
    {
        LocalTwoPlayer,
        PlayerVsAI
    }

    public enum AIDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public enum PlayerSide
    {
        Goat,
        Tiger
    }

    public static class GameModeSettings
    {
        public static GameMode CurrentMode { get; set; } = GameMode.LocalTwoPlayer;
        public static AIDifficulty Difficulty { get; set; } = AIDifficulty.Medium;
        public static PlayerSide PlayerSide { get; set; } = PlayerSide.Goat;
        public static EEntity AIEntity => PlayerSide == PlayerSide.Goat ? EEntity.Tiger : EEntity.Goat;
    }
}