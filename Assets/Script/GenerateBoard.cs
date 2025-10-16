using System;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
namespace NeplayGame.BagChal
{
    public class GenerateBoard
    {
        private GameObject[] spawnObjs = new GameObject[25];
        public GenerateBoard(GameObject spawnPointGO, float distance, Material lineMaterial)
        {
            CreateBaghChalBoard(spawnPointGO, distance, lineMaterial);
        }
        public Dictionary<int, List<int>> adjacency = new Dictionary<int, List<int>>
        {
            { 0, new List<int> { 1, 5, 6 } },
            { 1, new List<int> { 0, 2, 6 } },
            { 2, new List<int> { 1, 3, 7 , 6, 8} },
            { 3, new List<int> { 2, 4, 8 } },
            { 4, new List<int> { 3, 9, 8 } },

            { 5, new List<int> { 0, 6, 10} },
            { 6, new List<int> { 0, 1,2, 5, 7,10, 11, 12 } },
            { 7, new List<int> { 2, 6, 8, 12 } },
            { 8, new List<int> { 2, 3, 4, 7, 9,12, 13 ,12} },
            { 9, new List<int> { 4, 8, 14 } },

            { 10, new List<int> { 5,6, 11, 15,16 } },
            { 11, new List<int> {  6, 10, 12, 16 } },
            { 12, new List<int> { 6, 7,8, 11, 13, 16, 17,18 } },
            { 13, new List<int> { 8, 12, 14, 18 } },
            { 14, new List<int> { 8,9, 13,18, 19 } },

            { 15, new List<int> { 10, 16, 20} },
            { 16, new List<int> { 10, 11, 12, 15, 17,20, 21, 22 } },
            { 17, new List<int> { 12,  16, 18, 22} },
            { 18, new List<int> { 12, 13, 14, 17, 19,22, 23, 24 } },
            { 19, new List<int> { 14, 18, 24 } },

            { 20, new List<int> { 15,16, 21 } },
            { 21, new List<int> { 16, 20, 22 } },
            { 22, new List<int> { 16, 17, 18,21, 23 } },
            { 23, new List<int> { 18, 22, 24 } },
            { 24, new List<int> { 18, 19, 23 } },
        };

        private void CreateBaghChalBoard(GameObject spawnPointGO, float distance, Material lineMaterial)
        {
            int i = 0;
            int j = 0;
            int arrayIndex = 0;
            foreach (var point in adjacency)
            {
                spawnObjs[arrayIndex++] = GameObject.Instantiate(spawnPointGO, new Vector3(i++ * distance, 0, j * distance), spawnPointGO.transform.rotation);
                if (i == 5)
                {
                    i = 0;
                    j++;
                }
            }
            for (int k = 0; k < arrayIndex; k++)
            {
                SpawnPoint spawnPoint = spawnObjs[k].GetComponent<SpawnPoint>();
                List<int> adj = adjacency[k];
                List<SpawnPoint> spawnPoints = new();
                foreach (var index in adj)
                {
                    spawnPoints.Add(spawnObjs[index].GetComponent<SpawnPoint>());
                    CreateLine(spawnPoint.transform.position, spawnObjs[index].transform.position, lineMaterial);
                }
                spawnPoint.movablePoint = spawnPoints;
            }
        }
        private void CreateLine(Vector3 start, Vector3 end, Material lineMaterial)
        {
            GameObject lineObj = new GameObject("DynamicLine");
            var lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.startWidth = 1f;
            lr.endWidth = 1f;
            lr.positionCount = 2;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            lr.useWorldSpace = true;
        }
    }
}