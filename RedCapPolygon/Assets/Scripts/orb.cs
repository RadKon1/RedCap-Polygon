using UnityEngine;

public class Orb : MonoBehaviour
{
  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player") || GetComponent<PlayerManager>() != null)
    {
      PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();
      if (playerCombat != null)
      {
        playerCombat.LevelUp();
        Debug.Log("Orb collected, you have levelled up");
      }
    }
    Destroy(gameObject);
  }
}
