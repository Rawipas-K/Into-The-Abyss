using UnityEngine;

public class LevelExit : MonoBehaviour
{
    public bool isExitToNextFloor;
    public bool isExitFromCharacterSelect;
    public bool isWin;
    private int checkFloor;

    void Start()
    {
        checkFloor = PlayerStats.instance.currentFloor;
        if (checkFloor == 3 || checkFloor == 6 || checkFloor == 9 || checkFloor == 12)
        {
            isExitToNextFloor = false;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" & !PlayerStats.instance.isWin)
        {
            if(isExitToNextFloor && isExitFromCharacterSelect)
            {
                PlayerStats.instance.currentFloor += 1;
                StartCoroutine(LevelManager.instance.LevelEnd(true));
            }

            else if(isExitToNextFloor && !isExitFromCharacterSelect)
            {   
                LevelManager.instance.GetScore(PlayerStats.instance.currentFloor * 100);
                PlayerStats.instance.currentFloor += 1;
                StartCoroutine(LevelManager.instance.LevelEnd(true));
            }
            else if (isWin)
            {
                PlayerStats.instance.isWin = true;
                PlayerStats.instance.CalElapsedTime();
                PlayerDataManager.instance.GetPlayerData();
                UIController.instance.WhenPlayerVictory();
                Debug.Log("Win");
            }
            else
            {
                StartCoroutine(LevelManager.instance.LevelEnd(false));
            }
        }
    }
}
