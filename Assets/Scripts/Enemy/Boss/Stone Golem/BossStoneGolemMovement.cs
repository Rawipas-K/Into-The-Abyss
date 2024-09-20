using System.Collections.Generic;
using UnityEngine;

public class BossStoneGolemMovement : MonoBehaviour
{
    [Header("Boss Info")]
    public Animator anim;
    public Transform target; // Transform of the target the enemy chases
    public float moveSpeed = 3f; // Enemy movement speed
    public float shootBulletsCooldown = 2f;    
    public float shootLaserCooldown = 5f; // Interval between shoot laser animations (in seconds)

    private float shootBulletsTimer;
    private float shootLaserTimer;
    private bool isMoving = true;

    [Header("Pathfinding")]
    public Pathfinding2D pathfindingGrid; // Reference to the Pathfinding2D component    
    public float pathfindingInterval = 1f; // Time interval between pathfinding updates (in seconds)
    public float reachedThreshold = 0.1f; // Distance threshold for considering a node "reached"
    public float pathOffsetRange = 0.5f; // Range for random path offset (per axis)

    private List<Node2D> path;
    private float lastPathfindingTime = Mathf.NegativeInfinity;

    void Start()
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

        shootBulletsTimer = shootBulletsCooldown;
        shootLaserTimer = shootLaserCooldown;
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

        shootBulletsTimer -= Time.deltaTime;
        shootLaserTimer -= Time.deltaTime;
        
        // Check if it's time to shoot bullets
        if (shootBulletsTimer <= 0)
        {
            anim.SetTrigger("ShootBullet");
            shootBulletsTimer = shootBulletsCooldown;
            ResumeMovement();
        } 
        
        // Check if it's time to shoot laser
        if (shootLaserTimer <= 0)
        {
            anim.SetTrigger("CastLaser");
            shootLaserTimer = shootLaserCooldown;
            shootBulletsTimer = 5f;
            AudioManager.instance.PlaySFX("BossLaser");
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

        // Move the enemy towards the target node
        transform.Translate(movementDirection * moveSpeed * Time.deltaTime);

        // Check if enemy reached the target node
        if (Vector3.Distance(transform.position, targetNode.worldPosition) <= reachedThreshold)
        {
            path.RemoveAt(0); // Remove reached node from the path
        }
    }

    public void StopMovement()
    {
        isMoving = false;
    }

    public void ResumeMovement()
    {
        isMoving = true;
    }
}
