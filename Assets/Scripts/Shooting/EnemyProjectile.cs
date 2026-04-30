using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 25;
    public float speed = 18.0f;
    public float lifetime = 4.0f;
    public float hitRadius = 0.5f;

    private Vector3 moveDirection;
    private Transform owner;
    private float lifeTimer = 0.0f;

    public void Launch(Vector3 direction, float projectileSpeed, Transform ownerTransform)
    {
        moveDirection = direction.normalized;
        speed = projectileSpeed;
        owner = ownerTransform;
        lifeTimer = 0.0f;
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        float stepDistance = speed * Time.deltaTime;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + moveDirection * stepDistance;

        if (TryHitPlayer(startPosition, moveDirection, stepDistance))
        {
            return;
        }

        transform.position = endPosition;
    }

    private bool TryHitPlayer(Vector3 startPosition, Vector3 direction, float distance)
    {
        RaycastHit[] hits = Physics.SphereCastAll(
            startPosition,
            hitRadius,
            direction,
            distance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide
        );

        foreach (RaycastHit hit in hits)
        {

            PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                playerHealth = hit.collider.GetComponentInParent<PlayerHealth>();
                if(playerHealth == null && hit.collider.GetComponent<BombshellEnemy>() == null && hit.collider != gameObject.GetComponent<SphereCollider>())
                {
                    Destroy(gameObject);
                    
                }
            }

            if (playerHealth != null)
            {
                playerHealth.TryTakeDamage(damage, "An enemy shot you.");
                Debug.Log("Enemy projectile damaged player for " + damage);
                Destroy(gameObject);
                return true;
            }
            
            
        }

        return false;
    }
}