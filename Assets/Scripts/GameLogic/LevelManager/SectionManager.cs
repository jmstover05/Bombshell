using UnityEngine;

public class SectionManager : MonoBehaviour
{
    //collection of objects that are set to inactive when the self destruct sequence starts
    public GameObject destructionObjects;
    //collection of objects that are set to active when the self destruct sequence starts
    public GameObject nonDestructionObjects;

    public SelfDestructManager sdManager;

    // Update is called once per frame
    void Update()
    {
        if(sdManager.isSelfDestructing)
        {
            ActivateDestruction();
        }
    }

    public void ActivateDestruction()
    {
        destructionObjects.SetActive(false);
        nonDestructionObjects.SetActive(true);
    }
}
