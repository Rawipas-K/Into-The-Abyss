using System.Collections.Generic;
using UnityEngine;

public class BossFireDemonMovement : MonoBehaviour
{
    [Header("Boss Info")]
    public Animator anim;
    public Transform target;
    public Transform stepEffectPoint;
    public GameObject stepEffect;   
    public float moveSpeed = 3f;
    public Transform fireWavePoint;
    public GameObject fireWaveEffect;
    public float attackCooldown;  
    
    private float attackTimer;
    private bool isMoving = true;

    [Header("Pathfinding")]
    public Pathfinding2D pathfindingGrid;    
    public float pathfindingInterval = 0.5f;
    public float reachedThreshold = 0.2f;
    public float pathOffsetRange = 0.3f;

    private List<Node2D> path;
    private float lastPathfindingTime = Mathf.NegativeInfinity;
    private Vector3 lastGridPosition;

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

        attackTimer = attackCooldown;
    }

    void Update()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
        else
        {
            target = null;
        }

        if (ShouldRecalculatePath() && pathfindingGrid != null)
        {
            FindPath();
        }

        if (isMoving && HasValidPath())
        {
            MoveTowardsTargetNode();
        }

        // Update attack timer
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            anim.SetTrigger("Attack");
            attackTimer = attackCooldown;
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
            lastPathfindingTime = Time.time;
        }
    }

    void MoveTowardsTargetNode()
    {
        if (!HasValidPath())
        {
            return;
        }

        // Get the current target node
        Node2D targetNode = path[0];

        // Calculate direction vector towards target node
        Vector3 movementDirection = (targetNode.worldPosition - transform.position).normalized;

        // Update enemy facing direction based on movement (left or right)
        if (target != null && target.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(0.8f, 0.8f, 1);
        }
        else
        {
            transform.localScale = new Vector3(-0.8f, 0.8f, 1);
        }

        transform.Translate(movementDirection * moveSpeed * Time.deltaTime);

        Vector3 currentGridPosition = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));

        if (Vector3.Distance(currentGridPosition, lastGridPosition) >= 1f)
        {
            if (stepEffect != null)
            {
                Instantiate(stepEffect, stepEffectPoint.position, Quaternion.identity);
            }
            lastGridPosition = currentGridPosition;
        }

        if (Vector3.Distance(transform.position, targetNode.worldPosition) <= reachedThreshold)
        {
            path.RemoveAt(0);
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

    private void FireWave()
    {
        AudioManager.instance.PlaySFX("BossAttack");
        if (target != null)
        {
            // Calculate the direction vector from fireWavePoint to target
            Vector3 direction = (target.position - fireWavePoint.position).normalized;

            // Instantiate the fireWaveEffect GameObject and get its Rigidbody2D component
            GameObject fireWave = Instantiate(fireWaveEffect, fireWavePoint.position, Quaternion.identity);
            Rigidbody2D rb = fireWave.GetComponent<Rigidbody2D>();

            // Set the velocity of the fire wave to move towards the player
            float fireWaveSpeed = 15f;
            rb.velocity = direction * fireWaveSpeed;

            // Check if the target is to the left of fireWavePoint
            if (target.position.x < fireWavePoint.position.x)
            {
                // Flip the fireWave horizontally
                fireWave.transform.localScale = new Vector3(-3, 3, 1);
            }
        }
    }
}
