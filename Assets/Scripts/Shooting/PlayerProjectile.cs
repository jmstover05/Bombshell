using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float speed = 35.0f;
    public float lifetime = 3.0f;
    public float damage = 1.0f;
    public float hitRadius = 0.35f;

    private Vector3 moveDirection;
    private Transform owner;
    private float maxDistance = 100.0f;
    private float distanceTraveled = 0.0f;

    public void Launch(
        Vector3 direction,
        float projectileSpeed,
        float projectileDamage,
        float projectileRange,
        Transform ownerTransform)
    {
        moveDirection = direction.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;
        maxDistance = projectileRange;
        owner = ownerTransform;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        float stepDistance = speed * Time.deltaTime;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + moveDirection * stepDistance;

        RaycastHit hit;

        bool hitSomething = Physics.SphereCast(
            startPosition,
            hitRadius,
            moveDirection,
            out hit,
            stepDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide
        );

        if (hitSomething)
        {
            if (HandleHit(hit.collider))
            {
                return;
            }
        }

        transform.position = endPosition;
        distanceTraveled += stepDistance;

        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private bool HandleHit(Collider other)
    {
        if (owner != null)
        {
            if (other.transform == owner || other.transform.IsChildOf(owner))
            {
                return false;
            }
        }

        BombshellEnemy enemy = other.GetComponent<BombshellEnemy>();

        if (enemy == null)
        {
            enemy = other.GetComponentInParent<BombshellEnemy>();
        }

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("Player projectile damaged enemy: " + enemy.name + " for " + damage);
            Destroy(gameObject);
            return true;
        }

        if (!other.isTrigger)
        {
            Debug.Log("Player projectile hit non-enemy: " + other.name);
            Destroy(gameObject);
            return true;
        }

        return false;
    }
}