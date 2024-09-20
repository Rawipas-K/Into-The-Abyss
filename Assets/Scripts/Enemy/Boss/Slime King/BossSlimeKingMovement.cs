using System.Collections.Generic;
using UnityEngine;

public class BossSlimeKingMovement : MonoBehaviour
{
    [Header("Boss Info")]
    public Animator anim;
    public Transform target; // Transform of the target the enemy chases
    public float moveSpeed = 5f; // Enemy movement speed
    public float jumpCooldown = 5f; // Time interval between trigger calls (in seconds)
    public float boostedSpeed = 20f; // Speed when animation is triggered
    public GameObject slimePuddleEffect;  
      
    private float jumpTimer;
    private bool isAnimating = false; // Flag to indicate whether the boss is currently performing the animation

    [Header("Pathfinding")]
    public Pathfinding2D pathfindingGrid; // Reference to the Pathfinding2D component    
    public float pathfindingInterval = 1f; // Time interval between pathfinding updates (in seconds)
    public float reachedThreshold = 0.1f; // Distance threshold for considering a node "reached"
    public float pathOffsetRange = 0.5f; // Range for random path offset (per axis)

    private List<Node2D> path; // Stores the current path for the enemy (renamed for clarity)
    private float lastPathfindingTime = Mathf.NegativeInfinity; // Tracks the last time pathfinding was called

    
    void Start()
    {
        if (pathfindingGrid == null)
        {
            Debug.LogError("Pathfinding2D component not found!");
        }
        else
        {
            Debug.Log("GridOwner: " + pathfindingGrid.GridOwner); // Debug log to check GridOwner
            // Check if GridOwner is assigned before calling FindPath
            if (pathfindingGrid.GridOwner != null)
            {
                FindPath();
            }
            else
            {
                Debug.LogError("GridOwner not assigned in Pathfinding2D component!");
            }
        }

        jumpTimer = jumpCooldown;
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
        if (ShouldRecalculatePath())
        {
            FindPath();
        }

        if (HasValidPath())
        {
            MoveTowardsTargetNode();
        }
        
        jumpTimer -= Time.deltaTime;
        // Check if it's time to trigger animation
        if (jumpTimer <= 0)
        {
            anim.SetTrigger("JumUp");
            jumpTimer = jumpCooldown;
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
        if(target != null)
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

        if (target != null && target.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1); // Face right
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // Face left
        }

        // Adjust speed if currently animating
        float currentSpeed = isAnimating ? boostedSpeed : moveSpeed;

        // Move the enemy towards the target node
        transform.Translate(movementDirection * currentSpeed * Time.deltaTime);

        // Check if enemy reached the target node
        if (Vector3.Distance(transform.position, targetNode.worldPosition) <= reachedThreshold)
        {
            path.RemoveAt(0); // Remove reached node from the path
        }
    }

    // Method to be called when animation starts
    public void StartAnimation()
    {
        isAnimating = true;
    }

    // Method to be called when animation ends
    public void EndAnimation()
    {
        AudioManager.instance.PlaySFX("BossSlam");
        CameraShake.instance.ShakeCamera(5f, 5f, 1f);
        isAnimating = false;
        Instantiate(slimePuddleEffect, transform.position, Quaternion.identity);
    }
}