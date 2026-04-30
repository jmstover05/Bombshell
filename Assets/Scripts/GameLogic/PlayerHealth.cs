using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public FpsHudManager fpsHudManager;
    public int maxHealth = 100;
    public float damageCooldown = 0.35f;

    [SerializeField] private int currentHealth;
    private float nextDamageTime = 0.0f;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
    }

    public void SetHealth(int value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
    }

    public bool TryTakeDamage(int amount, string deathMessage)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (Time.time < nextDamageTime)
        {
            return false;
        }

        nextDamageTime = Time.time + damageCooldown;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        //play hurt screen animation
        fpsHudManager.hurtScreen.GetComponent<Animator>().Play("hurt");

        if (currentHealth == 0 && BombshellGameManager.Instance != null)
        {
            BombshellGameManager.Instance.LoseToEnemy(deathMessage);
        }

        return true;
    }
}