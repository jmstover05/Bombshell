using System.Collections.Generic;
using UnityEngine;

public class BombshellEnemy : MonoBehaviour
{
    public static readonly List<BombshellEnemy> Registry = new List<BombshellEnemy>();

    public float maxHealth = 3.0f;
    public float moveSpeed = 2.5f;
    public float chaseRange = 15.0f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.0f;

    private float currentHealth;
    private float nextAttackTime;

    public float CurrentHealth => currentHealth;

    void Awake()
    {
        if (!Registry.Contains(this))
        {
            Registry.Add(this);
        }
    }

    void OnDestroy()
    {
        Registry.Remove(this);
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (BombshellGameManager.Instance == null ||
            BombshellGameManager.Instance.LevelComplete ||
            BombshellGameManager.Instance.WaitingForContinue)
        {
            return;
        }

        Transform player = BombshellGameManager.Instance.PlayerTransform;
        if (player == null)
        {
            return;
        }

        Vector3 offset = player.position - transform.position;
        float distance = offset.magnitude;

        if (distance <= chaseRange)
        {
            Vector3 flatDirection = new Vector3(offset.x, 0.0f, offset.z);

            if (flatDirection.sqrMagnitude > 0.001f)
            {
                flatDirection.Normalize();
                transform.forward = flatDirection;
                transform.position += flatDirection * moveSpeed * Time.deltaTime;
            }
        }

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            BombshellGameManager.Instance.LoseToEnemy("An enemy got you.");
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0.0f)
        {
            currentHealth = 0.0f;

            if (BombshellGameManager.Instance != null)
            {
                BombshellGameManager.Instance.AddEnemyKillScore();
            }

            gameObject.SetActive(false);
        }
    }

    public void ApplySnapshot(bool activeState, Vector3 position, Quaternion rotation, float health)
    {
        transform.position = position;
        transform.rotation = rotation;
        currentHealth = health;
        nextAttackTime = Time.time + 0.2f;

        if (gameObject.activeSelf != activeState)
        {
            gameObject.SetActive(activeState);
        }
    }
}