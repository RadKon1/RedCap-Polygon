using UnityEngine;

public class PlayerTemp : MonoBehaviour
{
    private int _health = 100;

    public void TakeDamage(int damage)
    {
        _health -= damage;
        Debug.Log("Player HP: " + _health);
    }
}
