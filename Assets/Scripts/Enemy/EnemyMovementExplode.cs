using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementExplode : MonoBehaviour
{
    [Header("Enemy Info")]
    public Transform target;
    public Animator anim; 
    public float moveSpeed = 3f;
    public float explodeRange = 1.5f;
    public GameObject explodeEffect;

    [Header("Pathfinding")]
    public Pathfinding2D pathfindingGrid;
    public float pathfindingInterval = 0.1f; // Time interval between pathfinding updates (in seconds)
    public float reachedThreshold = 0.1f; // Distance threshold for considering a node "reached"
    public float pathOffsetRange = 2f; // Range for random path offset (per axis)

    private List<Node2D> path;
    private float lastPathfindingTime = Mathf.NegativeInfinity;
    

    private void Start()
    {
        if (pathfindingGrid == null)
        {
            Debug.LogError("Please assign the Pathfinding2D component in the inspector!");
            return;
        }

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
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

            // Check if the enemy is near the player
            if (distanceToPlayer <= explodeRange)
            {
                // Trigger the explode animation
                anim.SetTrigger("Explode");
            }
            else
            {
                if (ShouldRecalculatePath())
                {
                    FindPath();
                }

                if (HasValidPath())
                {
                    MoveTowardsTargetNode(); // Normal pathfinding movement
                }
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

    private void Explode()
    {
        GameObject explosion = Instantiate(explodeEffect, transform.position, Quaternion.identity);
        
        // Get the existing scale of the explosion GameObject
        Vector3 currentScale = explosion.transform.localScale;

        // Modify only the x and y scales, keeping the z scale unchanged
        explosion.transform.localScale = new Vector3(2f * currentScale.x, 2f * currentScale.y, currentScale.z);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}