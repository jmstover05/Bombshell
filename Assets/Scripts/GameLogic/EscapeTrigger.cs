using UnityEngine;

public class EscapeTrigger : MonoBehaviour
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

        used = true;
        BombshellGameManager.Instance.StartSelfDestruct();
    }
}