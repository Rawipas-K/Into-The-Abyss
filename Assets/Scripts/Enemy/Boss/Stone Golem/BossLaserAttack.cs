using UnityEngine;

public class BossLaserAttack : MonoBehaviour
{
    public float lineLength = 1f;
    public LayerMask obstacleLayer; // Layer containing obstacles 

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    private Transform player;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        edgeCollider = gameObject.GetComponent<EdgeCollider2D>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        Vector3 direction = player.position - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, lineLength, obstacleLayer);

        Vector3 endPoint;
        if (hit.collider != null)
        {
            endPoint = hit.point;
        }
        else
        {
            endPoint = transform.position + direction.normalized * lineLength;
        }

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);

        // Convert world space positions to local space
        Vector2 localStartPoint = transform.InverseTransformPoint(lineRenderer.GetPosition(0));
        Vector2 localEndPoint = transform.InverseTransformPoint(lineRenderer.GetPosition(1));

        // Update edge collider points with local space positions
        edgeCollider.points = new Vector2[] { localStartPoint, localEndPoint };
    }
}
