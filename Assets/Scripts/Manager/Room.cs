using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool closeWhenEntered /*, openWhenEnemiesCleared*/ ;

    public GameObject[] doors;

    //public List<GameObject> enemies = new List<GameObject>();

    [HideInInspector]
    public bool roomActive;
    public bool enemyActive;
    public bool trapActive;

    public GameObject mapHider;

    public void OpenDoors()
    {
        foreach(GameObject door in doors)
        {
            door.SetActive(false);

            closeWhenEntered = false;
        }

        LevelManager.instance.GetScore(10);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if(closeWhenEntered)
            {
                foreach(GameObject door in doors)
                {
                    door.SetActive(true);
                }
            }

            roomActive = true;

            enemyActive = true;

            mapHider.SetActive(false);

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            roomActive = false;

            enemyActive = false;
        }
    }
}
