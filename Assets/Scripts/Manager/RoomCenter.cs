using System.Collections.Generic;
using UnityEngine;

public class RoomCenter : MonoBehaviour
{
    public bool openWhenEnemiesCleared;
    public List<GameObject> enemies = new List<GameObject>();
    public Room theRoom;
    public GameObject gridOwner;
    public GameObject enemySpawner;

    // Start is called before the first frame update
    void Start()
    {
        if (gridOwner != null)
        {
            gridOwner.SetActive(false);
        }

        if(openWhenEnemiesCleared)
        {
            theRoom.closeWhenEntered = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (theRoom.enemyActive && gridOwner != null)
        {
            gridOwner.SetActive(true);
            enemySpawner.SetActive(true);
        }

        if (enemies.Count > 0 && theRoom.roomActive && openWhenEnemiesCleared)
        {
            for (int i = enemies.Count - 1; i >= 0; i--)  // Iterate backwards
            {
                GameObject enemy = enemies[i];
                if (enemy == null)
                {
                    enemies.RemoveAt(i);
                }
            }

            if (enemies.Count == 0)
            {
                theRoom.OpenDoors();

                gridOwner.SetActive(false);
                Destroy(gridOwner);
            }
        }
    }

     // New method to add enemy to the list
    public void AddEnemyToList(GameObject enemy)
    {
        enemies.Add(enemy);
    }
}
