using System;
using System.Collections.Generic;
using NeplayGame.BagChal.Entity;
using UnityEngine;

namespace NeplayGame.BagChal.AI
{
    public sealed class GoatAI : IBaghChalAI
    {
        private readonly AIDifficulty difficulty;

        public GoatAI(AIDifficulty difficulty)
        {
            this.difficulty = difficulty;
        }

        public AIMove ChooseMove(int goatsLeftToPlace, IReadOnlyList<SpawnPoint> boardPoints,
            IReadOnlyDictionary<SpawnPoint, EntityController> board)
        {
            bool placementPhase = goatsLeftToPlace > 0;
            List<AIMove> moves = placementPhase
                ? GetGoatPlacements(boardPoints, board)
                : GetGoatMoves(board);
            return SelectMove(moves, board, boardPoints, goatsLeftToPlace);
        }
        private static List<AIMove> GetGoatPlacements(
            IReadOnlyList<SpawnPoint> boardPoints,
            IReadOnlyDictionary<SpawnPoint, EntityController> board)
        {
            List<AIMove> moves = new();
            foreach (SpawnPoint point in boardPoints)
            {
                if (!board.ContainsKey(point))
                    moves.Add(AIMove.Place(point));
            }
            return moves;
        }

        private static List<AIMove> GetGoatMoves(IReadOnlyDictionary<SpawnPoint, EntityController> board)
        {
            List<AIMove> moves = new();
            foreach (KeyValuePair<SpawnPoint, EntityController> entry in board)
            {
                if (entry.Value.eEntity != EEntity.Goat)
                    continue;

                foreach (SpawnPoint destination in entry.Key.movablePoint)
                {
                    if (!board.ContainsKey(destination))
                        moves.Add(AIMove.Move(entry.Key, destination));
                }
            }
            return moves;
        }

        private AIMove SelectMove(List<AIMove> moves,
            IReadOnlyDictionary<SpawnPoint, EntityController> board,
            IReadOnlyList<SpawnPoint> boardPoints,
            int goatsLeftToPlace)
        {
            if (moves.Count == 0) return AIMove.None;
            if (difficulty == AIDifficulty.Easy)
                return moves[UnityEngine.Random.Range(0, moves.Count)];

            List<AIMove> safestMoves = KeepLowestCaptureRisk(moves, board);
            if (difficulty == AIDifficulty.Medium)
                return SelectTopGoatMove(safestMoves, board, 3);
            return SelectHardGoatMove(safestMoves, board, boardPoints, goatsLeftToPlace);
        }
        private static List<AIMove> KeepLowestCaptureRisk(
            List<AIMove> moves,
            IReadOnlyDictionary<SpawnPoint, EntityController> board)
        {
            Dictionary<SpawnPoint, EEntity> state = CreateState(board);
            List<AIMove> safest = new();
            int lowestCaptureCount = int.MaxValue;

            foreach (AIMove move in moves)
            {
                Dictionary<SpawnPoint, EEntity> result = ApplyMove(state, move, EEntity.Goat);
                int captureCount = CountTigerCaptures(result);
                if (captureCount < lowestCaptureCount)
                {
                    lowestCaptureCount = captureCount;
                    safest.Clear();
                    safest.Add(move);
                }
                else if (captureCount == lowestCaptureCount)
                {
                    safest.Add(move);
                }
            }
            return safest.Count > 0 ? safest : moves;
        }

        private static int CountTigerCaptures(Dictionary<SpawnPoint, EEntity> state)
        {
            int captures = 0;
            foreach (AIMove move in GetTigerMoves(state))
            {
                if (move.IsCapture)
                    captures++;
            }
            return captures;
        }
        private static AIMove SelectHardGoatMove(
            List<AIMove> goatMoves,
            IReadOnlyDictionary<SpawnPoint, EntityController> board,
            IReadOnlyList<SpawnPoint> boardPoints,
            int goatsLeftToPlace)
        {
            Dictionary<SpawnPoint, EEntity> currentState = CreateState(board);
            AIMove bestMove = goatMoves[0];
            int bestWorstCase = int.MinValue;

            foreach (AIMove goatMove in goatMoves)
            {
                Dictionary<SpawnPoint, EEntity> afterGoat = ApplyMove(currentState, goatMove, EEntity.Goat);
                List<AIMove> tigerReplies = GetTigerMoves(afterGoat);
                int worstReplyScore = EvaluateGoatState(afterGoat);

                if (tigerReplies.Count > 0)
                {
                    worstReplyScore = int.MaxValue;
                    foreach (AIMove tigerReply in tigerReplies)
                    {
                        Dictionary<SpawnPoint, EEntity> afterTiger = ApplyMove(afterGoat, tigerReply, EEntity.Tiger);
                        int bestRecovery = FindBestGoatRecovery(afterTiger, boardPoints, goatsLeftToPlace > 1);
                        if (bestRecovery < worstReplyScore)
                            worstReplyScore = bestRecovery;
                    }
                }

                worstReplyScore += GetStrategicPositionBonus(goatMove, goatsLeftToPlace);

                if (worstReplyScore > bestWorstCase)
                {
                    bestWorstCase = worstReplyScore;
                    bestMove = goatMove;
                }
            }

            return bestMove;
        }

        private static int GetStrategicPositionBonus(AIMove move, int goatsLeftToPlace)
        {
            int connectivity = move.Destination.movablePoint.Count;
            int bonus = connectivity * 18;
            if (goatsLeftToPlace > 12)
                bonus += connectivity * 14;
            if (move.Origin != null && move.Origin.movablePoint.Count > connectivity)
                bonus -= 30;
            return bonus;
        }
        private static int FindBestGoatRecovery(
            Dictionary<SpawnPoint, EEntity> state,
            IReadOnlyList<SpawnPoint> boardPoints,
            bool placementPhase)
        {
            List<AIMove> replies = placementPhase
                ? GetGoatPlacements(boardPoints, state)
                : GetGoatMoves(state);

            if (replies.Count == 0)
                return EvaluateGoatState(state);

            int bestScore = int.MinValue;
            foreach (AIMove reply in replies)
            {
                int score = EvaluateGoatState(ApplyMove(state, reply, EEntity.Goat));
                if (score > bestScore)
                    bestScore = score;
            }
            return bestScore;
        }

        private static List<AIMove> GetGoatPlacements(
            IReadOnlyList<SpawnPoint> boardPoints,
            Dictionary<SpawnPoint, EEntity> state)
        {
            List<AIMove> moves = new();
            foreach (SpawnPoint point in boardPoints)
            {
                if (!state.ContainsKey(point))
                    moves.Add(AIMove.Place(point));
            }
            return moves;
        }

        private static List<AIMove> GetGoatMoves(Dictionary<SpawnPoint, EEntity> state)
        {
            List<AIMove> moves = new();
            foreach (KeyValuePair<SpawnPoint, EEntity> entry in state)
            {
                if (entry.Value != EEntity.Goat)
                    continue;
                foreach (SpawnPoint destination in entry.Key.movablePoint)
                {
                    if (!state.ContainsKey(destination))
                        moves.Add(AIMove.Move(entry.Key, destination));
                }
            }
            return moves;
        }

        private static Dictionary<SpawnPoint, EEntity> CreateState(
            IReadOnlyDictionary<SpawnPoint, EntityController> board)
        {
            Dictionary<SpawnPoint, EEntity> state = new();
            foreach (KeyValuePair<SpawnPoint, EntityController> entry in board)
                state[entry.Key] = entry.Value.eEntity;
            return state;
        }
        private static Dictionary<SpawnPoint, EEntity> ApplyMove(
            Dictionary<SpawnPoint, EEntity> state,
            AIMove move,
            EEntity entity)
        {
            Dictionary<SpawnPoint, EEntity> result = new(state);
            if (move.Origin != null)
                result.Remove(move.Origin);
            if (move.CapturedGoat != null)
                result.Remove(move.CapturedGoat);
            result[move.Destination] = entity;
            return result;
        }

        private static List<AIMove> GetTigerMoves(Dictionary<SpawnPoint, EEntity> state)
        {
            List<AIMove> moves = new();
            foreach (KeyValuePair<SpawnPoint, EEntity> entry in state)
            {
                if (entry.Value != EEntity.Tiger)
                    continue;

                foreach (SpawnPoint middle in entry.Key.movablePoint)
                {
                    EEntity middleEntity = GetEntity(state, middle);
                    if (middleEntity == EEntity.None)
                    {
                        moves.Add(AIMove.Move(entry.Key, middle));
                        continue;
                    }

                    if (middleEntity != EEntity.Goat)
                        continue;

                    foreach (SpawnPoint landing in middle.movablePoint)
                    {
                        if (GetEntity(state, landing) == EEntity.None && AreCollinear(entry.Key, middle, landing))
                            moves.Add(AIMove.Capture(entry.Key, landing, middle));
                    }
                }
            }
            return moves;
        }

        private static int EvaluateGoatState(Dictionary<SpawnPoint, EEntity> state)
        {
            int goatCount = 0;
            int goatConnections = 0;
            int tigerMobility = 0;
            int tigerCaptures = 0;
            int trappedTigers = 0;

            foreach (KeyValuePair<SpawnPoint, EEntity> entry in state)
            {
                if (entry.Value == EEntity.Goat)
                {
                    goatCount++;
                    foreach (SpawnPoint neighbour in entry.Key.movablePoint)
                    {
                        if (GetEntity(state, neighbour) == EEntity.Goat)
                            goatConnections++;
                    }
                    continue;
                }

                if (entry.Value != EEntity.Tiger)
                    continue;

                int options = 0;
                foreach (SpawnPoint middle in entry.Key.movablePoint)
                {
                    EEntity middleEntity = GetEntity(state, middle);
                    if (middleEntity == EEntity.None)
                    {
                        tigerMobility++;
                        options++;
                    }
                    else if (middleEntity == EEntity.Goat)
                    {
                        foreach (SpawnPoint landing in middle.movablePoint)
                        {
                            if (GetEntity(state, landing) == EEntity.None && AreCollinear(entry.Key, middle, landing))
                            {
                                tigerCaptures++;
                                options++;
                            }
                        }
                    }
                }

                if (options == 0)
                    trappedTigers++;
            }

            return goatCount * 90 + goatConnections * 7 + trappedTigers * 900 -
                   tigerMobility * 28 - tigerCaptures * 240;
        }

        private static EEntity GetEntity(Dictionary<SpawnPoint, EEntity> state, SpawnPoint point)
        {
            return state.TryGetValue(point, out EEntity entity) ? entity : EEntity.None;
        }
        private static AIMove SelectTopGoatMove(
            List<AIMove> moves,
            IReadOnlyDictionary<SpawnPoint, EntityController> board,
            int topCount)
        {
            List<KeyValuePair<int, AIMove>> ranked = new();
            foreach (AIMove move in moves)
                ranked.Add(new KeyValuePair<int, AIMove>(ScoreGoatMove(move, board), move));
            ranked.Sort((a, b) => b.Key.CompareTo(a.Key));
            int choiceCount = Mathf.Min(topCount, ranked.Count);
            return ranked[UnityEngine.Random.Range(0, choiceCount)].Value;
        }

        private static int ScoreGoatMove(
            AIMove move,
            IReadOnlyDictionary<SpawnPoint, EntityController> board)
        {
            TigerPressure before = EvaluateTigerPressure(AIMove.None, board);
            TigerPressure after = EvaluateTigerPressure(move, board);

            int score = 0;
            score += (before.Mobility - after.Mobility) * 28;
            score += (before.Captures - after.Captures) * 140;
            score += after.TrappedTigers * 450;
            score -= after.Captures * 190;
            score += move.Destination.movablePoint.Count * 4;

            int friendlyNeighbours = 0;
            int tigerNeighbours = 0;
            foreach (SpawnPoint neighbour in move.Destination.movablePoint)
            {
                EEntity occupant = GetEntityAfterGoatMove(neighbour, move, board);
                if (occupant == EEntity.Goat)
                    friendlyNeighbours++;
                else if (occupant == EEntity.Tiger)
                    tigerNeighbours++;
            }
            score += friendlyNeighbours * 16;
            score += tigerNeighbours * 18;

            if (IsVulnerableGoatPosition(move, board))
                score -= 260;

            return score + UnityEngine.Random.Range(0, 3);
        }

        private static TigerPressure EvaluateTigerPressure(
            AIMove goatMove,
            IReadOnlyDictionary<SpawnPoint, EntityController> board)
        {
            int mobility = 0;
            int captures = 0;
            int trappedTigers = 0;

            foreach (KeyValuePair<SpawnPoint, EntityController> entry in board)
            {
                if (entry.Value.eEntity != EEntity.Tiger)
                    continue;

                int tigerOptions = 0;
                foreach (SpawnPoint neighbour in entry.Key.movablePoint)
                {
                    EEntity occupant = GetEntityAfterGoatMove(neighbour, goatMove, board);
                    if (occupant == EEntity.None)
                    {
                        mobility++;
                        tigerOptions++;
                        continue;
                    }

                    if (occupant != EEntity.Goat)
                        continue;

                    foreach (SpawnPoint landing in neighbour.movablePoint)
                    {
                        if (GetEntityAfterGoatMove(landing, goatMove, board) == EEntity.None &&
                            AreCollinear(entry.Key, neighbour, landing))
                        {
                            captures++;
                            tigerOptions++;
                        }
                    }
                }

                if (tigerOptions == 0)
                    trappedTigers++;
            }

            return new TigerPressure(mobility, captures, trappedTigers);
        }

        private static EEntity GetEntityAfterGoatMove(
            SpawnPoint point,
            AIMove move,
            IReadOnlyDictionary<SpawnPoint, EntityController> board)
        {
            if (move.IsValid && point == move.Destination)
                return EEntity.Goat;
            if (move.IsValid && point == move.Origin)
                return EEntity.None;
            return board.TryGetValue(point, out EntityController controller) ? controller.eEntity : EEntity.None;
        }

        private static bool IsVulnerableGoatPosition(
            AIMove move,
            IReadOnlyDictionary<SpawnPoint, EntityController> board)
        {
            foreach (SpawnPoint tigerPoint in move.Destination.movablePoint)
            {
                if (!IsEntity(board, tigerPoint, EEntity.Tiger))
                    continue;

                foreach (SpawnPoint landing in move.Destination.movablePoint)
                {
                    bool landingWillBeEmpty = !board.ContainsKey(landing) || landing == move.Origin;
                    if (landingWillBeEmpty && AreCollinear(tigerPoint, move.Destination, landing))
                        return true;
                }
            }
            return false;
        }

        private static bool IsEntity(
            IReadOnlyDictionary<SpawnPoint, EntityController> board,
            SpawnPoint point,
            EEntity entity)
        {
            return board.TryGetValue(point, out EntityController controller) && controller.eEntity == entity;
        }

        private static bool AreCollinear(SpawnPoint a, SpawnPoint b, SpawnPoint c)
        {
            Vector2 p1 = new Vector2(a.transform.position.x, a.transform.position.z);
            Vector2 p2 = new Vector2(b.transform.position.x, b.transform.position.z);
            Vector2 p3 = new Vector2(c.transform.position.x, c.transform.position.z);
            Vector2 first = p2 - p1;
            Vector2 second = p3 - p1;
            return Mathf.Approximately(first.x * second.y - first.y * second.x, 0f);
        }
    }

    internal readonly struct TigerPressure
    {
        public int Mobility { get; }
        public int Captures { get; }
        public int TrappedTigers { get; }

        public TigerPressure(int mobility, int captures, int trappedTigers)
        {
            Mobility = mobility;
            Captures = captures;
            TrappedTigers = trappedTigers;
        }
    }
}