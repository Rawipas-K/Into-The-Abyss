using UnityEngine;

public class PlayerBullet : MonoBehaviour
{   
    [Header("Bullet Info")]    
    public Rigidbody2D theRB;
    public GameObject impactEffect;
    public bool isPenetrate;

    private float damage;
    private float bulletSpeed;
    private float bulletLifeTime;
    private float scoreMultiplier;
    private float criticalChance;
    private float criticalMultiplier;

    private bool isCritical = false;

    // Update is called once per frame
    void Update()   
    {
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

        // Check if the hit is critical
        if (Random.Range(0f, 100f) < criticalChance)
        {
            this.damage *= criticalMultiplier;
            isCritical = true;
            Debug.Log("Critical Hit!");
        }
    }

    // Triggered on collision with solid objects
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))  
        {
            collision.collider.GetComponent<EnemyController>().DamageEnemy(damage, isCritical);
            LevelManager.instance.GetScoreMultiplier(scoreMultiplier);
        }
        else if (collision.collider.CompareTag("Boss"))  
        {
            collision.collider.GetComponent<BossController>().DamageBoss(damage, isCritical);
            LevelManager.instance.GetScoreMultiplier(scoreMultiplier);
        }
        else if (collision.collider.CompareTag("Breakable"))  
        {

        }

        Destroy(gameObject);

        // Always instantiate impact effect and play sound
        Instantiate(impactEffect, transform.position, transform.rotation);
        AudioManager.instance.PlaySFX("Impact");
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))  
        {
            collider.GetComponent<EnemyController>().DamageEnemy(damage, isCritical);
            LevelManager.instance.GetScoreMultiplier(scoreMultiplier);
            Instantiate(impactEffect, transform.position, transform.rotation);
            AudioManager.instance.PlaySFX("Impact");
        }
        else if (collider.CompareTag("Boss"))  
        {
            collider.GetComponent<BossController>().DamageBoss(damage, isCritical);
            LevelManager.instance.GetScoreMultiplier(scoreMultiplier);
            Instantiate(impactEffect, transform.position, transform.rotation);
            AudioManager.instance.PlaySFX("Impact");
        }
        else if (collider.CompareTag("Breakable"))  
        {

        }
        else if (collider.CompareTag("Obstacle")) // Hit an obstacle
        {
            Destroy(gameObject);
            Instantiate(impactEffect, transform.position, transform.rotation);
            AudioManager.instance.PlaySFX("Impact");
        }
    }
}
