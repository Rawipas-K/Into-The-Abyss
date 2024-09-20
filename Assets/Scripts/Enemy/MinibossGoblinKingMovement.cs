using UnityEngine;

public class MinibossGoblinKingMovement : MonoBehaviour
{
    [Header("Enemy Info")]
    public Animator anim;
    public Transform target; // Transform of the player to lock onto
    public Transform firepoint;
    public GameObject throwingBag;
    public float throwingCooldown = 5f;
    private float throwingTimer;

    void Start()
    {
        throwingTimer = throwingCooldown;
    }

    void Update()
    {
        // Check for player existence before assigning target
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
            
            // Flip sprite based on target position
            if (target.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1); // Face right
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1); // Face left
            }
        }
        else
        {
            target = null; // Clear target if player is not found
        }

        throwingTimer -= Time.deltaTime;
        if  (throwingTimer <= 0)
        {
            anim.SetTrigger("Throw");
            throwingTimer = throwingCooldown;
        }
    }

    private void ThrowBag()
    {
        Instantiate(throwingBag, firepoint.position, Quaternion.identity);
    }
}
