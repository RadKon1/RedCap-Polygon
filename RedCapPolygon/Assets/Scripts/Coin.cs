using UnityEngine;

public class Coin : MonoBehaviour
{

    [SerializeField] private PlayerStats _playerStats;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_playerStats != null)
            {
                _playerStats.NumberOfCoins++;
                Debug.Log("Coin collected!");
            }
            else
            {
                Debug.LogWarning("PlayerStats reference is missing on Coin.");
            }
            Destroy(gameObject); // Remove the coin from the scene

        }
    }
}
