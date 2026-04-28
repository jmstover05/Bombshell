using UnityEngine;
/// <summary>
/// Allows the 1st person camera to function
/// </summary>
public class cameraLook : MonoBehaviour
{
    // horizontal camera sensitivity
    public float horizSensitivity = 20.0f;
    // vertical camera sensitivity
    public float vertSensitivity = 10.0f;
    
    // max camera angle constraint
    public float upperAngleLimit = -70.0f;
    //  min camera angle constraint
    public float lowerAngleLimit = 60.0f;

    // ! Use horizAngle as the direction the player is facing for the player movement script. !
    // Horizontal angle of the camera (y-axis)
    private float horizAngle = 0;
    //Vertical angle of the camera (x-axis)
    private float vertAngle = 0;
    private void Start()
    {
        vertSensitivity = SettingsManager.vertSensitivity;
        horizSensitivity = SettingsManager.horizSensitivity;
    }

    void LateUpdate()
    {
        if(!pauseMenu.paused)
        {
            // Get the distance mouse has moved each tick 
            Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            // Mouse movement on the x axis ties to cam rotation on the y axis. 
            // Mouse movement on the y axis ties to cam rotation on the x axis. 
            horizAngle += mouseMovement.x * horizSensitivity;
            vertAngle += mouseMovement.y * -vertSensitivity;

            //vertical camera constraints
            vertAngle = Mathf.Clamp(vertAngle, upperAngleLimit, lowerAngleLimit);

            // apply rotations
            Quaternion rotation = Quaternion.Euler(new Vector3(vertAngle, horizAngle, 0.0f));
            transform.rotation = rotation;
        }
    }
}
