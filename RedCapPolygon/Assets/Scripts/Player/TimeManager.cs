using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    [Header("References")]
    public PlayerCombatStats CombatStats;

    public static TimeManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void HitStop()
    {
        StopAllCoroutines(); // Ensure hitstop does not stack
        StartCoroutine(DoHitStop());
    }

    private IEnumerator DoHitStop()
    {
        // freeze time
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(CombatStats.HitStopTime);

        // resume time
        Time.timeScale = 1f;
    }
}
