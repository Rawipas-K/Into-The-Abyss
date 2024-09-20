using System.Collections;
using UnityEngine;

public class SlimePuddle : MonoBehaviour
{
    public float slowdownFactor = 0.5f; // Adjust this value to determine how much you want to slow down the player.
    public float puddleDuration = 15f; // Duration for which the puddle remains active
    public float fadeDuration = 2f; // Duration over which the puddle fades out

    private float originalMoveSpeed;
    private float originalDashSpeed;

    void Start()
    {
        // Generate a random rotation angle from the set {0, 45, 90, 135} degrees
        float randomRotation = Random.Range(0, 4) * 45f;

        // Apply the random rotation
        transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

        // Start coroutine to fade and destroy the puddle after the specified duration
        StartCoroutine(FadeAndDestroy());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !PlayerController.instance.isSlowed)
        {
            PlayerController.instance.isSlowed = true;

            // Store original move speed and dash speed
            originalMoveSpeed = PlayerStats.instance.moveSpeed;
            originalDashSpeed = PlayerStats.instance.dashSpeed;

            // Slow down the player by adjusting its move speed and dash speed
            PlayerController.instance.moveSpeed *= slowdownFactor;
            PlayerController.instance.activeMoveSpeed *= slowdownFactor;
            PlayerController.instance.dashSpeed *= slowdownFactor;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !PlayerController.instance.isSlowed)
        {
            PlayerController.instance.isSlowed = true;

            // Store original move speed and dash speed
            originalMoveSpeed = PlayerStats.instance.moveSpeed;
            originalDashSpeed = PlayerStats.instance.dashSpeed;

            // Slow down the player by adjusting its move speed and dash speed
            PlayerController.instance.moveSpeed *= slowdownFactor;
            PlayerController.instance.activeMoveSpeed *= slowdownFactor;
            PlayerController.instance.dashSpeed *= slowdownFactor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController.instance.isSlowed = false;
            
            // Restore original move speed and dash speed
            PlayerController.instance.moveSpeed = originalMoveSpeed;
            PlayerController.instance.activeMoveSpeed = originalMoveSpeed;
            PlayerController.instance.dashSpeed = originalDashSpeed;
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(puddleDuration);

        // Get the SpriteRenderer component
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Fade out the alpha value over the specified duration
        float elapsedTime = 0f;
        Color startColor = spriteRenderer.color;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure alpha reaches 0
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // Destroy the object
        Destroy(gameObject);
    }
}
