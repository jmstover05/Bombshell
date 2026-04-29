using System.Collections.Generic;
using UnityEngine;

public class BombshellEnemy : MonoBehaviour
{
    public static readonly List<BombshellEnemy> Registry = new List<BombshellEnemy>();

    [Header("Health")]
    public float maxHealth = 3.0f;
    [SerializeField] private float currentHealth;

    [Header("Death")]
    public bool disappearOnDeath = true;

    [Header("Movement")]
    public float moveSpeed = 2.5f;
    public float chaseRange = 15.0f;

    [Header("Melee Attack")]
    public int attackDamage = 25;
    public float attackRange = 2.5f;
    public float attackCooldown = 1.0f;

    [Header("Ranged Attack")]
    public bool usesProjectileAttack = false;
    public EnemyProjectile projectilePrefab;
    public Transform projectileSpawnPoint;
    public float shootRange = 20.0f;
    public float shootCooldown = 1.5f;
    public float projectileSpeed = 18.0f;

    [Header("Safe Enemy Muzzle")]
    public float muzzleForwardOffset = 0.8f;
    public float muzzleUpOffset = 0.6f;
    public float maxAllowedSpawnPointDistance = 4.0f;

    private float nextMeleeAttackTime;
    private float nextShootTime;
    private bool dead = false;

    public float CurrentHealth => currentHealth;
    public bool IsDead => dead;

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
        ResetEnemyHealth();
    }

    void OnEnable()
    {
        if (currentHealth <= 0.0f)
        {
            ResetEnemyHealth();
        }
    }

    private void ResetEnemyHealth()
    {
        currentHealth = maxHealth;
        dead = false;
        nextMeleeAttackTime = 0.0f;
        nextShootTime = 0.0f;
    }

    void Update()
    {
        if (dead)
        {
            return;
        }

        if (BombshellGameManager.Instance == null ||
            BombshellGameManager.Instance.LevelComplete ||
            BombshellGameManager.Instance.WaitingForContinue ||
            !gameObject.activeInHierarchy)
        {
            return;
        }

        Transform player = BombshellGameManager.Instance.PlayerTransform;

        if (player == null)
        {
            return;
        }

        float distance = GetDistanceToPlayer(player);

        FacePlayer(player);

        if (usesProjectileAttack)
        {
            TryShootPlayer(player, distance);
            MoveRangedEnemy(player, distance);
        }
        else
        {
            MoveMeleeEnemy(player, distance);
            TryMeleeAttack(player, distance);
        }
    }

    private void FacePlayer(Transform player)
    {
        Vector3 offset = player.position - transform.position;
        Vector3 flatDirection = new Vector3(offset.x, 0.0f, offset.z);

        if (flatDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        transform.forward = flatDirection.normalized;
    }

    private void MoveMeleeEnemy(Transform player, float distance)
    {
        if (distance > chaseRange)
        {
            return;
        }

        if (distance <= attackRange * 0.8f)
        {
            return;
        }

        MoveToward(player);
    }

    private void MoveRangedEnemy(Transform player, float distance)
    {
        if (distance > chaseRange)
        {
            return;
        }

        if (distance > shootRange * 0.8f)
        {
            MoveToward(player);
        }
    }

    private void MoveToward(Transform player)
    {
        Vector3 offset = player.position - transform.position;
        Vector3 flatDirection = new Vector3(offset.x, 0.0f, offset.z);

        if (flatDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        flatDirection.Normalize();
        transform.position += flatDirection * moveSpeed * Time.deltaTime;
    }

    private void TryMeleeAttack(Transform player, float distance)
    {
        if (distance > attackRange)
        {
            return;
        }

        if (Time.time < nextMeleeAttackTime)
        {
            return;
        }

        nextMeleeAttackTime = Time.time + attackCooldown;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth = player.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.TryTakeDamage(attackDamage, "An enemy got you.");
            Debug.Log(name + " melee hit player for " + attackDamage);
        }
        else
        {
            BombshellGameManager.Instance.LoseToEnemy("An enemy got you.");
        }
    }

    private void TryShootPlayer(Transform player, float distance)
    {
        if (!usesProjectileAttack)
        {
            return;
        }

        if (projectilePrefab == null)
        {
            Debug.LogWarning(name + " cannot shoot because Projectile Prefab is missing.");
            return;
        }

        if (distance > shootRange)
        {
            return;
        }

        if (Time.time < nextShootTime)
        {
            return;
        }

        nextShootTime = Time.time + shootCooldown;
        ShootAtPlayer(player);
    }

    private void ShootAtPlayer(Transform player)
    {
        Vector3 spawnPosition = GetSafeMuzzlePosition();
        Vector3 aimPoint = GetPlayerAimPoint(player);
        Vector3 direction = (aimPoint - spawnPosition).normalized;

        EnemyProjectile projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.LookRotation(direction)
        );

        projectile.transform.position = spawnPosition;
        projectile.transform.rotation = Quaternion.LookRotation(direction);
        projectile.damage = attackDamage;
        projectile.Launch(direction, projectileSpeed, transform);

        Debug.DrawLine(spawnPosition, aimPoint, Color.red, 1.0f);
        Debug.Log(name + " fired projectile from SAFE ENEMY MUZZLE at " + spawnPosition);
    }

    private Vector3 GetSafeMuzzlePosition()
    {
        if (IsValidOwnSpawnPoint())
        {
            return projectileSpawnPoint.position;
        }

        Collider enemyCollider = GetComponentInChildren<Collider>();

        if (enemyCollider != null)
        {
            Bounds bounds = enemyCollider.bounds;

            Vector3 muzzle =
                bounds.center +
                transform.forward * (Mathf.Max(bounds.extents.x, bounds.extents.z) + muzzleForwardOffset) +
                Vector3.up * muzzleUpOffset;

            return muzzle;
        }

        return transform.position + transform.forward * muzzleForwardOffset + Vector3.up * muzzleUpOffset;
    }

    private bool IsValidOwnSpawnPoint()
    {
        if (projectileSpawnPoint == null)
        {
            return false;
        }

        if (!projectileSpawnPoint.IsChildOf(transform))
        {
            Debug.LogWarning(name + " ignored ProjectileSpawnPoint because it is not a child of this enemy.");
            return false;
        }

        float distanceFromEnemy = Vector3.Distance(transform.position, projectileSpawnPoint.position);

        if (distanceFromEnemy > maxAllowedSpawnPointDistance)
        {
            Debug.LogWarning(name + " ignored ProjectileSpawnPoint because it is too far away: " + distanceFromEnemy);
            return false;
        }

        return true;
    }

    private Vector3 GetPlayerAimPoint(Transform player)
    {
        CharacterController playerController = player.GetComponent<CharacterController>();

        if (playerController != null)
        {
            return player.position + playerController.center;
        }

        return player.position + Vector3.up;
    }

    private float GetDistanceToPlayer(Transform player)
    {
        Collider enemyCollider = GetComponentInChildren<Collider>();
        CharacterController playerController = player.GetComponent<CharacterController>();

        if (enemyCollider != null && playerController != null)
        {
            Vector3 playerCenter = player.transform.position + playerController.center;
            Vector3 closestEnemyPoint = enemyCollider.ClosestPoint(playerCenter);
            return Vector3.Distance(closestEnemyPoint, playerCenter);
        }

        return Vector3.Distance(transform.position, player.position);
    }

    public void TakeDamage(float amount)
    {
        if (dead)
        {
            return;
        }

        if (amount <= 0.0f)
        {
            return;
        }

        currentHealth -= amount;

        Debug.Log(name + " took " + amount + " damage. HP: " + currentHealth + " / " + maxHealth);

        if (currentHealth <= 0.0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (dead)
        {
            return;
        }

        dead = true;
        currentHealth = 0.0f;

        if (BombshellGameManager.Instance != null)
        {
            BombshellGameManager.Instance.AddEnemyKillScore();
        }

        Debug.Log(name + " died.");

        if (disappearOnDeath)
        {
            gameObject.SetActive(false);
        }
    }

    public Vector3 GetAimPoint()
    {
        Collider col = GetComponentInChildren<Collider>();

        if (col != null)
        {
            return col.bounds.center;
        }

        Renderer renderer = GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            return renderer.bounds.center;
        }

        return transform.position;
    }

    public Bounds GetAimBounds()
    {
        Collider col = GetComponentInChildren<Collider>();

        if (col != null)
        {
            return col.bounds;
        }

        Renderer renderer = GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            return renderer.bounds;
        }

        return new Bounds(transform.position, Vector3.one);
    }

    public void ApplySnapshot(bool activeState, Vector3 position, Quaternion rotation, float health)
    {
        transform.position = position;
        transform.rotation = rotation;

        currentHealth = Mathf.Clamp(health, 0.0f, maxHealth);
        dead = currentHealth <= 0.0f;
        nextMeleeAttackTime = Time.time + 0.2f;
        nextShootTime = Time.time + 0.2f;

        if (gameObject.activeSelf != activeState)
        {
            gameObject.SetActive(activeState);
        }

        if (activeState && currentHealth > 0.0f)
        {
            dead = false;
        }
    }
}