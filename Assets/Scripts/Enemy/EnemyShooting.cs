using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    EnemyController enemyController;
    [Header("Shooting")]
    public GameObject bullet;
    public Transform firePoint;
    public float fireRate;
    private float fireCounter;
    public float shootRange;
    public int bulletPerShot;
    public float spreadAngle;

    [Header("ChargeBullet")]
    public bool isChargeAttackType;
    public float chargeDuration = 1f;
    
    public bool isCharging; // Flag to track if the bullet is currently charging
    private float currentChargeTime = 0f; // Current charge time
    private GameObject chargingBullet; // Reference to the charging bullet instance

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        fireCounter = fireRate;
    }

    void Update()
    {
        if (enemyController.theBody.isVisible && PlayerController.instance.gameObject.activeInHierarchy)
        {
            if (!isChargeAttackType && Vector3.Distance(transform.position, PlayerController.instance.transform.position) < shootRange)
            {
                fireCounter -= Time.deltaTime;

                if (fireCounter <= 0 && bulletPerShot == 1)
                {
                    fireCounter = fireRate;
                    // Instantiate bullet with random rotation
                    GameObject newBullet = Instantiate(bullet, firePoint.transform.position, Quaternion.identity);
                        
                    // Set bullet direction and rotation
                    newBullet.GetComponent<EnemyBullet>().SetDirection((PlayerController.instance.transform.position - firePoint.transform.position).normalized, Quaternion.identity);
                    AudioManager.instance.PlaySFX("EnemyShoot");
                }
                else if (fireCounter <= 0)
                {
                    fireCounter = fireRate;
                    for (int i = 0; i < bulletPerShot; i++)
                    {
                        // Calculate direction towards player
                        Vector3 directionToPlayer = (PlayerController.instance.transform.position - firePoint.position).normalized;

                        // Introduce spread in the bullet directions
                        float angle = Random.Range(-spreadAngle, spreadAngle);
                        Quaternion spreadRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                        Vector3 spreadDirection = spreadRotation * directionToPlayer;

                        // Normalize the spread direction
                        spreadDirection.Normalize();

                        // Instantiate bullet with spread direction
                        GameObject newBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
                        newBullet.GetComponent<EnemyBullet>().SetDirection(spreadDirection, Quaternion.identity);
                        
                        AudioManager.instance.PlaySFX("EnemyShoot");
                    }
                }
            }

            if (isChargeAttackType && Vector3.Distance(transform.position, PlayerController.instance.transform.position) < shootRange)
            {
                // Decrement fireCounter
                fireCounter -= Time.deltaTime;

                // If fireCounter reaches zero or less, shoot a bullet
                if (fireCounter <= 0)
                {
                    if (!isCharging)
                    {
                        StartCharging();
                    }
                    else
                    {
                        ShootChargedBullet();
                    }
                }
            }
        }
    }

    void StartCharging()
    {
        isCharging = true;
        currentChargeTime = 0f;
        chargingBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
        chargingBullet.SetActive(true); // Enable rendering of the charging bullet
        AudioManager.instance.PlaySFX("EnemyDash");
    }

    void ShootChargedBullet()
    {
        // Check if chargingBullet is not null
        if (chargingBullet != null)
        {
            GetComponent<EnemyMovement>().canMove = false;
            // Increase the size of the charging bullet gradually
            currentChargeTime += Time.deltaTime;
            float chargeProgress = currentChargeTime / chargeDuration;
            float bulletScale = Mathf.Lerp(0.05f, 0.2f, chargeProgress); // Scale the bullet from 0.5 to 1.5
            chargingBullet.transform.localScale = Vector3.one * bulletScale; // Gradually increase the scale

            // If charging is complete, shoot the bullet
            if (currentChargeTime >= chargeDuration)
            {
                // Shoot the charged bullet towards the player
                Vector3 direction = (PlayerController.instance.transform.position - firePoint.position).normalized;
                chargingBullet.GetComponent<Rigidbody2D>().velocity = direction * 25f; // Set velocity for shooting
                chargingBullet = null; // Clear the charging bullet reference
                isCharging = false; // Reset charging flag
                fireCounter = fireRate; // Reset fireCounter
                GetComponent<EnemyMovement>().canMove = true;
            }
        }
        else
        {
            // If no charging bullet, resume shooting
            fireCounter = fireRate; // Reset fireCounter
        }
    }
}
