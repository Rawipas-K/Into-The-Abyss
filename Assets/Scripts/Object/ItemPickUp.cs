using UnityEngine;

public enum PickupType
{
    Coin,
    Health,
    Key,
    Ammo
}

public class ItemPickup : MonoBehaviour
{
    public PickupType pickupType;
    public int amount;
    public float delayBeforePickup = 0.25f;

    private bool collidedWithObstacle = false;

    // Update is called once per frame
    void Update()
    {
        if (delayBeforePickup > 0)
        {
            delayBeforePickup -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && delayBeforePickup <= 0)
        {
            PickUpItem();
        }
        else if (other.CompareTag("Obstacle"))
        {
            // Set collidedWithObstacle to true if collided with an obstacle
            collidedWithObstacle = true;
        }
    }

    private void PickUpItem()
    {
        switch (pickupType)
        {
            case PickupType.Health:
                if (PlayerStats.instance.currentHealth < PlayerStats.instance.maxHealth)
                {
                    PlayerStats.instance.HealPlayer(amount);
                    AudioManager.instance.PlaySFX("PickupHealth");
                    Destroy(gameObject);
                }
                break;
            case PickupType.Coin:
                LevelManager.instance.GetCoins(amount);
                AudioManager.instance.PlaySFX("PickupCoin");
                LevelManager.instance.GetScore(5);
                Destroy(gameObject);
                break;
            case PickupType.Key:
                LevelManager.instance.GetKey(amount);
                AudioManager.instance.PlaySFX("Buy");
                Destroy(gameObject);
                break;
            case PickupType.Ammo:
                Gun currentGun = PlayerInventory.instance.availableGuns[PlayerInventory.instance.currentGun];
                currentGun.maxAmmo += currentGun.ammoPerPickup;
                AudioManager.instance.PlaySFX("PickupGun");
                Destroy(gameObject);
                break;
        }
    }
}
