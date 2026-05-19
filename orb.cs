using UnityEngine

public class Orb : MonoBehaviour
{
  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
    {
      PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();
      if (playerCombat !- nul)
      {
        playerCombat.LevelUp();
        Debug.Log("Orb collected, you have levelled up");
      }
    }
    Destroy(GameObject);
  }
}
