using System;
using System.Collections.Generic;
using NeplayGame.BagChal.AI;
using NeplayGame.BagChal.Entity;
using UnityEngine;
namespace NeplayGame.BagChal
{
    public class EntityManager : IDisposable
    {
        private GameObject goat;
        private int totalGoat = 0;
        private float speed = 10;
        private EEntity CurrentEntity
        {
            get
            {
                return currentEntity;
            }
            set
            {
                currentEntity = value;
                OnChangeTurn?.Invoke(currentEntity, 20 - totalGoat);
            }
        }
        private EEntity currentEntity;
        private SpawnPoint obtainEntitySpawnPoint;
        Dictionary<SpawnPoint, EntityController> entitySpawnPoints = new();
        public event Action GoatKill;
        public event Action<EEntity, int> OnChangeTurn;
        private InputManager inputManager;
        private readonly EEntity aiEntity;
        private readonly IBaghChalAI ai;
        private readonly IReadOnlyList<SpawnPoint> boardPoints;

        public EntityManager(GenerateBoard generateBoard, GameObject tiger, GameObject goat, InputManager inputManager, float speed, EEntity aiEntity = EEntity.None, AIDifficulty aiDifficulty = AIDifficulty.Medium)
        {
            this.goat = goat;
            foreach (var tigerSpawnPoint in generateBoard.TigerSpawnPoint)
            {
                EntityController entityController = GameObject.Instantiate(tiger, tigerSpawnPoint.transform.position + Vector3.up, tiger.transform.rotation).GetComponent<EntityController>();
                entitySpawnPoints.Add(tigerSpawnPoint, entityController);
                entityController.MovementCompleted += CanMoveNext;
            }
            this.inputManager = inputManager;
            this.aiEntity = aiEntity;
            ai = aiEntity switch
            {
                EEntity.Goat => new GoatAI(aiDifficulty),
                EEntity.Tiger => new TigerAI(aiDifficulty),
                _ => null
            };
            boardPoints = generateBoard.SpawnPoints;
            CurrentEntity = EEntity.Goat;
            ConfigureInputForTurn();
            this.speed = speed;
        }

        private void CanMoveNext()
        {
            CurrentEntity = CurrentEntity == EEntity.Goat ? EEntity.Tiger : EEntity.Goat;
            ConfigureInputForTurn();
        }

        private bool IsAITurn => aiEntity != EEntity.None && CurrentEntity == aiEntity;

        private void ConfigureInputForTurn()
        {
            inputManager.TouchEntity -= CurrentTouchEntity;
            if (!IsAITurn)
                inputManager.TouchEntity += CurrentTouchEntity;
        }

        private void CurrentTouchEntity(SpawnPoint spawnPoint)
        {
            if (IsAITurn)
                return;


            if (totalGoat < 20)
            {
                if (CurrentEntity == EEntity.Goat)
                {
                    if (!entitySpawnPoints.ContainsKey(spawnPoint))
                    {
                        InstantiateGoat(spawnPoint);
                    }
                    return;
                }
            }
            TryMoveEntity(spawnPoint);
        }

        private void TryMoveEntity(SpawnPoint spawnPoint)
        {
            if (entitySpawnPoints.ContainsKey(spawnPoint))
            {
                EntityController entityController = entitySpawnPoints[spawnPoint];
                if (entityController.eEntity == CurrentEntity)
                {
                    entityController.StartGrowShrink();
                    if (obtainEntitySpawnPoint != null)
                        entitySpawnPoints[obtainEntitySpawnPoint].StopGrowShrink();
                    obtainEntitySpawnPoint = spawnPoint;
                    Debug.Log(obtainEntitySpawnPoint);
                }
                else
                {
                    obtainEntitySpawnPoint = null;
                }
                return;
            }
            if (!obtainEntitySpawnPoint)
                return;

            if (CanMove(obtainEntitySpawnPoint, spawnPoint))
            {
                entitySpawnPoints[obtainEntitySpawnPoint].MoveTo(spawnPoint.transform.position, speed);
                entitySpawnPoints.Add(spawnPoint, entitySpawnPoints[obtainEntitySpawnPoint]);
                entitySpawnPoints.Remove(obtainEntitySpawnPoint);
                obtainEntitySpawnPoint = null;
                inputManager.TouchEntity -= CurrentTouchEntity;
            }
        }

        public bool CanMove(SpawnPoint obtainEntitySpawnPoint, SpawnPoint spawnPoint)
        {
            if (obtainEntitySpawnPoint.movablePoint.Contains(spawnPoint))
            {
                return true;
            }
            if (CurrentEntity == EEntity.Tiger)
            {
                foreach (var spawnP in obtainEntitySpawnPoint.movablePoint)
                {
                    if (entitySpawnPoints.ContainsKey(spawnP) && entitySpawnPoints[spawnP].eEntity == EEntity.Goat)
                    {
                        if (spawnP.movablePoint.Contains(spawnPoint))
                        {
                            if (AreCollinear(spawnPoint.transform, spawnP.transform, obtainEntitySpawnPoint.transform))
                            {
                                TigerEntity tigerEntity = (TigerEntity)entitySpawnPoints[obtainEntitySpawnPoint];
                                tigerEntity.SetGoat((GoatEntity)entitySpawnPoints[spawnP]);
                                KillGoat(spawnP);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void KillGoat(SpawnPoint spawnP)
        {
            EntityController entityController = entitySpawnPoints[spawnP];
            entitySpawnPoints.Remove(spawnP);
            entityController.MovementCompleted -= CanMoveNext;
            //GameObject.Destroy(entityController.gameObject);
            GoatKill?.Invoke();
        }

        public bool AreCollinear(Transform a, Transform b, Transform c)
        {
            Vector2 p1 = new Vector2(a.position.x, a.position.z);
            Vector2 p2 = new Vector2(b.position.x, b.position.z);
            Vector2 p3 = new Vector2(c.position.x, c.position.z);

            // Vector from p1 to p2
            Vector2 v1 = p2 - p1;
            // Vector from p1 to p3
            Vector2 v2 = p3 - p1;

            // Cross product in 2D -> scalar (z-component of 3D cross product)
            float cross = v1.x * v2.y - v1.y * v2.x;
            // If cross = 0 (or very close), they're collinear
            return Mathf.Approximately(cross, 0f);
        }

        public bool IsAlignedOnAxis(Transform objA, Transform objB, Transform objC)
        {
            // ✅ Same X plane (all share the same X value)
            bool sameX = Mathf.Approximately(objA.position.x, objB.position.x) &&
                         Mathf.Approximately(objB.position.x, objC.position.x);

            // ✅ Same Z plane (all share the same Z value)
            bool sameZ = Mathf.Approximately(objA.position.z, objB.position.z) &&
                         Mathf.Approximately(objB.position.z, objC.position.z);

            return sameX || sameZ;
        }

        private void InstantiateGoat(SpawnPoint spawnPoint)
        {
            totalGoat++;
            EntityController entityController = GameObject.Instantiate(goat, spawnPoint.transform.position + Vector3.up, goat.transform.rotation).GetComponent<EntityController>();
            entitySpawnPoints.Add(spawnPoint, entityController);
            CurrentEntity = EEntity.Tiger;
            entityController.MovementCompleted += CanMoveNext;
            ConfigureInputForTurn();
        }

        public bool PerformAITurn()
        {
            if (!IsAITurn || ai == null)
                return false;

            AIMove selectedMove = ai.ChooseMove(
                Mathf.Max(0, 20 - totalGoat),
                boardPoints,
                entitySpawnPoints);

            if (!selectedMove.IsValid)
                return false;

            if (selectedMove.IsPlacement)
                InstantiateGoat(selectedMove.Destination);
            else
                MoveEntity(selectedMove.Origin, selectedMove.Destination, selectedMove.CapturedGoat);

            return true;
        }

        private void MoveEntity(SpawnPoint origin, SpawnPoint destination, SpawnPoint capturedGoat)
        {
            EntityController controller = entitySpawnPoints[origin];
            if (capturedGoat != null)
            {
                ((TigerEntity)controller).SetGoat((GoatEntity)entitySpawnPoints[capturedGoat]);
                KillGoat(capturedGoat);
            }

            controller.MoveTo(destination.transform.position, speed);
            entitySpawnPoints.Add(destination, controller);
            entitySpawnPoints.Remove(origin);
        }


        public void Dispose()
        {
            inputManager.TouchEntity -= CurrentTouchEntity;
        }

        public bool CheckTigerLock()
        {
            foreach (var entityController in entitySpawnPoints)
            {
                if (entityController.Value.eEntity == EEntity.Tiger)
                {
                    foreach (var spawnP in entityController.Key.movablePoint)
                    {
                        if (!entitySpawnPoints.ContainsKey(spawnP))
                        {
                            return false;
                        }
                        if (entitySpawnPoints[spawnP].eEntity == EEntity.Goat)
                        {
                            foreach (var neighbourSpawnP in spawnP.movablePoint)
                            {
                                if (!entitySpawnPoints.ContainsKey(neighbourSpawnP) && AreCollinear(entityController.Key.transform, spawnP.transform, neighbourSpawnP.transform))
                                {
                                    return false;
                                }
                            }
                        }
                    }

                }
            }
            Debug.Log(true);
            return true;
        }
    }
}