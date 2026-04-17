using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public Transform respawnPoint;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (BombshellGameManager.Instance == null)
        {
            return;
        }

        if (BombshellGameManager.Instance.CheckpointsLocked)
        {
            return;
        }

        Vector3 checkpointPos = respawnPoint != null ? respawnPoint.position : transform.position;
        BombshellGameManager.Instance.SetCheckpoint(checkpointPos);
    }
}