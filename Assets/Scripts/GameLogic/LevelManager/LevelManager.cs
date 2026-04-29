using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] sections;

    public void ActivateSection(int idx)
    {
        sections[idx].SetActive(true);
    }

    public void DeactivateSection(int idx)
    {
        sections[idx].SetActive(false);
    }

}
