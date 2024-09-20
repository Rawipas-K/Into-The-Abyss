using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public bool isWin, isLastBossDefeated;
    public int maxHealth, currentHealth;
    public float currentScoreMultiplier;
    public float damageInvincLength = 1f;
    private float invincTimer;
    [HideInInspector] public bool isInvinc;

    public int currentLevel;
    public int maxExp, currentExp ,excessExp;

    public int currentCoins, currentKey, currentScore, currentFloor;

    public float moveSpeed;
    public float dashSpeed = 5f, dashLength = .4f, dashCooldown = 1f, dashInvincibility = .5f;

    [Header("Bonus stats from upgraded")]
    public float bonusDamage;
    public float bonusMoveSpeed;    
    public float bonusCriticalChance;
    public float bonusCriticalMultiplier;
    public float bonusShootInterval;
    public float bonusReloadTime;
    public float bonusBulletSpeed;
    public float bonusBulletLifeTime;
    public float bonusMaxAmmoInMagazine;

    public float initialTime;
    public bool startTimer;
    public string playerName;
    public float elapsedTime;
    public string timeText;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        maxExp = currentLevel * 5;

        //HP
        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        //Level & Exp
        UIController.instance.levelSlider.maxValue = maxExp;
        UIController.instance.levelSlider.value = currentExp;
        UIController.instance.levelText.text = "Lv." + currentLevel.ToString();

        //MoveSpeed
        PlayerController.instance.moveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(invincTimer > 0)
        {
            invincTimer -= Time.deltaTime;

            if(invincTimer <= 0)
            {
                PlayerController.instance.bodySR.color = new Color(PlayerController.instance.bodySR.color.r, PlayerController.instance.bodySR.color.g, PlayerController.instance.bodySR.color.b, 1f);
            }
        }
    }

    public void DamagePlayer(int damage)
    {
        if(invincTimer <= 0)
        {
            AudioManager.instance.PlaySFX("PlayerHurt");

            CameraShake.instance.ShakeCamera(3f, 3f, 0.75f);

            currentHealth -= damage;
            invincTimer = damageInvincLength;
            isInvinc = true;

            PlayerController.instance.bodySR.color = new Color(PlayerController.instance.bodySR.color.r, PlayerController.instance.bodySR.color.g, PlayerController.instance.bodySR.color.b, .5f);

            if(currentHealth <= 0)
            {
                isWin = false;
                PlayerDataManager.instance.GetPlayerData();
                AudioManager.instance.PlaySFX("PlayerDeath");
                UIController.instance.WhenPlayerDeath();
            }

            UIController.instance.UpdatePlayerUI("HP");
            LevelManager.instance.GetScoreMultiplier(-0.5f);
        }
        else
        {
            isInvinc = false;
        }
    }

    public void MakeInvincible(float length)
    {
        invincTimer = length;
        PlayerController.instance.bodySR.color = new Color(PlayerController.instance.bodySR.color.r, PlayerController.instance.bodySR.color.g, PlayerController.instance.bodySR.color.b, .5f);
    }

    public void HealPlayer(int healAmount)
    {
        currentHealth += healAmount;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UIController.instance.UpdatePlayerUI("HP");
    }

    public void IncreasedMaxHealth(int amount)
    {
        maxHealth += amount;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UIController.instance.UpdatePlayerUI("HP");       
    }

    public void GetExp(int amount)
    {
        currentExp += amount;
        UIController.instance.UpdatePlayerUI("Level");
        Debug.Log("Exp = " + amount);
        LevelUp();
    }

    public void LevelUp()
    {
        if(currentExp >= maxExp)
        {
            excessExp = currentExp - maxExp;
            currentLevel += 1;
            maxExp += 5; 
            currentExp = 0 + excessExp;

            UIController.instance.UpdatePlayerUI("Level");
            Debug.Log("LevelUp Work");
            LevelManager.instance.LevelUpPause();
        }
    }

    public IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(2f);

        if (!startTimer)
        {
            startTimer = true;
            initialTime = Time.time;
        }
    }

    public void CalElapsedTime()
    {
        float winTime = Time.time;
        elapsedTime = winTime - initialTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 1000f) % 1000f);

        timeText = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}