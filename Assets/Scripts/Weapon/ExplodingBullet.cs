using UnityEngine;

public class ExplodingBullet : MonoBehaviour
{
    [Header("Bullet Info")]    
    public Rigidbody2D theRB;
    public GameObject impactEffect;

    private float damage;
    private float bulletSpeed;
    private float bulletLifeTime;
    private float scoreMultiplier;
    private float criticalChance;
    private float criticalMultiplier;

    private Vector3 targetPosition; // Mouse position when bullet was fired

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < 0.5f) // Adjust the threshold value as needed
        {
            Explode();
        }

        bulletLifeTime -= Time.deltaTime;
        if (bulletLifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Method to set the value for the bullet
    public void SetBulletStat(float damage, float bulletSpeed, float bulletLifeTime, float criticalChance, float criticalMultiplier, float scoreMultiplier)
    {
        this.damage = damage;
        this.bulletSpeed = bulletSpeed;
        this.bulletLifeTime = bulletLifeTime;
        this.criticalChance = criticalChance;
        this.criticalMultiplier = criticalMultiplier;
        this.scoreMultiplier = scoreMultiplier;

        theRB.velocity = transform.right * bulletSpeed;
        Vector3 mousePos = Input.mousePosition;
        targetPosition = Camera.main.ScreenToWorldPoint(mousePos); // Store the mouse position when bullet was fired
        targetPosition.z = 0;
    }

    void Explode()
    {
        // Instantiate the impact effect at the mouse position when bullet was fired
        GameObject explosion = Instantiate(impactEffect, transform.position, Quaternion.identity);
        explosion.GetComponent<ExplosionDamage>().SetExplosionStat(damage, criticalChance, criticalMultiplier, scoreMultiplier);
        AudioManager.instance.PlaySFX("Explosion");

        Destroy(gameObject);
    }

    // Triggered on collision with solid objects
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Enemy")
        {

        }
        else if (collision.collider.gameObject.tag == "Boss")
        {

        }
        else if (collision.collider.gameObject.tag == "Breakable")
        {
            // Implement Breakable object damage logic here
        }
        else
        {
            PlayerStats.instance.currentScoreMultiplier -= scoreMultiplier;
        }

        Explode();
    }
}
