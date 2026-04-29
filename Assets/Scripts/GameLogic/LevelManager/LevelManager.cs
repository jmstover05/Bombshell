using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] sections;
    public int[] sectionsToReset;

    public void ActivateSection(int idx)
    {
        sections[idx].SetActive(true);
    }

    public void DeactivateSection(int idx)
    {
        sections[idx].SetActive(false);
    }

    public void ResetSections()
    {
        for (int i = 0; i < sectionsToReset.Length; i++)
        {
            DeactivateSection(sectionsToReset[i]);
        }
    }

}
