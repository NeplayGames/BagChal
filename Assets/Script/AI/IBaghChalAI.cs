using System.Collections.Generic;
using NeplayGame.BagChal.Entity;

namespace NeplayGame.BagChal.AI
{
    public interface IBaghChalAI
    {
        AIMove ChooseMove(int goatsLeftToPlace, IReadOnlyList<SpawnPoint> points,
            IReadOnlyDictionary<SpawnPoint, EntityController> board);
    }

    public readonly struct AIMove
    {
        public static AIMove None => default;
        public SpawnPoint Origin { get; }
        public SpawnPoint Destination { get; }
        public SpawnPoint CapturedGoat { get; }
        public bool IsValid => Destination != null;
        public bool IsPlacement => Origin == null && Destination != null;
        public bool IsCapture => CapturedGoat != null;

        private AIMove(SpawnPoint origin, SpawnPoint destination, SpawnPoint captured)
        {
            Origin = origin;
            Destination = destination;
            CapturedGoat = captured;
        }

        public static AIMove Place(SpawnPoint destination) => new AIMove(null, destination, null);
        public static AIMove Move(SpawnPoint origin, SpawnPoint destination) => new AIMove(origin, destination, null);
        public static AIMove Capture(SpawnPoint origin, SpawnPoint destination, SpawnPoint goat) => new AIMove(origin, destination, goat);
    }
}