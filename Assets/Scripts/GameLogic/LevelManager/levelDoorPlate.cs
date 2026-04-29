using UnityEngine;

public class levelDoorPlate : MonoBehaviour
{
    public int plateID;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            transform.parent.GetComponent<levelDoor>().DoorTriggerEntered(other, plateID);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            transform.parent.GetComponent<levelDoor>().DoorTriggerExited(other, plateID);
        }
    }
}
