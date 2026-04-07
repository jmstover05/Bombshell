using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Stores all the important elements of the FPS HUD so that other scripts can easily access them
/// </summary>
public class FpsHudManager : MonoBehaviour
{
    [Tooltip("The crosshair HUD element")]
    public Image crosshair;
    [Tooltip("The gun sprite HUD element")]
    public Image gun;
    [Tooltip("The ammo counter HUD element")]
    public TextMeshProUGUI ammoCounter;

}
