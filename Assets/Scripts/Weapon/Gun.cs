using UnityEngine;

public class Gun : MonoBehaviour
{   
    [Header("Gun Info")]
    public int GunID;
    public string weaponName;
    public Sprite gunUI;
    public GameObject gunPickup;
    public GameObject bullet;
    public Transform firePoint;
    public int itemCost;
    
    [Header("Gun Stat")]
    public float damage;
    public float bulletSpeed;
    public float bulletLifeTime;
    public float criticalChance;
    public float criticalMultiplier;
    public int shotPerClick;
    public float scoreMultiplier;
    public float shootInterval;
    public float reloadTime;
    public float maxAmmo, maxAmmoInMagazine;

    public int ammoPerPickup;

    private float shotTimer, reloadTimer;
    [HideInInspector] public float ammoCount, ammoShotCount;

    //variable for check if stats need update
    private float baseDamage, baseBulletSpeed, baseBulletLifeTime, baseBulletSize, baseCriticalChance, baseCriticalMultiplier, 
    baseShootInterval, baseReloadTime, baseMaxAmmoInMagazine;

    // Start is called before the first frame update
    void Start()
    {
        baseDamage = damage;
        baseBulletSpeed = bulletSpeed;
        baseBulletLifeTime = bulletLifeTime;
        baseCriticalChance = criticalChance;
        baseCriticalMultiplier = criticalMultiplier;
        baseShootInterval = shootInterval;
        baseReloadTime = reloadTime;
        baseMaxAmmoInMagazine = maxAmmoInMagazine;

        ammoCount = maxAmmoInMagazine;
        UIController.instance.ammoText.text = ammoCount.ToString() + " / " + maxAmmo.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyStatUpgraded();

        if(PlayerController.instance.canMove && !LevelManager.instance.isPaused)
        {
            Gun currentGun = PlayerInventory.instance.availableGuns[PlayerInventory.instance.currentGun];

            ShootBullet(currentGun);

            ReloadAmmo(currentGun);
            
        }
    }

    //Shoot Bullet
    private void ShootBullet(Gun currentGun)
    {
        if(shotTimer > 0)
        {
            shotTimer -= Time.deltaTime;
        } 
        else
        {
            if((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && (ammoCount > 0) && (reloadTimer <= 0))
            {
                if(shotPerClick == 1)
                {
                    // Instantiate bullet and set its stats
                    GameObject bulletToFire = Instantiate(bullet, firePoint.position, firePoint.rotation);

                    if (weaponName == "Rocket Launcher")
                    {
                        bulletToFire.GetComponent<ExplodingBullet>().SetBulletStat(damage, bulletSpeed, bulletLifeTime, criticalChance, criticalMultiplier, scoreMultiplier);
                    }
                    else
                    {
                        bulletToFire.GetComponent<PlayerBullet>().SetBulletStat(damage, bulletSpeed, bulletLifeTime, criticalChance, criticalMultiplier, scoreMultiplier);
                    }
                        
                    shotTimer = shootInterval;
                    ammoCount--;
                    ammoShotCount++;
                    UIController.instance.ammoText.text = ammoCount.ToString() + " / " + maxAmmo.ToString();
                    if (weaponName == "Plasma Blaster")
                    {
                        AudioManager.instance.PlaySFX("FireLaser");
                    }
                    else 
                    {
                        AudioManager.instance.PlaySFX("PlayerShoot");
                    }
                }
                else
                {
                    for (int i = 0; i < shotPerClick; i++)
                    {
                        // Introduce some spread in the bullet directions
                        Quaternion bulletRotation = firePoint.rotation;
                        bulletRotation = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0, Random.Range(-20f, 20f), Random.Range(-7.5f, 7.5f)));

                        // Instantiate bullet and set its stats
                        GameObject bulletToFire = Instantiate(bullet, firePoint.position, bulletRotation);
                        bulletToFire.GetComponent<PlayerBullet>().SetBulletStat(damage, bulletSpeed, bulletLifeTime, criticalChance, criticalMultiplier, scoreMultiplier);
                        AudioManager.instance.PlaySFX("PlayerShoot");
                    }

                    shotTimer = shootInterval;
                    ammoCount--;
                    ammoShotCount++;
                    UIController.instance.ammoText.text = ammoCount.ToString() + " / " + maxAmmo.ToString();
                }
            }
        }
    }

    //Reload ammo
    private void ReloadAmmo(Gun currentGun)
    {
        if(reloadTimer > 0)
        {
            reloadTimer -= Time.deltaTime;
        }
        else
        {
            if((Input.GetKeyDown(KeyCode.R) && (ammoShotCount > 0) && (maxAmmo > 0)) 
            || ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && (ammoCount == 0) && (maxAmmo > 0)))
            {
                float reloadTime = currentGun.reloadTime; // Set reload time based on current gun

                ReloadBar.instance.StartReload(reloadTime); // Call StartReload() with reload time
                    
                if(ammoShotCount <= maxAmmo)
                {
                    reloadTimer = reloadTime;

                    maxAmmo -= ammoShotCount;
                    ammoCount = maxAmmoInMagazine;
                    ammoShotCount = 0;
                    AudioManager.instance.PlaySFX("GunReload");
                }
                else
                {
                    reloadTimer = reloadTime;

                    ammoCount += maxAmmo;
                    maxAmmo -= maxAmmo;
                    ammoShotCount = 0;
                    AudioManager.instance.PlaySFX("GunReload");
                }
            }

            if(reloadTimer <= 0)
            {
                UIController.instance.ammoText.text = ammoCount.ToString() + " / " + maxAmmo.ToString();
            }
        }
    }

    public void DropGunPickup(Vector3 position)
    {
        // Instantiate a new instance of the gunPickup prefab
        GameObject newGunPickup = Instantiate(gunPickup, position, Quaternion.identity);
        
        // Get the GunPickup component from the new instance and call OldGun 
        GunPickup newGunPickupComponent = newGunPickup.GetComponent<GunPickup>();
        newGunPickupComponent.OldGun(GunID);
    }

    public void ApplyStatUpgraded()
    {
        damage = CalculateStat(baseDamage, PlayerStats.instance.bonusDamage);
        bulletSpeed = CalculateStat(baseBulletSpeed, PlayerStats.instance.bonusBulletSpeed);
        bulletLifeTime = CalculateStat(baseBulletLifeTime, PlayerStats.instance.bonusBulletLifeTime);
        criticalChance = CalculateStat(baseCriticalChance, PlayerStats.instance.bonusCriticalChance);
        criticalMultiplier = CalculateStat(baseCriticalMultiplier, PlayerStats.instance.bonusCriticalMultiplier);
        shootInterval = CalculateStat(baseShootInterval, PlayerStats.instance.bonusShootInterval);
        reloadTime = CalculateStat(baseReloadTime, PlayerStats.instance.bonusReloadTime);
        maxAmmoInMagazine = Mathf.FloorToInt(CalculateStat(baseMaxAmmoInMagazine, PlayerStats.instance.bonusMaxAmmoInMagazine));
    }

    private float CalculateStat(float baseStat, float bonus)
    {
        return baseStat * (1 + bonus);
    }
}