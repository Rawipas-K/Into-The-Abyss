using UnityEngine;

public class LandMine : MonoBehaviour
{
    public GameObject explodeEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject explosion = Instantiate(explodeEffect, transform.position, Quaternion.identity);
            
            // Get the existing scale of the explosion GameObject
            Vector3 currentScale = explosion.transform.localScale;

            // Modify only the x and y scales, keeping the z scale unchanged
            explosion.transform.localScale = new Vector3(1.5f * currentScale.x, 1.5f * currentScale.y, currentScale.z);

            AudioManager.instance.PlaySFX("Explosion");
            Destroy(gameObject);
        }
    }
}
