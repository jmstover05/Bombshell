using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        anim = GetComponent<Animator>();
        anim.Play("Idle");
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.parent.GetComponent<SelfDestructManager>().isSelfDestructing)
        {
            anim.Play("active");
        }
    }
}
