using UnityEngine;

public class Node2D
{
    public float gCost; // Cost from the starting node to this node
    public float hCost; // Heuristic cost from this node to the target node
    public bool obstacle;
    public Vector3 worldPosition;

    public int GridX, GridY;
    public Node2D parent;


    public Node2D(bool _obstacle, Vector3 _worldPos, int _gridX, int _gridY)
    {
        obstacle = _obstacle;
        worldPosition = _worldPos;
        GridX = _gridX;
        GridY = _gridY;
    }

    public float FCost
    {
        get
        {
            return gCost + hCost;
        }

    }
    

    public void SetObstacle(bool isOb)
    {
        obstacle = isOb;
    }
}
