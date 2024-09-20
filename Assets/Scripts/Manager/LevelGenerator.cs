using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    // Room prefabs and colors
    public GameObject layoutRoom;
    public Color startRoomColor, endRoomColor, shopRoomColor, chestRoomColor, miniBossRoomColor;

    // Generation parameters
    public int distanceToEnd;
    public bool includeShop, includeChestRoom, includeMiniBossRoom;
    public int minDistanceToShop, maxDistanceToShop;
    public int minDistanceToChestRoom, maxDistanceToChestRoom;
    public int minDistanceToMiniBossRoom, maxDistanceToMiniBossRoom;
    private int checkFloor;

    // Generation point and offsets
    public Transform generatorPoint;
    public float xOffset = 36f, yOffset = 26f;

    // Room detection
    public LayerMask whatIsRoom;

    // Room center prefabs
    public RoomCenter centerStart, centerEnd, centerShop, centerChestRoom, centerMiniBossRoom;
    public RoomCenter[] potentialCenters;
    private GameObject endRoom, shopRoom, chestRoom, miniBossRoom;

    // Direction enumeration
    public enum Direction { up, right, down, left };
    public Direction selectedDirection, firstSelectedDirection;

    // Room prefabs with exits
    public List<RoomPrefab> roomPrefabs = new List<RoomPrefab>();

    // Generated room outlines
    private List<GameObject> layoutRoomObjects = new List<GameObject>();
    private List<GameObject> generatedOutlines = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        checkFloor = PlayerStats.instance.currentFloor;
        if (checkFloor == 6 || checkFloor == 9 || checkFloor == 12)
        {
            includeMiniBossRoom = true;
        }
        else
        {
            includeMiniBossRoom = false;
        }

        InitializeStartRoom();
        GenerateLayout();
        GenerateSpecialRoomsLayout();
        CreateRoomOutlines();
    }

    // Update is called once per frame
    void Update()
    {
        ReloadScene();
    }

    // Initialize start room
    void InitializeStartRoom()
    {
        GameObject startRoom = Instantiate(layoutRoom, generatorPoint.position, generatorPoint.rotation);
        startRoom.GetComponent<SpriteRenderer>().color = startRoomColor;
    }

    void GenerateLayout()
    {
        selectedDirection = GetWeightedDirection(1f, 1f, 1f, 1f);
        firstSelectedDirection = selectedDirection;
        MoveGenerationPoint();

        // Generate rooms until reaching distanceToEnd
        for (int i = 0; i < distanceToEnd; i++)
        {
            GenerateRoomLayout();
        }
        SetFarthestRoom();
    }

    // Generate a room layout in the direction
    void GenerateRoomLayout()
    {
        GameObject newRoom = Instantiate(layoutRoom, generatorPoint.position, generatorPoint.rotation);
        layoutRoomObjects.Add(newRoom);

        SelectNextDirection();
        AvoidOverlap();
    }

    // Generate special rooms layout
    void GenerateSpecialRoomsLayout()
    {
        if (includeShop)
            GenerateShopRoomLayout();

        if (includeChestRoom)
            GenerateChestRoomLayout();

        if (includeMiniBossRoom)
            GenerateMiniBossRoomLayout();
    }

    // Select next direction based on first selected direction
    void SelectNextDirection()
    {
        switch (firstSelectedDirection)
        {
            case Direction.up:
                selectedDirection = GetWeightedDirection(0.5f, 0.3f, 0f, 0.3f);
                MoveGenerationPoint();
                break;
            case Direction.down:
                selectedDirection = GetWeightedDirection(0f, 0.3f, 0.5f, 0.3f);
                MoveGenerationPoint();
                break;
            case Direction.right:
                selectedDirection = GetWeightedDirection(0.3f, 0.5f, 0.3f, 0f);
                MoveGenerationPoint();
                break;
            case Direction.left:
                selectedDirection = GetWeightedDirection(0.3f, 0f, 0.3f, 0.5f);
                MoveGenerationPoint();
                break;
        }
    }

    // Find the farthest room using BFS
    GameObject FindFarthestRoom()
    {
        Queue<GameObject> queue = new Queue<GameObject>();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        // Enqueue the starting room
        queue.Enqueue(layoutRoomObjects[0]); // Assuming the first room is the starting room
        visited.Add(layoutRoomObjects[0]);

        GameObject farthestRoom = null;

        while (queue.Count > 0)
        {
            GameObject currentRoom = queue.Dequeue();
            farthestRoom = currentRoom; // Update farthest room at each step

            foreach (GameObject neighbor in GetAdjacentRooms(currentRoom))
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return farthestRoom;
    }

    // Get adjacent rooms of a given room
    List<GameObject> GetAdjacentRooms(GameObject room)
    {
        List<GameObject> adjacentRooms = new List<GameObject>();

        //iterate over the generated room objects and check if they are adjacent to the current room.
        foreach (GameObject otherRoom in layoutRoomObjects)
        {
            if (IsAdjacent(room, otherRoom))
            {
                adjacentRooms.Add(otherRoom);
            }
        }

        return adjacentRooms;
    }

    // Check if two rooms are adjacent
    bool IsAdjacent(GameObject room1, GameObject room2)
    {
        // Check if the rooms are within a certain distance threshold
        float distanceThreshold = 35f;
        return Vector3.Distance(room1.transform.position, room2.transform.position) < distanceThreshold;
    }

    // Call this method to find and set the farthest room
    void SetFarthestRoom()
    {
        GameObject farthestRoom = FindFarthestRoom();
        if (farthestRoom != null)
        {
            SetEndRoomLayout(farthestRoom, layoutRoomObjects.IndexOf(farthestRoom));
            Debug.LogError("farthestRoom = "+farthestRoom);
        }
        else
        {
            Debug.LogError("Farthest room not found.");
        }
    }

    // Set end room layout
    void SetEndRoomLayout(GameObject room, int endSelector)
    {
        endRoom = room;
        Debug.LogError("endSelector = "+endSelector);
        layoutRoomObjects.RemoveAt(endSelector);
        room.GetComponent<SpriteRenderer>().color = endRoomColor;
    }

    // Generate shop room layout
    void GenerateShopRoomLayout()
    {
        int shopSelector = Random.Range(minDistanceToShop, maxDistanceToShop + 1);
        shopRoom = layoutRoomObjects[shopSelector];
        layoutRoomObjects.RemoveAt(shopSelector);

        shopRoom.GetComponent<SpriteRenderer>().color = shopRoomColor;
    }

    // Generate chest room layout
    void GenerateChestRoomLayout()
    {
        int chestSelector = Random.Range(minDistanceToChestRoom, maxDistanceToChestRoom + 1);
        chestRoom = layoutRoomObjects[chestSelector];
        layoutRoomObjects.RemoveAt(chestSelector);

        chestRoom.GetComponent<SpriteRenderer>().color = chestRoomColor;
    }

    // Generate miniboss room layout
    void GenerateMiniBossRoomLayout()
    {
        int miniBossSelector = Random.Range(minDistanceToMiniBossRoom, maxDistanceToMiniBossRoom + 1);
        miniBossRoom = layoutRoomObjects[miniBossSelector];
        layoutRoomObjects.RemoveAt(miniBossSelector);

        miniBossRoom.GetComponent<SpriteRenderer>().color = miniBossRoomColor;
    }

    // Create room outlines for all generated rooms
    void CreateRoomOutlines()
    {
        CreateOutline(Vector3.zero);
        foreach (GameObject room in layoutRoomObjects)
        {
            CreateOutline(room.transform.position);
        }
        CreateOutline(endRoom.transform.position);

        if (includeShop)
            CreateOutline(shopRoom.transform.position);

        if (includeChestRoom)
            CreateOutline(chestRoom.transform.position);

        if (includeMiniBossRoom)
            CreateOutline(miniBossRoom.transform.position);

        GenerateRoomCenters();
    }

    // Generate room centers
    void GenerateRoomCenters()
    {
        foreach (GameObject outline in generatedOutlines)
        {
            bool generateCenter = true;

            if (outline.transform.position == Vector3.zero)
            {
                Instantiate(centerStart, outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();
                generateCenter = false;
            }

            if (outline.transform.position == endRoom.transform.position)
            {
                Instantiate(centerEnd, outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();
                generateCenter = false;
            }

            if (includeShop && outline.transform.position == shopRoom.transform.position)
            {
                Instantiate(centerShop, outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();
                generateCenter = false;
            }

            if (includeChestRoom && outline.transform.position == chestRoom.transform.position)
            {
                Instantiate(centerChestRoom, outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();
                generateCenter = false;
            }

            if (includeMiniBossRoom && outline.transform.position == miniBossRoom.transform.position)
            {
                Instantiate(centerMiniBossRoom, outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();
                generateCenter = false;
            }

            if (generateCenter)
            {
                int centerSelect = Random.Range(0, potentialCenters.Length);
                Instantiate(potentialCenters[centerSelect], outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();
            }
        }
    }

    // Adjusted direction probabilities based on weights
    private Direction GetWeightedDirection(float upWeight, float rightWeight, float downWeight, float leftWeight)
    {
        float totalWeight = upWeight + rightWeight + downWeight + leftWeight;
        float randomValue = Random.Range(0f, totalWeight);

        if (randomValue < upWeight)
            return Direction.up;
        else if (randomValue < upWeight + rightWeight)
            return Direction.right;
        else if (randomValue < upWeight + rightWeight + downWeight)
            return Direction.down;
        else
            return Direction.left;
    }

    // Move generation point based on selected direction
    public void MoveGenerationPoint()
    {
        switch (selectedDirection)
        {
            case Direction.up:
                generatorPoint.position += new Vector3(0f, yOffset, 0f);
                break;
            case Direction.down:
                generatorPoint.position += new Vector3(0f, -yOffset, 0f);
                break;
            case Direction.right:
                generatorPoint.position += new Vector3(xOffset, 0f, 0f);
                break;
            case Direction.left:
                generatorPoint.position += new Vector3(-xOffset, 0f, 0f);
                break;
        }
    }

    // Avoid overlapping generated rooms
    void AvoidOverlap()
    {
        while (Physics2D.OverlapCircle(generatorPoint.position, .2f, whatIsRoom))
        {
            MoveGenerationPoint();
        }
    }

    // Pick a room prefab based on exits
    public RoomPrefab PickRoom(bool exitUp, bool exitRight, bool exitDown, bool exitLeft)
    {
        foreach (RoomPrefab rp in roomPrefabs)
        {
            if (rp.exitUp == exitUp && rp.exitRight == exitRight && rp.exitDown == exitDown && rp.exitLeft == exitLeft)
            {
                return rp;
            }
        }
        return null;
    }

    // Create room outline for a given position
    public void CreateOutline(Vector3 position)
    {
        bool exitUp = Physics2D.OverlapCircle(position + new Vector3(0f, yOffset), .2f, whatIsRoom);
        bool exitDown = Physics2D.OverlapCircle(position + new Vector3(0f, -yOffset), .2f, whatIsRoom);
        bool exitRight = Physics2D.OverlapCircle(position + new Vector3(xOffset, 0f), .2f, whatIsRoom);
        bool exitLeft = Physics2D.OverlapCircle(position + new Vector3(-xOffset, 0f), .2f, whatIsRoom);

        var roomPrefab = PickRoom(exitUp, exitRight, exitDown, exitLeft);

        if (roomPrefab != null)
        {
            generatedOutlines.Add(Instantiate(roomPrefab.prefab, position, Quaternion.identity, transform));
        }
    }

    // Reload scene if 'V' is pressed (for debugging purposes)
    void ReloadScene()
    {
        #if UNITY_EDITOR
                if (Input.GetKey(KeyCode.V))
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        #endif
    }
}

[System.Serializable]
public class RoomPrefab
{
    public bool exitUp, exitRight, exitDown, exitLeft;
    public GameObject prefab;
}
