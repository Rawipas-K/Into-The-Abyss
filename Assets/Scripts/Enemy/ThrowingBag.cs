using UnityEngine;

public class ThrowObject : MonoBehaviour
{   
    public GameObject ObjectToSpawn;
    public float speed = 10f;
    public float distanceThreshold = 1f;

    private Animator anim;
    private Transform player;
    private Vector2 initialPlayerPosition;
    private Vector2 directionToInitialPosition;
    
    void Start()
    {   
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        initialPlayerPosition = player.position; // Store the initial position of the player
        directionToInitialPosition = (initialPlayerPosition - (Vector2)transform.position).normalized; // Calculate direction towards the initial player position
    }

    void Update()
    {
        // Move the object towards the initial position of the player
        GetComponent<Rigidbody2D>().velocity = directionToInitialPosition * speed;

        // Check if the object is close enough to the initial position of the player
        if (Vector2.Distance(transform.position, initialPlayerPosition) <= distanceThreshold)
        {
            Explode();
        }
    }

    // Function to handle explosion
    void Explode()
    {
        anim.SetTrigger("Explode");
        Instantiate(ObjectToSpawn, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
