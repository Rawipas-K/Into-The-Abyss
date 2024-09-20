using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Grid2D grid; // Reference to the Grid2D class
    [SerializeField] private Transform spawnPoint; // Transform for enemy spawn location
    [SerializeField] private Vector2 spawnRadius; // Radius for random spawn area
    [SerializeField] private float spawnInterval = 1.0f; // Time between enemy spawns
    [SerializeField] private int maxEnemies = 4; // Maximum number of enemies allowed to spawn
    [SerializeField] private EnemySpawnData[] enemySpawnData; // Array of data for each enemy type

    private float timer = 0f; // Timer for tracking spawn intervals
    private int enemiesSpawned = 0; // Track the number of enemies spawned

    private int[] cumulativeWeights; // Array to store cumulative weights for spawn chance calculation

    void Awake()
    {
        gameObject.SetActive(false); // Can be activated manually or through other scripts
        CalculateCumulativeWeights(); // Calculate cumulative weights for enemy spawn chance
    }

    private void Update()
    {
        timer += Time.deltaTime; // Increment timer based on frame time

        // Spawn enemy if enough time has elapsed and max enemies not reached
        if (timer >= spawnInterval && enemiesSpawned < maxEnemies)
        {
            SpawnEnemy();
            timer = 0f; // Reset timer after spawn
        }
    }

    // Calculate cumulative weights for spawn chance
    void CalculateCumulativeWeights()
    {
        cumulativeWeights = new int[enemySpawnData.Length];
        int totalWeight = 0;
        for (int i = 0; i < enemySpawnData.Length; i++)
        {
            totalWeight += enemySpawnData[i].spawnChance;
            cumulativeWeights[i] = totalWeight;
        }
    }

    void SpawnEnemy()
    {
        // Check if the maximum number of enemies has already been spawned
        if (enemiesSpawned >= maxEnemies)
        {
            return; // Exit the method if the maximum number of enemies has been reached
        }

        // Check if the current floor level is greater than or equal to the minimum required for each enemy in the array
        int currentFloorLevel = PlayerStats.instance.currentFloor;
        for (int i = 0; i < enemySpawnData.Length; i++)
        {
            // Check if an enemy has already been spawned
            if (enemiesSpawned >= maxEnemies)
            {
                break; // Exit the loop if the maximum number of enemies has been reached
            }

            if (currentFloorLevel >= enemySpawnData[i].minFloorLevel)
            {
                // Generate random offsets within the specified x and y spawn radii
                float randomXOffset = Random.Range(-spawnRadius.x, spawnRadius.x);
                float randomYOffset = Random.Range(-spawnRadius.y, spawnRadius.y);

                // Calculate spawn position using the generated offsets
                Vector3 spawnPosition = new Vector3(spawnPoint.position.x + randomXOffset, spawnPoint.position.y + randomYOffset, spawnPoint.position.z);

                // Convert spawn position to grid node
                Node2D spawnNode = grid.NodeFromWorldPoint(spawnPosition);

                // Check if the spawn node is walkable
                if (!spawnNode.obstacle)
                {
                    // Continue with enemy spawn if position is valid
                    // Generate a random value to determine enemy type based on spawn chances
                    int randomValue = Random.Range(0, cumulativeWeights[cumulativeWeights.Length - 1]);

                    // Determine the index of the chosen enemy based on the random value
                    int chosenEnemyIndex = 0;
                    while (chosenEnemyIndex < cumulativeWeights.Length && randomValue >= cumulativeWeights[chosenEnemyIndex])
                    {
                        chosenEnemyIndex++;
                    }

                    // Check if the chosen enemy meets the floor level requirement
                    if (currentFloorLevel >= enemySpawnData[chosenEnemyIndex].minFloorLevel)
                    {
                        // Spawn the chosen enemy prefab at the calculated spawn position
                        GameObject enemy = Instantiate(enemySpawnData[chosenEnemyIndex].enemyPrefab, spawnPosition, Quaternion.identity);
                        enemy.transform.parent = transform; // Make the spawned enemy a child of this object

                        // Call a method to notify RoomCenter about the spawned enemy (if available)
                        RoomCenter roomCenter = GetComponentInParent<RoomCenter>();
                        if (roomCenter != null)
                        {
                            roomCenter.AddEnemyToList(enemy);
                        }
                        enemiesSpawned++; // Increment the counter after spawning
                    }
                }
            }
        }
    }
}

// Serializable class to store enemy spawn data with spawn chance
[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab; // Prefab of the enemy
    [Range(0, 100)] public int spawnChance; // Spawn chance represented as a percentage
    public int minFloorLevel; // Minimum floor level required for the enemy to spawn
}
