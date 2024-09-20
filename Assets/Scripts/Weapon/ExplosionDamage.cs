using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [Header("Explosion Info")]   
    private float damage;
    private float scoreMultiplier;
    private float criticalChance;
    private float criticalMultiplier;

    private bool isCritical = false;

    // Method to set the value for the bullet
    public void SetExplosionStat(float damage, float criticalChance, float criticalMultiplier, float scoreMultiplier)
    {
        this.damage = damage;
        this.criticalChance = criticalChance;
        this.criticalMultiplier = criticalMultiplier;
        this.scoreMultiplier = scoreMultiplier;

        // Check if the hit is critical
        if (Random.Range(0f, 100f) < criticalChance)
        {
            this.damage *= criticalMultiplier;
            isCritical = true;
            Debug.Log("Critical Hit!"); // Optional: Log critical hit
        }
    }

    // Triggered on collision with solid objects
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Enemy")
        {
            collision.collider.gameObject.GetComponent<EnemyController>().DamageEnemy(damage, isCritical);
            LevelManager.instance.GetScoreMultiplier(scoreMultiplier);
        }
        else if (collision.collider.gameObject.tag == "Boss")
        {
            collision.collider.GetComponent<BossController>().DamageBoss(damage, isCritical);
            LevelManager.instance.GetScoreMultiplier(scoreMultiplier);
        }
        else if (collision.collider.gameObject.tag == "Breakable")
        {
            // Implement Breakable object damage logic here
        }
        else if (collision.collider.gameObject.tag == "Player")
        {
            PlayerStats.instance.DamagePlayer(1);
        }
        else
        {
            LevelManager.instance.GetScoreMultiplier(-scoreMultiplier * 2);
        }
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
