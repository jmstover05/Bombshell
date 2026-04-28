using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public bool destroyOnFinish = false;
    private Animator explosion;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        explosion = GetComponent<Animator>();
        explosion.Play("Explode");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }

    public void OnFinish()
    {
        if(destroyOnFinish)
        {
            Destroy(gameObject);
        }
    }
}
