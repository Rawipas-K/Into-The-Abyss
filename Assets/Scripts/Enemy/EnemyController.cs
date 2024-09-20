using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody2D theRB;    
    public SpriteRenderer theBody;
    public Animator anim;
    
    [Header("Variables")]
    public float maxHealth;
    public float health;
    public int exp, score;
    [HideInInspector] public bool isDeath;

    [Header("Drop")]
    public bool shouldDropItem;
    public ItemDrop[] itemsToDrop;

    public GameObject[] deathSplatters;
    public GameObject hitEffect;

    [System.Serializable]
    public class ItemDrop
    {
        public GameObject item;
        [Range(0f, 100f)] public float dropChance;
        public int minAmount;
        public int maxAmount;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        AudioManager.instance.PlaySFX("Appearing");
    }

    public void DamageEnemy(float damage, bool isCritical)
    {
        if(theBody != null)
        {
            enemyHitFlash();
        }

        health -= damage;

        AudioManager.instance.PlaySFX("EnemyHurt");

        if(isCritical)
        {
            DamageNumberController.instance.SpawnDamage(damage, transform.position, true);
        }
        else
        {
            DamageNumberController.instance.SpawnDamage(damage, transform.position, false);
        }

        Instantiate(hitEffect, transform.position, transform.rotation);
        
        if(health <= 0 && !isDeath)
        {
            isDeath = true;

            Destroy(gameObject);
        
            AudioManager.instance.PlaySFX("EnemyDeath");

            LevelManager.instance.GetScore(score);
            PlayerStats.instance.GetExp(exp);
            
            int selectSplatter = Random.Range(0, deathSplatters.Length);

            Instantiate(deathSplatters[selectSplatter], transform.position, transform.rotation);

            if (shouldDropItem)
            {
                DropItems();
            }
        }
    }

    public void enemyHitFlash()
    {
        StartCoroutine(enemyHitFlashing());
    }

    public IEnumerator enemyHitFlashing()
    {
        theBody.color = new Color(255, 0, 0);
        yield return new WaitForSeconds(.1f);
        yield return null;
            
        theBody.color = new Color(255, 255, 255);
        yield return new WaitForSeconds(.5f);
        yield return null;
    }

    void DropItems()
    {
        foreach (var itemDrop in itemsToDrop)
        {
            float dropChance = Random.Range(0f, 100f);
            if (dropChance < itemDrop.dropChance)
            {
                int amountToDrop = Random.Range(itemDrop.minAmount, itemDrop.maxAmount + 1);
                for (int i = 0; i < amountToDrop; i++)
                {
                    Vector3 dropPosition = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
                    Instantiate(itemDrop.item, dropPosition, Quaternion.identity);
                }
            }
        }
    }

    public void Heal(float amount)
    {
        health = Mathf.Clamp(health + amount, 0f, maxHealth);
    }

}