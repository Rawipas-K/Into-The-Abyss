using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public int damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerStats.instance.DamagePlayer(damage);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerStats.instance.DamagePlayer(damage);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerStats.instance.DamagePlayer(damage);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerStats.instance.DamagePlayer(damage);
        }
    } 

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void PlaySFX()
    {
        AudioManager.instance.PlaySFX("BossThunderStrike");
    }
}
