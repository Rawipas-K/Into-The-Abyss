using System.Collections.Generic;
using UnityEngine;

public class BossDeathBringerMovement : MonoBehaviour
{
    [Header("Boss Info")]
    public Animator anim;
    public Transform target; // Transform of the target the enemy chases
    public float moveSpeed = 6f; // Enemy movement speed
    public float attackRange; // Range for boss to melee attack 
    public GameObject darkSpellEffect;
    public float spellCooldown; // Interval between spell casts (in seconds)
    public float gravityPullCooldown;
    public float pullRange; // Range within which the pull is activated 
    public float pullForceMax; // Strength of the pull force
    public float pullDuration;
   
    private float spellTimer, gravityPullTimer, pullForce, pullDurationTimer;
    private int spellHit, spellHitCount;
    private bool isMoving = true;
    
    [Header("Pathfinding")]
    public Pathfinding2D pathfindingGrid; // Reference to the Pathfinding2D component
    public float pathfindingInterval = 0.5f; // Time interval between pathfinding updates (in seconds)
    public float reachedThreshold = 0.2f; // Distance threshold for considering a node "reached"
    public float pathOffsetRange = 0.3f; // Range for random path offset (per axis) 

    private List<Node2D> path; // Stores the current path for the enemy (renamed for clarity)
    private float lastPathfindingTime = Mathf.NegativeInfinity; // Tracks the last time pathfinding was called

    void Start()
    {
        if (pathfindingGrid == null)
        {
            Debug.LogError("Please assign the Pathfinding2D component in the inspector!");
            return;
        }
        if (pathfindingGrid != null)
        {
            FindPath();
        }

        spellTimer = spellCooldown;
        gravityPullTimer = gravityPullCooldown;
        pullDurationTimer = pullDuration;
    }

    void Update()
    {
        // Check for player existence before assigning target
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
        else
        {
            target = null; // Clear target if player is not found
        }

        // Check if path needs recalculating
        if (ShouldRecalculatePath() && pathfindingGrid != null)
        {
            FindPath();
        }

        if (isMoving && HasValidPath())
        {
            MoveTowardsTargetNode();
        }

        spellTimer -= Time.deltaTime;
        gravityPullTimer -= Time.deltaTime;

        if (spellHitCount <= 0)
        {
            spellHit = Random.Range(3, 6);
            spellHitCount = spellHit;
        }

        if (spellTimer <= 0)
        {
            anim.SetTrigger("Cast");
            AudioManager.instance.PlaySFX("BossCastSpell");
            spellTimer = spellCooldown;
        }

        // Check if the target (player) is outside the pull range and the pull has not been applied yet
        if (gravityPullTimer <= 0 && target != null && Vector2.Distance(transform.position, target.position) > pullRange)
        {
            spellTimer = spellCooldown;
            //StopMovement();
            anim.SetBool("GravityPull", true);
            // Pull the player towards the boss
            PullPlayer(target.GetComponent<Rigidbody2D>());
            pullDurationTimer -= Time.deltaTime;

            if (Vector2.Distance(transform.position, target.position) < 2f || pullDurationTimer<= 0)
            {
                anim.SetBool("GravityPull", false);
                gravityPullTimer = gravityPullCooldown;
                pullDurationTimer = pullDuration;
                pullForce = 0;
                anim.SetTrigger("Attack");
                AudioManager.instance.PlaySFX("BossAttack");
            }
        }
    }

    bool ShouldRecalculatePath()
    {
        return path == null || Time.time - lastPathfindingTime >= pathfindingInterval;
    }

    bool HasValidPath()
    {
        return path != null && path.Count > 0;
    }

    void FindPath()
    {
        if (target != null)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-pathOffsetRange, pathOffsetRange), 0f, Random.Range(-pathOffsetRange, pathOffsetRange));
            pathfindingGrid.FindPath(transform.position + randomOffset, target.position);
            path = pathfindingGrid.grid.path;
            lastPathfindingTime = Time.time; // Update last pathfinding time
        }
    }

    void MoveTowardsTargetNode()
    {
        if (!HasValidPath())
        {
            return; // No path to follow
        }

        // Get the current target node
        Node2D targetNode = path[0];

        // Calculate direction vector towards target node
        Vector3 movementDirection = (targetNode.worldPosition - transform.position).normalized;

        // Update enemy facing direction based on movement (left or right)

        if (target != null & target.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1); // Face right
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // Face left
        }

        // Move the enemy towards the target node
        transform.Translate(movementDirection * moveSpeed * Time.deltaTime);

        // Check if enemy reached the target node
        if (Vector3.Distance(transform.position, targetNode.worldPosition) <= reachedThreshold)
        {
            path.RemoveAt(0); // Remove reached node from the path
        }

        // Check the distance between boss and target for initiating an attack
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= attackRange)
            {
                // Play attack animation
                anim.SetTrigger("Attack");
                AudioManager.instance.PlaySFX("BossAttack");
            }
        }
    }

    private void StopMovement()
    {
        isMoving = false;
    }

    private void ResumeMovement()
    {
        isMoving = true;
    }
    
    private void DarkSpell()
    {
        // Define the offset above the player's head
        Vector3 spellOffset = new Vector3(0f, 2f, 0f); // Adjust the y-offset as needed

        // Instantiate the darkSpellEffect above the player's head
        Instantiate(darkSpellEffect, target.position + spellOffset, Quaternion.identity);

        spellHitCount--;

        if (spellHitCount > 0)
        {
            spellTimer = 0;
        }
    }

    private void PullPlayer(Rigidbody2D playerRB)
    {
        // Calculate direction from player to boss
        Vector2 pullDirection = ((Vector2)transform.position - playerRB.position).normalized;

        if (pullForce < pullForceMax)
        {
            pullForce += 20 * Time.deltaTime;
        }
        // Apply the pull force to the player
        playerRB.AddForce(pullDirection * pullForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            PlayerStats.instance.DamagePlayer(2);
        }
    }
}
