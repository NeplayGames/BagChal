using System.Collections.Generic;
using NeplayGame.BagChal.Entity;
using UnityEngine;

namespace NeplayGame.BagChal.AI
{
    public sealed class TigerAI : IBaghChalAI
    {
        private readonly AIDifficulty difficulty;

        public TigerAI(AIDifficulty difficulty) => this.difficulty = difficulty;

        public AIMove ChooseMove(int goatsLeftToPlace, IReadOnlyList<SpawnPoint> points,
            IReadOnlyDictionary<SpawnPoint, EntityController> board)
        {
            List<AIMove> moves = GetMoves(board);
            if (moves.Count == 0) return AIMove.None;
            if (difficulty == AIDifficulty.Easy) return RandomMove(moves);

            List<AIMove> captures = moves.FindAll(move => move.IsCapture);
            List<AIMove> choices = captures.Count > 0 ? captures : moves;
            if (difficulty == AIDifficulty.Medium) return RandomMove(choices);

            AIMove best = choices[0];
            int bestScore = Score(best, board);
            for (int i = 1; i < choices.Count; i++)
            {
                int score = Score(choices[i], board);
                if (score > bestScore) { best = choices[i]; bestScore = score; }
            }
            return best;
        }

        private static List<AIMove> GetMoves(IReadOnlyDictionary<SpawnPoint, EntityController> board)
        {
            List<AIMove> moves = new();
            foreach (var entry in board)
            {
                if (entry.Value.eEntity != EEntity.Tiger) continue;
                foreach (SpawnPoint middle in entry.Key.movablePoint)
                {
                    if (!IsEntity(board, middle, EEntity.Goat))
                    {
                        if (!board.ContainsKey(middle)) moves.Add(AIMove.Move(entry.Key, middle));
                        continue;
                    }
                    foreach (SpawnPoint landing in middle.movablePoint)
                    {
                        if (!board.ContainsKey(landing) && Collinear(entry.Key, middle, landing))
                            moves.Add(AIMove.Capture(entry.Key, landing, middle));
                    }
                }
            }
            return moves;
        }

        private static int Score(AIMove move, IReadOnlyDictionary<SpawnPoint, EntityController> board)
        {
            int score = move.IsCapture ? 1000 : 0;
            score += move.Destination.movablePoint.Count * 8;
            foreach (SpawnPoint point in move.Destination.movablePoint)
            {
                if (IsEntity(board, point, EEntity.Goat) && point != move.CapturedGoat) score += 25;
                if (!board.ContainsKey(point) || point == move.Origin || point == move.CapturedGoat) score += 4;
            }
            return score + Random.Range(0, 4);
        }

        private static AIMove RandomMove(List<AIMove> moves) => moves[Random.Range(0, moves.Count)];
        private static bool IsEntity(IReadOnlyDictionary<SpawnPoint, EntityController> board, SpawnPoint point, EEntity entity) =>
            board.TryGetValue(point, out EntityController value) && value.eEntity == entity;
        private static bool Collinear(SpawnPoint a, SpawnPoint b, SpawnPoint c)
        {
            Vector2 first = new Vector2(b.transform.position.x - a.transform.position.x, b.transform.position.z - a.transform.position.z);
            Vector2 second = new Vector2(c.transform.position.x - a.transform.position.x, c.transform.position.z - a.transform.position.z);
            return Mathf.Approximately(first.x * second.y - first.y * second.x, 0f);
        }
    }
}