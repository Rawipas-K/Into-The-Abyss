using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Rigidbody2D theRB;
    public float speed;
    private Vector3 direction;

    public void SetDirection(Vector3 direction, Quaternion rotation)
    {
        this.direction = direction;
        theRB.velocity = direction * speed;
        transform.rotation = rotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            PlayerStats.instance.DamagePlayer(1);
        }

        Destroy(gameObject);
        AudioManager.instance.PlaySFX("Impact");
    }
}
