using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float waitToLoad = 2f;
    public string nextLevel;
    public bool isBossRoom;

    public bool isPaused;

    public Transform startPoint;

    public void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadPlayerData();

        if (PlayerStats.instance.currentFloor == 1)
        {
            StartCoroutine(PlayerStats.instance.StartTimer());
        }

        if (!isBossRoom)
        {
            if (PlayerStats.instance.currentFloor <= 6)
            {
                nextLevel = "Level 1";
            }
            else 
            {
                nextLevel = "Level 2";
            }

            if (PlayerStats.instance.currentFloor == 3 || PlayerStats.instance.currentFloor == 6 || 
            PlayerStats.instance.currentFloor == 9 || PlayerStats.instance.currentFloor == 12)
            {
                nextLevel = "Boss 1";
            }
        }
        else if (isBossRoom)
        {
            if (PlayerStats.instance.currentFloor == 3)
            {
                nextLevel = "Level 1";
            }
            else if (PlayerStats.instance.currentFloor == 6 || PlayerStats.instance.currentFloor == 9)
            {
                nextLevel = "Level 2";
            }
            else
            {
                nextLevel = "";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    public void LoadPlayerData()
    {
        //Position
        PlayerController.instance.transform.position = startPoint.position;
        PlayerController.instance.canMove = true;

        UIController.instance.UpdatePlayerUI("All");

        Time.timeScale = 1f; 
    }

    public IEnumerator LevelEnd(bool isExitToNextFloor)
    {
        AudioManager.instance.PlayChangeScene();

        PlayerController.instance.canMove = false;

        if(isExitToNextFloor)
        {
            UIController.instance.StartFadeToBlack(true);
        }
        else
        {
            UIController.instance.StartFadeToBlack(false);
        }

        yield return new WaitForSeconds(waitToLoad);

        SceneManager.LoadScene(nextLevel);
    }

    //pause game
    public void PauseUnpause()
    {
        if(!isPaused)
        {
            UIController.instance.pauseMenu.SetActive(true);

            isPaused = true;

            Time.timeScale = 0f;
        } else
        {
            UIController.instance.pauseMenu.SetActive(false);    

            isPaused = false;

            Time.timeScale = 1f;
        }
    }

    //pause when level up
    public void LevelUpPause()
    {
        UIController.instance.upgradeMenu.SetActive(true);

        UpgradeMenuController.instance.RandomUpgrade();
        Debug.Log("LevelUpPause work");
        isPaused = true;

        Time.timeScale = 0f;
    }

    public void LevelUpUnpause()
    {
        UIController.instance.upgradeMenu.SetActive(false);

        isPaused = false;

        Time.timeScale = 1f;

        PlayerStats.instance.LevelUp();
    }

    public void GetCoins(int amount)
    {
        PlayerStats.instance.currentCoins += amount;

        UIController.instance.UpdatePlayerUI("Coin");
    }

    public void SpendCoins(int amount)
    {
        PlayerStats.instance.currentCoins -= amount;

        if(PlayerStats.instance.currentCoins < 0)
        {
            PlayerStats.instance.currentCoins = 0;
        }

        UIController.instance.UpdatePlayerUI("Coin");
    }

    public void GetKey(int amount)
    {
        PlayerStats.instance.currentKey += amount;

        UIController.instance.UpdatePlayerUI("Key");
    }

    public void UseKey(int amount)
    {
        PlayerStats.instance.currentKey -= amount;

        if (PlayerStats.instance.currentKey < 0)
        {
            PlayerStats.instance.currentKey = 0;
        }

        UIController.instance.UpdatePlayerUI("Key");
    }

    public void GetScore(int amount)
    {
        PlayerStats.instance.currentScore += Mathf.RoundToInt(amount * PlayerStats.instance.currentScoreMultiplier);
        UIController.instance.UpdatePlayerUI("Score");
    }

    public void GetScoreMultiplier(float scoreMultiplier)
    {
        PlayerStats.instance.currentScoreMultiplier += scoreMultiplier;
        if (PlayerStats.instance.currentScoreMultiplier < 1)
        {
            PlayerStats.instance.currentScoreMultiplier = 1;
        } 
        UIController.instance.UpdatePlayerUI("Score");
    }
}