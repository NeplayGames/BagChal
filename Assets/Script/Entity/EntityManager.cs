using System;
using System.Collections.Generic;
using NeplayGame.BagChal.Entity;
using NeplayGame.BagChal.UI;
using Unity.VisualScripting;
using UnityEngine;
namespace NeplayGame.BagChal
{
    public class EntityManager : IDisposable
    {
        private GameObject goat;
        private int totalGoat = 0;
        private EEntity CurrentEntity
        {
            get
            {
                return currentEntity;
            }
            set
            {
                currentEntity = value;
                OnChangeTurn?.Invoke(currentEntity);
            }
        }
        private EEntity currentEntity;
        private SpawnPoint obtainEntitySpawnPoint;
        Dictionary<SpawnPoint, EntityController> entitySpawnPoints = new();
        public event Action GoatKill;
        public event Action<EEntity> OnChangeTurn;
        private InputManager inputManager;
        public EntityManager(GenerateBoard generateBoard, GameObject tiger, GameObject goat, InputManager inputManager)
        {
            this.goat = goat;
            foreach (var tigerSpawnPoint in generateBoard.TigerSpawnPoint)
            {
                EntityController entityController = GameObject.Instantiate(tiger, tigerSpawnPoint.transform.position + Vector3.up, tiger.transform.rotation).GetComponent<EntityController>();
                entitySpawnPoints.Add(tigerSpawnPoint, entityController);
                entityController.MovementCompleted += CanMoveNext;
            }
            this.inputManager = inputManager;
            inputManager.TouchEntity += CurrentTouchEntity;
            CurrentEntity = EEntity.Goat;
        }

        private void CanMoveNext()
        {
            inputManager.TouchEntity += CurrentTouchEntity;
        }

        private void CurrentTouchEntity(SpawnPoint spawnPoint)
        {

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
                obtainEntitySpawnPoint = entityController.eEntity == CurrentEntity ? spawnPoint : null;
                return;
            }
            if (!obtainEntitySpawnPoint)
                return;

            if (CanMove(obtainEntitySpawnPoint, spawnPoint))
            {
                entitySpawnPoints[obtainEntitySpawnPoint].MoveTo(spawnPoint.transform.position);
                entitySpawnPoints.Add(spawnPoint, entitySpawnPoints[obtainEntitySpawnPoint]);
                entitySpawnPoints.Remove(obtainEntitySpawnPoint);
                obtainEntitySpawnPoint = null;
                CurrentEntity = CurrentEntity == EEntity.Goat ? EEntity.Tiger : EEntity.Goat;
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
            Debug.Log(cross);
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
            EntityController entityController = GameObject.Instantiate(goat, spawnPoint.transform.position + Vector3.up, goat.transform.rotation).GetComponent<EntityController>();
            entitySpawnPoints.Add(spawnPoint, entityController);
            CurrentEntity = EEntity.Tiger;
            totalGoat++;
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
                                if (!entitySpawnPoints.ContainsKey(spawnP) && AreCollinear(entityController.Key.transform, spawnP.transform, neighbourSpawnP.transform))
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