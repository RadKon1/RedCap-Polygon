using UnityEngine;

public class PrefabTesting : MonoBehaviour
{
    [Header("Which prefab")]
    [SerializeField] private GameObject prefabToSpawn;

    private void Start()
    {
            SpawnPrefab();
    }

    public void SpawnPrefab()
    {
        Instantiate(prefabToSpawn, new Vector3Int(-6, 1), Quaternion.identity);
    }
}