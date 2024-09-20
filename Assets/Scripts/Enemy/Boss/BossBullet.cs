using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed;
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        direction = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerStats.instance.DamagePlayer(1);
        }

        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}

