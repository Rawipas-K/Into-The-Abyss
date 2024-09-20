using UnityEngine;

public class SpikeBox : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float checkDelay;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer; 
    private Vector3[] direction = new Vector3[4];
    private Vector3 destination;
    private float checkTimer;
    private bool attacking;

    // Update is called once per frame
    void Update()
    {
        //Move spikebox to destination only if attacking
        if (attacking)
        {
            transform.Translate(destination * Time.deltaTime * speed);
        }
        else
        {
            checkTimer += Time.deltaTime;
            if (checkTimer > checkDelay)
            {
                CheckForPlayer();
            }
        }
    }
    
    private void CheckForPlayer()
    {
        CalculateDirection();

        //check if spikebox see player in all 4 direction
        for (int i = 0; i < direction.Length; i++)
        {
            Debug.DrawRay(transform.position, direction[i], Color.red);
            
            // Perform the raycast with layerMask
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction[i], range, playerLayer);

            // Check if the obstacle is hit before the player
            RaycastHit2D obstacleHit = Physics2D.Raycast(transform.position, direction[i], range, obstacleLayer);

            if (hit.collider != null && !attacking && (obstacleHit.collider == null || obstacleHit.distance > hit.distance))
            {
                attacking = true;
                destination = direction[i];
                checkTimer = 0;
            }
        }
    }

    private void CalculateDirection()
    {
        direction[0] = transform.right * range; // right
        direction[1] = -transform.right * range; // left
        direction[2] = transform.up * range; // up
        direction[3] = -transform.up * range; // down
    }    
    
    private void Stop()
    {
        destination = transform.position; // set destination as current position so it doesn't move
        attacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            PlayerStats.instance.DamagePlayer(1);
        }
        Stop();
    }
}
