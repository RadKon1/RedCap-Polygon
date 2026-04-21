using System.Collections.Generic;
using UnityEngine;

namespace LegacyGenerator
{
    public class LevelGeneration : MonoBehaviour
    {
        [System.Serializable]
        public struct RoomPrefabMapping
        {
            public RoomType Type;
            public GameObject[] Prefabs;
        }

        [SerializeField] private RoomPrefabMapping[] roomMappings;

        private int MAX_PATH_LENGTH = 5;
        private LevelNode startNode = new LevelNode(RoomType.Start);
        private LevelNode currentNode;

        // Path creation at the start of the game
        private void Start()
        {
            currentNode = startNode;
            for (int i = 0; i < MAX_PATH_LENGTH; i++)
            {
                LevelNode nextNode = GenerateNextNode(currentNode);
                currentNode.AddNextNode(nextNode);
                if (Random.Range(0, 100) < 30 && currentNode.NodeType != RoomType.Start)
                {
                    nextNode = GenerateNextNode(currentNode);
                    currentNode.AddNextNode(nextNode);
                }
                currentNode = currentNode.NextNodes[0];
            }
            LevelNode bossNode = new LevelNode(RoomType.Boss);
            currentNode.AddNextNode(bossNode);

            PrintGeneratedPath();
            SpawnNodeRecursive(startNode, null);
        }

        // Node generation and creation function. !!!The logic is a game design choice, yet to be made!!!

        private LevelNode GenerateNextNode(LevelNode node)
        {
            switch (node.NodeType)
            {
                case RoomType.Start:
                    return new LevelNode(RoomType.Combat);

                case RoomType.Combat:
                    if (Random.Range(0, 100) < 50)
                    {
                        return new LevelNode(RoomType.Combat);
                    }
                    else
                        return new LevelNode(RoomType.UpgradeRoom);

                case RoomType.UpgradeRoom:
                    return new LevelNode(RoomType.Transition);

                case RoomType.Transition:
                    return new LevelNode(RoomType.Combat);

                default:
                    return new LevelNode(RoomType.Transition);
            }
        }

        // Get prefab that is associated with the room type
        private GameObject GetPrefabForRoomType(RoomType typeToFind)
        {
            for (int i = 0; i < roomMappings.Length; i++)
            {
                if (roomMappings[i].Type == typeToFind)
                {
                    if (roomMappings[i].Prefabs == null || roomMappings[i].Prefabs.Length == 0)
                    {
                        return null;
                    }

                    int prefabNumber = Random.Range(0, roomMappings[i].Prefabs.Length);
                    return roomMappings[i].Prefabs[prefabNumber];
                }
            }
            Debug.LogError("Prefab not found for type " + typeToFind);
            return null;
        }

        // Spawn the created room sequence
        private void SpawnNodeRecursive(LevelNode node, Transform incomingExitPoint)
        {
            GameObject prefabToSpawn = GetPrefabForRoomType(node.NodeType);
            if (prefabToSpawn == null)
            {
                return;
            }
            GameObject spawnedRoom = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);
            RoomController roomData = spawnedRoom.GetComponent<RoomController>();
            if (incomingExitPoint != null)
            {
                Vector3 offset = incomingExitPoint.position - roomData.EntryPoint.position;
                spawnedRoom.transform.position += offset;
            }

            List<Transform> availableExits = new List<Transform>(roomData.ExitPoints);

            for (int i = 0; i < node.NextNodes.Count; i++)
            {
                if (availableExits.Count == 0)
                {
                    break;
                }
                int randomIndex = Random.Range(0, availableExits.Count);
                Transform chosenExit = availableExits[randomIndex];
                availableExits.RemoveAt(randomIndex);
                SpawnNodeRecursive(node.NextNodes[i], chosenExit);
            }
        }

        private void PrintGeneratedPath()
        {
            LevelNode tempNode = startNode;
            string path = "";
            while (tempNode != null)
            {
                path += tempNode.NodeType.ToString();
                if (tempNode.NextNodes.Count > 0)
                {
                    path += " -> ";
                    tempNode = tempNode.NextNodes[0];
                }
                else
                {
                    break;
                }
            }
            Debug.Log(path);
        }
    }
}