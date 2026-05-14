using UnityEngine;

[System.Serializable]
public struct RoomSocket
{
    public Transform spawnPoint;

    public GameObject[] possibleEntities;

    [Range(0, 100)]
    public int spawnChance;
}


public class InRoomGenerator : MonoBehaviour
{
    public RoomSocket[] sockets;

    public void PopulateRoom()
    {
        if (sockets == null || sockets.Length == 0)
        {
            return;
        }

        foreach (RoomSocket socket in sockets)
        {
            if (socket.spawnPoint == null || socket.possibleEntities == null || socket.possibleEntities.Length == 0)
            {
                continue;
            }

            if (Random.Range(0, 100) < socket.spawnChance)
            {
                int randomIndex = Random.Range(0, socket.possibleEntities.Length);

                GameObject entityToSpawn = socket.possibleEntities[randomIndex];

                if (entityToSpawn != null)
                {
                    // Spawnujemy wroga
                    GameObject newEnemy = Instantiate(entityToSpawn, socket.spawnPoint.position, socket.spawnPoint.rotation);

                    newEnemy.transform.localScale = Vector3.one;
                }
            }
        }
    }
}