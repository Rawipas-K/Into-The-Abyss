using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Rigidbody2D theRB;
    public SpriteRenderer theBody;
    public Animator anim;
    public int bossFloor;

    [Header("Variables")]
    public float maxHealth;
    public float health;
    public int exp, score;
    [HideInInspector] public bool isDeath = false;

    [Header("Drop")]
    public bool shouldDropItem;
    public ItemDrop[] itemsToDrop;

    public GameObject  hitEffect, deathEffect, levelExit;

    [System.Serializable]
    public class ItemDrop
    {
        public GameObject item;
        [Range(0f, 100f)] public float dropChance;
        public int minAmount;
        public int maxAmount;
    }
    

    private void Awake()
    {
        if (PlayerStats.instance.currentFloor == bossFloor)
        {
            gameObject.SetActive(true);
        }
        else 
        {
            //gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        UIController.instance.bossHealthBar.maxValue = maxHealth;
        UIController.instance.bossHealthBar.value = health;
        AudioManager.instance.PlaySFX("Appearing");
    }

    public void DamageBoss(float damage, bool isCritical)
    {
        if(theBody != null)
        {
            enemyHitFlash();
        }

        health -= damage;

        AudioManager.instance.PlaySFX("EnemyHurt");

        Instantiate(hitEffect, transform.position, transform.rotation);

        UIController.instance.bossHealthBar.value = health;

        if(isCritical)
        {
            DamageNumberController.instance.SpawnDamage(damage, transform.position, true);
        }
        else
        {
            DamageNumberController.instance.SpawnDamage(damage, transform.position, false);
        }

        if(health <= 0 && !isDeath)
        {
            isDeath = true;
        
            AudioManager.instance.PlaySFX("EnemyDeath");

            LevelManager.instance.GetScore(score);
            PlayerStats.instance.GetExp(exp);
            UIController.instance.bossHealthBar.gameObject.SetActive(false);
            
            Instantiate(deathEffect, transform.position, transform.rotation);

            if(Vector3.Distance(PlayerController.instance.transform.position, levelExit.transform.position) < 2f)
            {
                levelExit.transform.position += new Vector3(4f, 0f, 0f);
            }

            levelExit.SetActive(true);

            if (shouldDropItem)
            {
                DropItems();
            }

            DeathAnimation();
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

    public void DeathAnimation()
    {
        anim.SetTrigger("Death");
    }

    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}