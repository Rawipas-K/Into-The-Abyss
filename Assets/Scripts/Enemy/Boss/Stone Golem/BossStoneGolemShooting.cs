using UnityEngine;

public class BossStoneGolemShooting : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bullet;
    public Transform firePoint;

    private void ShootBullet()
    {
        GameObject newBullet = Instantiate(bullet, firePoint.transform.position, Quaternion.identity);      
        newBullet.GetComponent<EnemyBullet>().SetDirection((PlayerController.instance.transform.position - firePoint.transform.position).normalized, Quaternion.identity);
        AudioManager.instance.PlaySFX("EnemyShoot");
    }
}
