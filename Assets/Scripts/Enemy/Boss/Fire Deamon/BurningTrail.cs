using UnityEngine;

public class BurningTrail : MonoBehaviour
{
    public Animator anim;
    public float Duration = 10f;
    private float countdownTimer;

    // Start is called before the first frame update
    void Start()
    {
        countdownTimer = Duration;
    }

    // Update is called once per frame
    void Update()
    {
        countdownTimer -= Time.deltaTime;

        if (countdownTimer <= 0f)
        {
            anim.SetTrigger("End");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerStats.instance.DamagePlayer(1);
        }
    }

        private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerStats.instance.DamagePlayer(1);
        }
    }
    
        private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerStats.instance.DamagePlayer(1);
        }
    }

        private void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerStats.instance.DamagePlayer(1);
        }
    } 
    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
