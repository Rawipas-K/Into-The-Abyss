using UnityEngine;

public class ConfusionSmoke : MonoBehaviour
{
    public float confusionSmokeDuration = 10f;
    public float confusionDuration = 5f;

    void Start()
    {
        // Call the DestroyObject method after 'destroyTime' seconds
        Invoke("DestroyObject", confusionSmokeDuration);
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerController.instance.confusionDuration = confusionDuration;
        }
    }


    private void DestroyObject()
    {
        // Destroy the object this script is attached to
        Destroy(gameObject);
    }
}
