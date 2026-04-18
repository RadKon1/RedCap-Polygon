using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [System.Serializable]
    public struct DoorMapping
    {
        [Tooltip("Direction (eg. X:0, Y:1 for UP)")]
        public Vector2Int direction;

        [Tooltip("Full wall object (turn-off/onable)")]
        public GameObject solidWall;

        [Tooltip("Transition/door way (turn-off/onable)")]
        public GameObject doorOpening;
    }

    [Header("Door Configuration")]
    public DoorMapping[] doorMappings;

    public void Initialize(List<Vector2Int> availableDoors)
    {
        foreach (DoorMapping doorMapping in doorMappings)
        {
            if (doorMapping.solidWall != null)
            {
                doorMapping.solidWall.SetActive(true);
            }
            if (doorMapping.doorOpening != null)
            {
                doorMapping.doorOpening.SetActive(false);
            }
        }
        foreach (Vector2Int doorDir in availableDoors)
        {
            foreach (DoorMapping mapping in doorMappings)
            {
                if (mapping.direction == doorDir)
                {
                    if (mapping.solidWall != null) mapping.solidWall.SetActive(false);
                    if (mapping.doorOpening != null) mapping.doorOpening.SetActive(true);
                }
            }
        }
    }
}