using UnityEngine;

public class ForceField : MonoBehaviour
{
    public GameObject HitEffect;
    private bool disableForceField;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            disableForceField = true;
        }

        if (collision.CompareTag("PlayerBullet") && !disableForceField)
        {
            Instantiate(HitEffect, collision.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            disableForceField = false;
        }
    }
}