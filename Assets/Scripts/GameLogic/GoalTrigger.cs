using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private bool used = false;

    public bool Used => used;

    public void SetUsed(bool value)
    {
        used = value;
    }

    void OnTriggerEnter(Collider other)
    {
        if (used)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (BombshellGameManager.Instance == null)
        {
            return;
        }

        if (!BombshellGameManager.Instance.sdManager.isSelfDestructing)
        {
            return;
        }

        used = true;
        BombshellGameManager.Instance.CompleteLevel();
    }
}