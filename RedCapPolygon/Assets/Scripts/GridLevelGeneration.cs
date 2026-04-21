using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Start,
    Combat,
    UpgradeRoom,
    Transition,
    Boss
}

[System.Serializable]
public struct DoorConnection
{
    public Vector2Int localOffset;
    public Vector2Int direction;
}

public class GridNode
{
    public Vector2Int GridPosition;
    public RoomType Type;
    public Vector2Int OriginPosition;
    public bool IsOrigin;
    public List<DoorConnection> AvailableDoors;

    public GridNode(Vector2Int position, RoomType type, Vector2Int origin, bool isOrigin)
    {
        GridPosition = position;
        Type = type;
        OriginPosition = origin;
        IsOrigin = isOrigin;
        AvailableDoors = new List<DoorConnection>();
    }
}

public class GridLevelGeneration : MonoBehaviour
{
    [System.Serializable]
    public struct RoomPrefabMapping
    {
        public RoomType Type;
        public GameObject[] Prefabs;
    }
    [SerializeField] private RoomPrefabMapping[] roomMappings;
    [System.Serializable]
    public struct BranchRecipe
    {
        [Tooltip("RoomType information for the developer")]
        public string branchName;

        [Tooltip("How long is the branch")]
        public int length;

        [Tooltip("What type of room is guaranteed in this branch")]
        public RoomType specialRoomType;
    }
    [Header("Guaranteed Branches")]
    public List<BranchRecipe> requiredBranches;

    [System.Serializable]
    public struct RecipeStep
    {
        [Tooltip("Room pool, that the algorithm will choose one room from, for this step")]
        public RoomType[] possibleRooms;
    }
    [Header("Level Recipe")]
    public List<RecipeStep> criticalPathRecipe;

    private List<Vector2Int> criticalPathPositions = new List<Vector2Int>();
    private Dictionary<Vector2Int, GridNode> levelMap = new Dictionary<Vector2Int, GridNode>();
    private readonly Dictionary<Vector2Int, int> directionWeights = new Dictionary<Vector2Int, int>
    {
        [Vector2Int.right] = 75,
        [Vector2Int.down] = 10,         //35
        [Vector2Int.up] = 0,
        [Vector2Int.left] = 0          //10
    };
    private readonly Vector2Int[] directions = {
        Vector2Int.up,    // (0, 1)
        Vector2Int.down,  // (0, -1)
        Vector2Int.left,  // (-1, 0)
        Vector2Int.right  // (1, 0)
    };
    public Vector2 roomSize = new Vector2(30f, 20f);
    [Header("Room Dimensions (in grid cells)")]
    private readonly Dictionary<RoomType, Vector2Int> roomDimensions = new Dictionary<RoomType, Vector2Int>
    {
        [RoomType.Start] = new Vector2Int(1, 1),
        [RoomType.Combat] = new Vector2Int(1, 1),
        [RoomType.UpgradeRoom] = new Vector2Int(1, 1),
        [RoomType.Transition] = new Vector2Int(1, 1),
        [RoomType.Boss] = new Vector2Int(2, 2)
    };
    private void Start()
    {
        GenerateCriticalPath(criticalPathRecipe);
        GenerateBranches();
        CreateDoorways();
        InstantiateMap();
    }
    private void GenerateCriticalPath(List<RecipeStep> recipe)
    {
        criticalPathPositions.Clear();
        Vector2Int currentPos = new Vector2Int(-6, 1);
        OccupyRoomArea(currentPos, RoomType.Start);
        criticalPathPositions.Add(currentPos);
        for (int i = 1; i < recipe.Count; i++)
        {
            int totalWeight = 0;
            RoomType[] roomPool = recipe[i].possibleRooms;
            // For now fully random choice from the pool (to be changed)
            RoomType nextRoomType = roomPool[Random.Range(0, roomPool.Length)];
            Vector2Int dimensions = roomDimensions[nextRoomType];
            List<Vector2Int> safeDirections = new List<Vector2Int>();
            for (int j = 0; j < directions.Length; j++)
            {
                Vector2Int potentialPos = currentPos + directions[j];
                if (CanFitRoom(potentialPos, dimensions))
                {
                    safeDirections.Add(directions[j]);
                }
            }
            if (safeDirections.Count == 0)
            {
                break;
            }
            for (int j = 0; j < safeDirections.Count; j++)
            {
                totalWeight += directionWeights[safeDirections[j]];
            }
            int randomWeight = Random.Range(0, totalWeight);
            foreach (Vector2Int direction in safeDirections)
            {
                int weightToSub = directionWeights[direction];
                randomWeight -= weightToSub;
                if (randomWeight <= 0)
                {
                    currentPos += direction;
                    break;
                }
            }
            OccupyRoomArea(currentPos, nextRoomType);
            criticalPathPositions.Add(currentPos);
        }
    }
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

    private void GenerateSingleBranch(Vector2Int startPos, BranchRecipe recipe)
    {
        Vector2Int currentPos = startPos;
        for (int i = 0; i < recipe.length; i++)
        {
            RoomType roomType;
            if (i != recipe.length - 1)
            {
                if (Random.Range(0, 100) < 55)
                {
                    roomType = RoomType.Transition;
                }
                else
                {
                    roomType = RoomType.Combat;
                }
            }
            else
            {
                roomType = recipe.specialRoomType;
            }
            Vector2Int dimensions = roomDimensions[roomType];
            List<Vector2Int> safeDirections = new List<Vector2Int>();
            for (int j = 0; j < directions.Length; j++)
            {
                Vector2Int potentialPos = currentPos + directions[j];
                if (CanFitRoom(potentialPos, dimensions))
                {
                    safeDirections.Add(directions[j]);
                }
            }
            if (safeDirections.Count == 0)
            {
                break;
            }
            currentPos += safeDirections[Random.Range(0, safeDirections.Count)];
            OccupyRoomArea(currentPos, roomType);
        }
    }

    private void GenerateBranches()
    {
        if (criticalPathPositions.Count <= 7)
        {
            Debug.LogWarning("Critical Path too short");
            return;
        }
        List<int> availableIndices = new List<int>();
        for (int i = 3; i < criticalPathPositions.Count - 4; i++)
        {
            availableIndices.Add(i);
        }
        //Shuffling
        for (int i = 0; i < availableIndices.Count; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int temp = availableIndices[i];
            availableIndices[i] = availableIndices[randomIndex];
            availableIndices[randomIndex] = temp;
        }
        foreach (BranchRecipe recipe in requiredBranches)
        {
            if (availableIndices.Count > 0)
            {
                int lastElementIndex = availableIndices.Count - 1;
                int roomIndexOnPath = availableIndices[lastElementIndex];
                availableIndices.RemoveAt(lastElementIndex);
                Vector2Int startPos = criticalPathPositions[roomIndexOnPath];
                GenerateSingleBranch(startPos, recipe);
            }
        }
    }

    private bool CanFitRoom(Vector2Int originPos, Vector2Int dimensions)
    {
        for (int x = originPos.x; x < originPos.x + dimensions.x; x++)
        {
            for (int y = originPos.y; y < originPos.y + dimensions.y; y++)
            {
                if (levelMap.ContainsKey(new Vector2Int(x, y)))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void OccupyRoomArea(Vector2Int originPos, RoomType type)
    {
        Vector2Int dimensions = roomDimensions[type];

        for (int x = originPos.x; x < originPos.x + dimensions.x; x++)
        {
            for (int y = originPos.y; y < originPos.y + dimensions.y; y++)
            {
                bool isOrigin = (x == originPos.x && y == originPos.y);
                Vector2Int cellPos = new Vector2Int(x, y);
                levelMap.Add(cellPos, new GridNode(cellPos, type, originPos, isOrigin));
            }
        }
    }

    private void CreateDoorways()
    {
        foreach (KeyValuePair<Vector2Int, GridNode> node in levelMap)
        {
            Vector2Int currentPos = node.Key;
            GridNode currentNode = node.Value;

            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbourPos = currentPos + direction;

                if (levelMap.ContainsKey(neighbourPos))
                {
                    GridNode neighbourNode = levelMap[neighbourPos];
                    if (currentNode.OriginPosition != neighbourNode.OriginPosition)
                    {
                        Vector2Int offset = currentPos - currentNode.OriginPosition;
                        GridNode originNode = levelMap[currentNode.OriginPosition];
                        originNode.AvailableDoors.Add(new DoorConnection
                        {
                            localOffset = offset,
                            direction = direction
                        });
                    }
                }
            }
        }
    }
    private void InstantiateMap()
    {
        foreach (KeyValuePair<Vector2Int, GridNode> node in levelMap)
        {
            if (!node.Value.IsOrigin) continue;
            float globalX = node.Key.x * roomSize.x;
            float globalY = node.Key.y * roomSize.y;
            GameObject prefabToSpawn = GetPrefabForRoomType(node.Value.Type);
            GameObject newRoom = Instantiate(prefabToSpawn, new Vector3(globalX, globalY, 0), Quaternion.identity);
            RoomController roomController = newRoom.GetComponent<RoomController>();
            if (roomController != null)
            {
                roomController.Initialize(node.Value.AvailableDoors);
            }
            else
            {
                Debug.LogWarning($"Prefab for {node.Value.Type} doesn't have a RoomController script!");
            }
        }
    }
}