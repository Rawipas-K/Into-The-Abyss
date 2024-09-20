using UnityEngine;

public class ActivateTrap : MonoBehaviour
{
    public bool isSpikeTrap, isFireTrap;
    public int damageAmount; // Amount of damage inflicted
    public float delayBeforeActivation = 0f; // Delay before activating the trap

    public SpriteRenderer theSR;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();        
        anim.enabled = false;

        Invoke("Activate", delayBeforeActivation);
    }

    void Activate()
    {
        anim.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats.instance.DamagePlayer(damageAmount);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats.instance.DamagePlayer(damageAmount);
        }
    }

    private void PlaySFX()
    {
        if (theSR.isVisible)
        {
            if (isSpikeTrap)
            {
                AudioManager.instance.PlaySFX("SpikeTrap");
            }
            if (isFireTrap)
            {
                AudioManager.instance.PlaySFX("FireTrap");
            }
        }
    }
}
