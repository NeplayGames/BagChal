using System;
using UnityEngine;

namespace NeplayGame.BagChal
{
    public class InputManager
    {
        public event Action<SpawnPoint> TouchEntity;
        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.TryGetComponent(out SpawnPoint spawnPoint))
                    {
                    Debug.Log(hit.collider);
                        TouchEntity?.Invoke(spawnPoint);
                    }
                }
            }
        }
    }
}