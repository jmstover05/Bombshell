using UnityEngine;

/// <summary>
/// Handles shooting, ammo, player projectiles, enemy damage, and angled gun-jump.
/// Projectile fires straight through the crosshair. Gun-jump does not spawn a projectile.
/// Put this on Gun Manager under the FPS Camera.
/// </summary>
public class GunShooter : MonoBehaviour
{
    public FpsHudManager HUD;

    [Header("Aim Source")]
    public Camera aimCamera;

    [Header("Projectile Shooting")]
    public GameObject playerProjectilePrefab;
    public float projectileSpeed = 35.0f;
    public float projectileSpawnDistance = 0.6f;

    [Header("Shooting")]
    public float range = 100.0f;
    public float maxCooldown = 0.2f;
    public int maxAmmo = 100;
    public float damage = 1.0f;

    [Header("Crosshair Enemy Check")]
    public float crosshairPaddingPixels = 10.0f;

    [Header("Gun Jump")]
    public bool enableGunJump = true;
    public float gunJumpImpulsePower = 9.0f;
    public float gunJumpMinUpVelocity = 7.0f;
    public string gunJumpSurfaceTag = "GunJumpSurface";
    public float lookDownThreshold = -0.35f;

    [Header("Gun Jump Face Check")]
    public Vector3 allowedFaceNormal = Vector3.up;
    public float faceMatchRequired = 0.75f;

    private float cooldownTimer = 0.0f;
    private bool cooldownOn = false;
    private int ammo = 0;

    private PlayerMovement playerMovement;

    void Start()
    {
        ammo = maxAmmo;

        playerMovement = GetComponentInParent<PlayerMovement>();

        if (playerMovement == null)
        {
            playerMovement = FindAnyObjectByType<PlayerMovement>();
        }

        if (aimCamera == null)
        {
            aimCamera = GetComponentInParent<Camera>();
        }

        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }

        UpdateAmmoUI();
    }

    void Update()
    {
        if (!cooldownOn)
        {
            RunCheckCast();

            if (Input.GetMouseButtonDown(0))
            {
                if (ammo > 0)
                {
                    FireGun();
                }
                else
                {
                    Debug.Log("Out of ammo!");
                }
            }
        }

        if (cooldownOn)
        {
            if (HUD != null && HUD.crosshair != null)
            {
                HUD.crosshair.color = Color.red;
            }

            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= maxCooldown)
            {
                cooldownOn = false;
                cooldownTimer = 0.0f;
            }
        }
    }

    private void FireGun()
    {
        PlayGunAnimation();

        Vector2 crosshairScreenPoint = GetCrosshairScreenPoint();
        Ray aimRay = GetAimRay(crosshairScreenPoint);

        bool didGunJump = TryGunJump(aimRay);

        if (!didGunJump)
        {
            SpawnPlayerProjectile(aimRay);
        }

        ammo -= 1;
        UpdateAmmoUI();

        cooldownOn = true;
        cooldownTimer = 0.0f;
    }

    private Vector2 GetCrosshairScreenPoint()
    {
        if (HUD != null && HUD.crosshair != null)
        {
            RectTransform crosshairRect = HUD.crosshair.rectTransform;
            return RectTransformUtility.WorldToScreenPoint(null, crosshairRect.position);
        }

        if (aimCamera != null)
        {
            return new Vector2(aimCamera.pixelWidth * 0.5f, aimCamera.pixelHeight * 0.5f);
        }

        return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    private Ray GetAimRay(Vector2 screenPoint)
    {
        if (aimCamera != null)
        {
            return aimCamera.ScreenPointToRay(screenPoint);
        }

        return new Ray(transform.position, transform.forward);
    }

    private void SpawnPlayerProjectile(Ray aimRay)
    {
        Vector3 spawnPosition = aimRay.origin + aimRay.direction * projectileSpawnDistance;
        Vector3 fireDirection = aimRay.direction;

        GameObject projectileObject;

        if (playerProjectilePrefab != null)
        {
            projectileObject = Instantiate(
                playerProjectilePrefab,
                spawnPosition,
                Quaternion.LookRotation(fireDirection)
            );
        }
        else
        {
            projectileObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectileObject.name = "RuntimePlayerProjectile";
            projectileObject.transform.position = spawnPosition;
            projectileObject.transform.rotation = Quaternion.LookRotation(fireDirection);
            projectileObject.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

            Collider col = projectileObject.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        PlayerProjectile projectile = projectileObject.GetComponent<PlayerProjectile>();

        if (projectile == null)
        {
            projectile = projectileObject.AddComponent<PlayerProjectile>();
        }

        Transform owner = playerMovement != null ? playerMovement.transform : transform;

        projectile.Launch(
            fireDirection,
            projectileSpeed,
            damage,
            range,
            owner
        );

        Debug.Log("Projectile fired straight through crosshair.");
    }

    private bool TryGunJump(Ray aimRay)
    {
        if (!enableGunJump)
        {
            return false;
        }

        if (playerMovement == null)
        {
            return false;
        }

        bool lookingDown = aimRay.direction.y <= lookDownThreshold;

        if (!lookingDown)
        {
            return false;
        }

        RaycastHit hit;

        if (!Physics.Raycast(
            aimRay,
            out hit,
            range,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        if (!hit.collider.CompareTag(gunJumpSurfaceTag))
        {
            Debug.Log("Gun jump blocked. Hit object is not tagged " + gunJumpSurfaceTag);
            return false;
        }

        Vector3 requiredNormal = allowedFaceNormal.normalized;
        float faceMatch = Vector3.Dot(hit.normal.normalized, requiredNormal);

        if (faceMatch < faceMatchRequired)
        {
            Debug.Log("Gun jump blocked. Wrong face. Hit normal: " + hit.normal);
            return false;
        }

        Vector3 impulseDirection = -aimRay.direction;

        playerMovement.ApplyGunJump(
            impulseDirection,
            gunJumpImpulsePower,
            gunJumpMinUpVelocity
        );

        Debug.Log("Angled gun jump triggered from surface: " + hit.collider.name);
        return true;
    }

    private void PlayGunAnimation()
    {
        if (HUD != null && HUD.gun != null)
        {
            Animator gunAnimator = HUD.gun.GetComponent<Animator>();

            if (gunAnimator != null)
            {
                gunAnimator.SetTrigger("Fired");
            }
        }
    }

    private void RunCheckCast()
    {
        if (HUD == null || HUD.crosshair == null)
        {
            return;
        }

        Vector2 crosshairScreenPoint = GetCrosshairScreenPoint();
        BombshellEnemy enemy = GetEnemyUnderCrosshair(crosshairScreenPoint);

        HUD.crosshair.color = enemy != null ? Color.green : Color.black;
    }

    private BombshellEnemy GetEnemyUnderCrosshair(Vector2 crosshairScreenPoint)
    {
        if (aimCamera == null)
        {
            return null;
        }

        BombshellEnemy bestEnemy = null;
        float bestDepth = float.MaxValue;

        BombshellEnemy[] enemies = FindObjectsByType<BombshellEnemy>();

        foreach (BombshellEnemy enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            if (!enemy.gameObject.activeInHierarchy || enemy.IsDead)
            {
                continue;
            }

            Bounds bounds = enemy.GetAimBounds();

            if (IsCrosshairInsideBounds(crosshairScreenPoint, bounds, out float depth))
            {
                if (depth < bestDepth)
                {
                    bestDepth = depth;
                    bestEnemy = enemy;
                }
            }
        }

        return bestEnemy;
    }

    private bool IsCrosshairInsideBounds(Vector2 crosshairScreenPoint, Bounds bounds, out float depth)
    {
        depth = float.MaxValue;

        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        Vector3[] corners = new Vector3[8];

        corners[0] = center + new Vector3(-extents.x, -extents.y, -extents.z);
        corners[1] = center + new Vector3(-extents.x, -extents.y, extents.z);
        corners[2] = center + new Vector3(-extents.x, extents.y, -extents.z);
        corners[3] = center + new Vector3(-extents.x, extents.y, extents.z);
        corners[4] = center + new Vector3(extents.x, -extents.y, -extents.z);
        corners[5] = center + new Vector3(extents.x, -extents.y, extents.z);
        corners[6] = center + new Vector3(extents.x, extents.y, -extents.z);
        corners[7] = center + new Vector3(extents.x, extents.y, extents.z);

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        float closestDepth = float.MaxValue;

        bool anyVisible = false;

        foreach (Vector3 corner in corners)
        {
            Vector3 screenPoint = aimCamera.WorldToScreenPoint(corner);

            if (screenPoint.z <= 0.0f)
            {
                continue;
            }

            anyVisible = true;

            minX = Mathf.Min(minX, screenPoint.x);
            maxX = Mathf.Max(maxX, screenPoint.x);
            minY = Mathf.Min(minY, screenPoint.y);
            maxY = Mathf.Max(maxY, screenPoint.y);
            closestDepth = Mathf.Min(closestDepth, screenPoint.z);
        }

        if (!anyVisible)
        {
            return false;
        }

        minX -= crosshairPaddingPixels;
        maxX += crosshairPaddingPixels;
        minY -= crosshairPaddingPixels;
        maxY += crosshairPaddingPixels;

        bool inside =
            crosshairScreenPoint.x >= minX &&
            crosshairScreenPoint.x <= maxX &&
            crosshairScreenPoint.y >= minY &&
            crosshairScreenPoint.y <= maxY;

        if (inside)
        {
            depth = closestDepth;
        }

        return inside;
    }

    private void UpdateAmmoUI()
    {
        if (HUD != null && HUD.ammoCounter != null)
        {
            HUD.ammoCounter.text = $"AMMO\n{ammo:00}";
        }
    }

    public void RefillAmmo()
    {
        ammo = maxAmmo;
        UpdateAmmoUI();
    }
}