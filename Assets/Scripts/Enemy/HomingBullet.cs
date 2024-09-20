using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public float speed = 5f;
    public float homingDuration = 1.5f;
    public float straightDuration = 1.5f;
    public float homingStrength = 0.5f; // Strength of homing effect (0 = no homing, 1 = full homing)
    
    private GameObject player;
    private bool isHoming = true;
    private Rigidbody2D rb;
    private Vector2 initialDirection;
    private float timer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        initialDirection = (player.transform.position - transform.position).normalized;
        rb.velocity = initialDirection * speed;
        AudioManager.instance.PlaySFX("EnemyShoot");
    }

    void Update()
    {
        if (isHoming)
        {
            HomingUpdate();
        }
        else
        {
            StraightUpdate();
        }
    }

    void HomingUpdate()
    {
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
        Vector2 newDirection = Vector2.Lerp(rb.velocity.normalized, directionToPlayer, homingStrength);
        rb.velocity = newDirection * speed;

        timer += Time.deltaTime;
        if (timer >= homingDuration)
        {
            timer = 0f;
            isHoming = false;
        }
    }

    void StraightUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= straightDuration)
        {
            timer = 0f;
            isHoming = true;
            rb.velocity = initialDirection * speed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Handle player damage
            PlayerStats.instance.DamagePlayer(1);
            Destroy(gameObject);
        }
        else
        {
            // Handle collision with other objects
            Destroy(gameObject);
        }
    }
}
