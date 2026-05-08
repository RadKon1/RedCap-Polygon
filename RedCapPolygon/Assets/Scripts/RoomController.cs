using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [System.Serializable]
    public struct DoorMapping
    {
        [Tooltip("Position of cell in the whole room (mainly for Boss purposes)")]
        public Vector2Int localOffset;

        [Tooltip("Direction (eg. X:0, Y:1 for UP)")]
        public Vector2Int direction;

        [Tooltip("Full wall object (turn-off/onable)")]
        public GameObject solidWall;

        [Tooltip("Transition/door way (turn-off/onable)")]
        public GameObject doorOpening;
    }

    [Header("Door Configuration")]
    public DoorMapping[] doorMappings;

    public void Initialize(List<DoorConnection> availableDoors)
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
        foreach (DoorConnection door in availableDoors)
        {
            foreach (DoorMapping mapping in doorMappings)
            {
                if (mapping.direction == door.direction && mapping.localOffset == door.localOffset)
                {
                    if (mapping.solidWall != null) mapping.solidWall.SetActive(false);
                    if (mapping.doorOpening != null) mapping.doorOpening.SetActive(true);
                }
            }
        }

        InRoomGenerator populator = GetComponent<InRoomGenerator>();
        if (populator != null)
        {
            populator.PopulateRoom();
        }
    }
}