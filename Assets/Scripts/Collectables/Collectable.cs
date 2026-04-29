using UnityEngine;

public class Collectible : MonoBehaviour
{
    public static int score = 0;
    public int value = 1;  // How much this collectible is worth

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            score += value;  // Add this collectible's value
            //Debug.Log("Score: " + score);
            Destroy(gameObject);
        }
    }
}