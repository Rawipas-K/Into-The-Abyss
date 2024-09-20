using UnityEngine;
using System.Collections.Generic;

public class Pathfinding2D : MonoBehaviour
{

    public Transform seeker, target;
    [HideInInspector] public Grid2D grid;
    Node2D seekerNode, targetNode;
    public GameObject GridOwner;
    private GameObject enemyParent;
    public bool isBossRoom;

    void Start()
    {
        if (!isBossRoom)
        {
            // Get the parent of this enemy
            enemyParent = transform.parent.gameObject;
        }

        if (enemyParent != null)
        {
            // Find the Room Center GameObject
            GameObject roomCenter = enemyParent.transform.parent.gameObject;

            if (roomCenter != null)
            {
                // Find the GridOwner GameObject under the Room Center
                Transform gridOwnerTransform = roomCenter.transform.Find("Grid/GridOwner");

                if (gridOwnerTransform != null)
                {
                    GameObject GD = gridOwnerTransform.gameObject;

                    // Set the GridOwner variable in the Pathfinding2D script
                    GridOwner = GD;

                    grid = GridOwner.GetComponent<Grid2D>();
                }
            }
        }

        if(isBossRoom)
        {
            grid = GridOwner.GetComponent<Grid2D>();
        }
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
            target = null; // Clear target if player is not found
        }
    }


    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //get player and target position in grid coords
        seekerNode = grid.NodeFromWorldPoint(startPos);
        targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node2D> openSet = new List<Node2D>();
        HashSet<Node2D> closedSet = new HashSet<Node2D>();
        openSet.Add(seekerNode);
        
        //calculates path for pathfinding
        while (openSet.Count > 0)
        {

            //iterates through openSet and finds lowest FCost
            Node2D node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost <= node.FCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            //If target found, retrace path
            if (node == targetNode)
            {
                RetracePath(seekerNode, targetNode);
                return;
            }
            
            // Adds neighbor nodes to openSet
            foreach (Node2D neighbour in grid.GetNeighbors(node))
            {
                if (neighbour.obstacle || closedSet.Contains(neighbour))
                {
                    continue;
                }

                float newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    //reverses calculated path so first node is closest to seeker
    void RetracePath(Node2D startNode, Node2D endNode)
    {
        List<Node2D> path = new List<Node2D>();
        Node2D currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse(); 

        grid.path = path;

    }

    // Gets Euclidean distance between 2 nodes for calculating cost
    float GetDistance(Node2D nodeA, Node2D nodeB)
    {
        int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        // Euclidean distance calculation
        return Mathf.Sqrt(dstX * dstX + dstY * dstY);
    }
}