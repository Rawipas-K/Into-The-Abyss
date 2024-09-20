using UnityEngine;

public class UpgradeStat : MonoBehaviour
{
    public static UpgradeStat instance;
    public GameObject CoinPickup;
    public GameObject AmmoBoxPickup;
    private int amountToDrop;

    private void Awake()
    {
        instance = this;
    }

    public void UpgradeCharacter(string selectedUpgrade)
    {
        switch(selectedUpgrade)
        {
            case "HP":
                PlayerStats.instance.IncreasedMaxHealth(1);
                PlayerStats.instance.HealPlayer(1);
                break;
            case "DMG":
                PlayerStats.instance.bonusDamage += 0.1f;
                break;   
            case "MoveSpeed":
                PlayerStats.instance.bonusMoveSpeed += 0.03f;
                break;  
            case "CriticalChance":
                PlayerStats.instance.bonusCriticalChance += 0.2f;
                break;
            case "CriticalDmg":
                PlayerStats.instance.bonusCriticalMultiplier += 0.1f;
                break;
            case "ReloadTime":
                PlayerStats.instance.bonusReloadTime -= 0.05f;
                break;
            case "ShootingTime":
                PlayerStats.instance.bonusShootInterval -= 0.05f;
                break;
            case "ShootRange":
                PlayerStats.instance.bonusBulletLifeTime += 0.05f;
                break;
            case "Magazine":
                PlayerStats.instance.bonusMaxAmmoInMagazine += 0.1f;
                break;
            case "Endurance":
                PlayerStats.instance.IncreasedMaxHealth(2);
                PlayerStats.instance.HealPlayer(2);
                PlayerStats.instance.bonusShootInterval += 0.05f;
                break;
            case "Berserker":
                PlayerStats.instance.bonusDamage += 0.25f;
                if (PlayerStats.instance.maxHealth > 1)
                {
                    PlayerStats.instance.IncreasedMaxHealth(-1);
                }
                break;
            case "SpeedStar":
                PlayerStats.instance.bonusMoveSpeed += 0.05f;
                if (PlayerStats.instance.maxHealth > 1)
                {
                    PlayerStats.instance.IncreasedMaxHealth(-1);
                }
                break;
            case "Bulleye":
                PlayerStats.instance.bonusCriticalChance += 0.5f;
                PlayerStats.instance.bonusMaxAmmoInMagazine -= 0.1f;
                break;
            case "Carry":
                PlayerStats.instance.bonusMaxAmmoInMagazine += 0.25f;
                PlayerStats.instance.bonusReloadTime += 0.1f;
                break;
            case "Steadfast":
                PlayerStats.instance.bonusDamage += 0.25f;
                PlayerStats.instance.bonusMoveSpeed -= 0.03f;
                break;
            case "FastHand":
                PlayerStats.instance.bonusShootInterval -= 0.15f;
                PlayerStats.instance.bonusDamage -= 0.15f;
                break;
            case "Snipe": 
                PlayerStats.instance.bonusBulletLifeTime += 0.25f;
                PlayerStats.instance.bonusShootInterval += 0.10f;
                break;
            case "LuckyShot":
                PlayerStats.instance.bonusCriticalChance += 0.4f;
                PlayerStats.instance.bonusCriticalMultiplier += 0.3f;
                LevelManager.instance.GetCoins(-Mathf.RoundToInt(PlayerStats.instance.currentCoins * 0.5f));
                break;
            case "Heal":
                PlayerStats.instance.HealPlayer(PlayerStats.instance.maxHealth);
                break;
            case "Coin":
                amountToDrop = Random.Range(7, 12 + 1);
                for (int i = 0; i < amountToDrop; i++)
                {
                    Vector3 dropPosition = PlayerStats.instance.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
                    Instantiate(CoinPickup, dropPosition, Quaternion.identity);
                }
                break;
            case "Ammo":
                amountToDrop = Random.Range(5, 7 + 1);
                for (int i = 0; i < amountToDrop; i++)
                {
                    Vector3 dropPosition = PlayerStats.instance.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
                    Instantiate(AmmoBoxPickup, dropPosition, Quaternion.identity);
                }
                break;
        }
    }
}
