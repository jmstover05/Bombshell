using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public string deathMessage = "You fell into a hazard.";

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (BombshellGameManager.Instance != null)
        {
            BombshellGameManager.Instance.LoseToHazard(deathMessage);
        }
    }
}