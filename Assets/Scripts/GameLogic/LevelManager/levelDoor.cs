using UnityEngine;

public class levelDoor : MonoBehaviour
{
    // section IDs hold the index of the desired section stored in the LevelManager "sections" array
    public int primarySectionID;
    public int secondarySectionID;
    // trigger that opens the door
    public BoxCollider doorPlate;

    public LevelManager levelManager;
    public Animator animator;

    // the id of the last plate that the player exited from
    private int lastPlateExited = 0;

    // keeps track of whether or not the player is inside either plate
    private bool isInsidePlate1 = false;
    private bool isInsidePlate2 = false;
    public void DoorTriggerEntered(Collider other, int plateID)
    {
        if(other.tag == "Player")
        {
            if(plateID == 1) // load secondary section
            {
                levelManager.ActivateSection(secondarySectionID);
                isInsidePlate1 = true;
                if(!isInsidePlate2)
                {
                    animator.Play("Open");
                }

            }
            else if(plateID == 2)// load primary section
            {
                levelManager.ActivateSection(primarySectionID);
                isInsidePlate2 = true;
                if (!isInsidePlate1)
                {
                    animator.Play("Open");
                }
            }
            
        }
    }

    public void DoorTriggerExited(Collider other, int plateID)
    {
        if (other.tag == "Player") 
        {
            if(!(isInsidePlate1 && isInsidePlate2)) //does not call close if player is inside both plates at the same time
            {
                animator.Play("Close");
            }

            lastPlateExited = plateID;
            if (lastPlateExited == 1)
            {
                isInsidePlate1 = false;
            }
            else if (lastPlateExited == 2)
            {
                isInsidePlate2 = false;
            }
        }
    }

    public void DisableSections()
    {
        if (lastPlateExited == 1)
        {
            levelManager.DeactivateSection(secondarySectionID);
            isInsidePlate1 = false;
        }
        else if (lastPlateExited == 2)
        {
            levelManager.DeactivateSection(primarySectionID);
            isInsidePlate2 = false;
        }
    }
}
