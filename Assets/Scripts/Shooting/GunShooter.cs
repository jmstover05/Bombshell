using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Allows for basic shooting mechanics<br></br>
/// <strong>IMPORTANT:</strong> Must be a child of an FP Camera in order to work
/// </summary>

public class GunShooter : MonoBehaviour
{
    public FpsHudManager HUD;
    public float range = 100.0f;
    public float maxCooldown = 0.2f;
    public int maxAmmo = 10;
    [Tooltip("The amount of damage the shots do (currently not implemented)")]
    public float damage = 0.0f;

    private float cooldownTimer = 0.0f;
    private bool cooldownOn = false;
    private int ammo = 0;

    private void Start()
    {
        ammo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (!cooldownOn)
        {
            RunCheckCast();

            if(Input.GetMouseButtonDown(0) && !pauseMenu.paused)
            {
                if(ammo > 0)
                {
                    ShootRaycast();
                    ammo -= 1;
                }
                else
                {
                    Debug.Log("Out of ammo!"); 
                }
            }
        }

        HUD.ammoCounter.text = $"AMMO\n{ammo:00}";

        // Manage weapon cooldown
        if(cooldownOn)
        {
            HUD.crosshair.color = Color.red;
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= maxCooldown)
            { 
                cooldownOn = false;
                cooldownTimer = 0.0f;
            }
        }

    }
    /// <summary>
    /// Creates a raycast that will hit enemies
    /// </summary>
    void ShootRaycast()
    {
        //make check raycast
        RaycastHit shootCast;

        // play shooting animation
        HUD.gun.GetComponent<Animator>().SetTrigger("Fired");

        if (Physics.Raycast(transform.position, transform.forward, out shootCast, range))
        {
            //make it so that the following code only runs if the target of the raycast is an enemy


            // vv make enemy take damage here vv
            Debug.Log("HIT!");
        }

        cooldownOn = true;
        
    }
    /// <summary>
    /// Creates a raycast that allows the player to see if they can hit an enemy or not
    /// </summary>
    void RunCheckCast()
    {
        //make check raycast
        RaycastHit checkCast;

        // make it so that the crosshair only turns green when looking at an enemy (use tags)
        if(Physics.Raycast(transform.position, transform.forward, out checkCast, range))
        {
            HUD.crosshair.color = Color.green;
        }
        else
        {
            HUD.crosshair.color = Color.black;
        }
    }
}
