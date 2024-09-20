using UnityEngine;

public class LaserSpin : MonoBehaviour
{
    public enum RotationDirection { Clockwise, Counterclockwise }
    
    public float rotationSpeed = 90f;
    public float lineLength = 1f;
    public LayerMask obstacleLayer; // Layer containing obstacles 
    public RotationDirection rotationDirection = RotationDirection.Clockwise;

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    private float currentAngle = 0f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        edgeCollider = gameObject.GetComponent<EdgeCollider2D>();
    }

    void LateUpdate()
    {
        // Update current angle and direction
        if (rotationDirection == RotationDirection.Clockwise)
            currentAngle += rotationSpeed * Time.deltaTime;
        else
            currentAngle -= rotationSpeed * Time.deltaTime;
        
        currentAngle %= 360f;
        Vector3 localDirection = Quaternion.Euler(0f, 0f, currentAngle) * Vector2.right;

        // Set line renderer positions relative to parent's position
        Vector3 startPos = transform.parent.position;
        lineRenderer.SetPosition(0, startPos);
        
        // Check for collisions with obstacles
        RaycastHit2D hit = Physics2D.Raycast(startPos, localDirection.normalized, lineLength, obstacleLayer);

        Vector3 endPos;
        if (hit.collider != null)
        {
            // Line collided with an obstacle!
            endPos = startPos + ((Vector3)hit.point - startPos);
        }
        else
        {
            // No collision, use full laser length
            endPos = startPos + localDirection.normalized * lineLength;
        }

        lineRenderer.SetPosition(1, endPos);

        // Update edge collider points with calculated end point
        edgeCollider.points = new Vector2[] { Vector2.zero, endPos - startPos };

    }
}