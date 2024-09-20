using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementCharge : MonoBehaviour
{
    [Header("Enemy Info")]
    public Transform target;
    public Animator anim;
    public float moveSpeed = 3f;

    [Header("Pathfinding")]
    public Pathfinding2D pathfindingGrid;
    public float pathfindingInterval = 0.1f; // Time interval between pathfinding updates (in seconds)
    public float reachedThreshold = 0.1f; // Distance threshold for considering a node "reached"
    public float pathOffsetRange = 2f; // Range for random path offset (per axis)

    private List<Node2D> path;
    private float lastPathfindingTime = Mathf.NegativeInfinity;

    [Header("Charge Attack")]
    public bool isHaveChargeAnimation;
    public float rangeForChargeAttack = 1f; // Distance for charge attack initiation
    public float chargeSpeed = 6f; // Faster speed for charge attack
    public float chargeDuration = 2f; // Time in seconds for the charge attack
    public float chargeDelay = 0.5f; // Delay before starting movement in charge attack
    public float chargeCooldown = 3f; // Time in seconds between charge attacks
    
    private bool isCharging = false;
    private float chargeDurationTimer;
    private float chargeTimer;
    private Vector3 chargeDirection; // Stores the initial charge direction

    private void Start()
    {
        if (pathfindingGrid == null)
        {
            Debug.LogError("Please assign the Pathfinding2D component in the inspector!");
            return;
        }

        chargeDurationTimer = chargeDuration;
        chargeTimer = chargeCooldown;

        if(pathfindingGrid != null)
        {
            FindPath();
        }
    }

    private void Update()
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

        // Decide between normal movement or charge attack based on conditions
        if (target != null)
        {
            chargeTimer -= Time.deltaTime;
            if (!isCharging && chargeTimer <= 0 && Vector3.Distance(transform.position, target.position) <= rangeForChargeAttack)
            {
                StartChargeAttack();
            }
            else if (isCharging) // Handle movement during charge attack
            {
                ChargeTowardPlayer();
            }
            else // Normal movement when not charging
            {
                if (ShouldRecalculatePath())
                {
                    FindPath();
                }

                if (HasValidPath())
                {
                    MoveTowardsTargetNode();
                }
            }
        }

        if (ShouldRecalculatePath())
        {
            FindPath();
        }

        if (HasValidPath())
        {
            if (!isCharging) // Check if not charging for normal movement
            {
                MoveTowardsTargetNode(); // Normal pathfinding movement
            }
            else // Handle charging movement
            {
                ChargeTowardPlayer();
            }
        }
    }

    private bool ShouldRecalculatePath()
    {
        return path == null || Time.time - lastPathfindingTime >= pathfindingInterval;
    }

    private bool HasValidPath()
    {
        return path != null && path.Count > 0;
    }

    private void FindPath()
    {
        if(target != null)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-pathOffsetRange, pathOffsetRange), 0f, Random.Range(-pathOffsetRange, pathOffsetRange));
            pathfindingGrid.FindPath(transform.position + randomOffset, target.position);
            path = pathfindingGrid.grid.path;
            lastPathfindingTime = Time.time; // Update last pathfinding time
        }
    }

    private void MoveTowardsTargetNode()
    {
        if (!HasValidPath())
        {
            return; // No path to follow
        }

        // Get the current target node
        Node2D targetNode = path[0];

        // Calculate direction vector towards target node
        Vector3 movementDirection = (targetNode.worldPosition - transform.position).normalized;

        // Check for nearby enemies (optional)
        CheckForNearbyEnemies(ref movementDirection);

        // Update enemy facing direction based on movement (left or right)
        float targetX = targetNode.worldPosition.x;

        if (targetX > transform.position.x)
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
    }

    private void CheckForNearbyEnemies(ref Vector3 movementDirection)
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, pathOffsetRange);
        foreach (Collider collider in nearbyColliders)
        {
            if (collider.gameObject != gameObject) // Skip self
            {
                movementDirection += (transform.position - collider.transform.position).normalized * 0.2f; // Small offset
                break; // Consider only the closest enemy for simplicity
            }
        }
    }

    private void StartChargeAttack()
    {
        isCharging = true;
        AudioManager.instance.PlaySFX("");
        StartCoroutine(ChargeDelay()); // Initiate charge delay coroutine
        chargeDirection = (target.position - transform.position).normalized; // Get initial charge direction
    }

    private IEnumerator ChargeDelay()
    {   
        yield return new WaitForSeconds(chargeDelay); // Wait for the delay
        if(isHaveChargeAnimation)
        {
            anim.SetTrigger("Charge");
        }
    }

    private void ChargeTowardPlayer()
    {
        chargeDurationTimer -= Time.deltaTime;
        transform.Translate(chargeDirection * chargeSpeed * Time.deltaTime);
        // Check if charge duration is over
        if (chargeDurationTimer <= 0)
        {
            isCharging = false; // Reset for potential next charge
            chargeDurationTimer = chargeDuration;
            chargeTimer = chargeCooldown;
        }

        // Check for collisions during charging
        RaycastHit hit;
        if (Physics.Raycast(transform.position, chargeDirection, out hit, chargeSpeed * Time.deltaTime))
        {
            // If the enemy is going to collide with an obstacle, adjust its position to avoid passing through walls
            if (hit.collider.CompareTag("Obstacle"))
            {
                transform.position = hit.point - chargeDirection * reachedThreshold;
                isCharging = false; // Stop charging upon collision with an obstacle
                chargeDurationTimer = chargeDuration;
                chargeTimer = chargeCooldown;
            }
        }

        // Check if reached player for attack execution (optional)
        if (Vector3.Distance(transform.position, target.position) <= reachedThreshold)
        {
            // Execute attack logic here
            isCharging = false; // Reset for potential next charge
            chargeDurationTimer = chargeDuration;
            chargeTimer = chargeCooldown;
        }
    }
}
