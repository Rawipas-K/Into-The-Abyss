using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{   
    public static UIController instance;

    [Header("Player UI")]
    public Image currentGun;
    public TextMeshProUGUI gunText, ammoText;
    public Slider healthSlider, levelSlider;
    public TextMeshProUGUI healthText, levelText, coinText, keyText, scoreText, scoreMultiplierText;
    

    [Header("Loading Black Screen")]
    public Image fadeScreen;
    public float fadeSpeed;
    private bool fadeToBlack, fadeOutBlack;
    public GameObject floorScene;
    public TextMeshProUGUI floorText;
    private bool showFloor;

    [Header("End Screen")]
    public TMP_InputField playerNameInput; 
    public GameObject endScreen, deathText, winText, showTime, enterPlayerName, loading;
    public TextMeshProUGUI scorePointEnd, timeEnd;
    public TextMeshProUGUI newHighscoreText, newBestTimeText;
    public TextMeshProUGUI saveDataMessage;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public Slider musicSlider, sfxSlider;

    [Header("Tab UI")]
    public GameObject bigMapScreen; 
    public TextMeshProUGUI statMoveSpeed, statDMG, statCrit, statCritDMG, statShootTime, statReloadTime, statBulletSpeed, statBulletLifeTime;
    public TextMeshProUGUI floor;

    [Header("Upgrade Menu")]
    public GameObject upgradeMenu;

    [Header("Other")]
    public Slider bossHealthBar;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeOutBlack = true;
        fadeToBlack = false;

        currentGun.sprite = PlayerInventory.instance.availableGuns[PlayerInventory.instance.currentGun].gunUI;
        gunText.text = PlayerInventory.instance.availableGuns[PlayerInventory.instance.currentGun].weaponName;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            OpenMap();
        }
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            bigMapScreen.SetActive(false);
        }

        if(fadeOutBlack)
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));
            
            if(fadeScreen.color.a == 0f)
            {
                fadeOutBlack = false;

                if(!showFloor)  
                {
                    floorScene.SetActive(false);
                    Debug.Log("Disable Show Floor");
                }
            }
        }

        if(fadeToBlack)
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
            
            if(fadeScreen.color.a == 1f)
            {
                fadeToBlack = false;
                Debug.Log("showFloor = " + showFloor);

                if(showFloor)
                {
                    floorText.text = PlayerStats.instance.currentFloor.ToString();
                    floorScene.SetActive(true);
                    Debug.Log("showFloor Work");
                }
            }
        }
    }

    public void StartFadeToBlack(bool isExitToNextFloor)
    {
        if(isExitToNextFloor) 
        {
            showFloor = true;
        }
        else
        {
            showFloor = false;
        }

        fadeToBlack = true;
        fadeOutBlack = false;
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Character Select");

        Destroy(PlayerController.instance.gameObject);

        AudioManager.instance.PlaySFX("ButtonClick");
    }

    public void ReturnToMainMenu()
    {        
        Time.timeScale = 1f;

        //PlayerController.instance.gameObject.SetActive(false);

        SceneManager.LoadScene("Title menu");
        Destroy(PlayerController.instance.gameObject);
        Destroy(CameraController.instance.gameObject);
        AudioManager.instance.PlaySFX("ButtonClick");
    }

    public void Resume()
    {
        LevelManager.instance.PauseUnpause();
        AudioManager.instance.PlaySFX("ButtonClick");
    }

    public void OpenMap()
    {
        UpdateStatUI();
        bigMapScreen.SetActive(true);
    }

    public void UpdatePlayerUI(string update)
    {
        switch (update)
        {
            case "HP":
                healthSlider.maxValue = PlayerStats.instance.maxHealth;
                healthSlider.value = PlayerStats.instance.currentHealth;
                healthText.text = PlayerStats.instance.currentHealth.ToString() + " / " + PlayerStats.instance.maxHealth.ToString();
                break;
            case "Level":
                levelSlider.maxValue = PlayerStats.instance.maxExp;
                levelSlider.value = PlayerStats.instance.currentExp;
                levelText.text = "Lv." + PlayerStats.instance.currentLevel.ToString();
                break;
            case "Coin":
                coinText.text = PlayerStats.instance.currentCoins.ToString();
                break;
            case "Key":
                keyText.text = PlayerStats.instance.currentKey.ToString();
                break;
            case "Score":
                scoreText.text =PlayerStats.instance. currentScore.ToString();
                scoreMultiplierText.text = PlayerStats.instance.currentScoreMultiplier.ToString("F2");
                break;
            // For when enter new floor
            case "All":
                healthSlider.maxValue = PlayerStats.instance.maxHealth;
                healthSlider.value = PlayerStats.instance.currentHealth;
                healthText.text = PlayerStats.instance.currentHealth.ToString() + " / " + PlayerStats.instance.maxHealth.ToString();

                levelSlider.maxValue = PlayerStats.instance.maxExp;
                levelSlider.value = PlayerStats.instance.currentExp;
                levelText.text = "Lv." + PlayerStats.instance.currentLevel.ToString();

                coinText.text = PlayerStats.instance.currentCoins.ToString();

                keyText.text = PlayerStats.instance.currentKey.ToString();

                scoreText.text = PlayerStats.instance. currentScore.ToString();
                PlayerStats.instance.currentScoreMultiplier = 1;
                scoreMultiplierText.text = PlayerStats.instance.currentScoreMultiplier.ToString("F2");
                break;
        }
    }

    public void UpdateStatUI()
    {
        Gun currentGun = PlayerInventory.instance.availableGuns[PlayerInventory.instance.currentGun];
        statMoveSpeed.text = "Move Speed : " + PlayerController.instance.activeMoveSpeed.ToString();
        statDMG.text = "DMG : " + currentGun.damage.ToString();
        statCrit.text = "Critical Chance : " + currentGun.criticalChance.ToString() + " %";
        statCritDMG.text = "Critical DMG : x " + currentGun.criticalMultiplier.ToString();
        statShootTime.text = "Shoot Time : " + currentGun.shootInterval.ToString() + " Per second";
        statReloadTime.text = "Reload Time : " + currentGun.reloadTime.ToString() + " s"; 
        statBulletSpeed.text = "Bullet Speed : " + currentGun.bulletSpeed.ToString(); 
        statBulletLifeTime.text = "Bullet Life Time : " + currentGun.bulletLifeTime.ToString() + " s";

        floor.text = "Floor " + PlayerStats.instance.currentFloor.ToString();
    }

    public void OnUpgradeClicked(string selectedUpgrade)
    {
        Debug.Log("" + selectedUpgrade);
        UpgradeStat.instance.UpgradeCharacter(selectedUpgrade);
        AudioManager.instance.PlaySFX("ButtonClick");
        LevelManager.instance.LevelUpUnpause();
    }

    public void WhenPlayerDeath()
    {
        Time.timeScale = 0f;
        PlayerController.instance.gameObject.SetActive(false);
        deathText.SetActive(true);
        scorePointEnd.text = PlayerStats.instance.currentScore.ToString();
        endScreen.SetActive(true);
        AudioManager.instance.PlayGameOver();
    }

    public void WhenPlayerVictory()
    {
        Time.timeScale = 0f;
        PlayerController.instance.gameObject.SetActive(false);
        winText.SetActive(true);
        scorePointEnd.text = PlayerStats.instance.currentScore.ToString();
        timeEnd.text = PlayerStats.instance.timeText;
        showTime.SetActive(true);
        endScreen.SetActive(true);
        AudioManager.instance.PlayGameWin();
    }

    public void ShowEnterName()
    {
        enterPlayerName.SetActive(true);
    }

    public void HideEnterName()
    {
        enterPlayerName.SetActive(false);
    }

    public void ShowNewBest(string show)
    {
        switch(show)
        {
            case "highscore":
                newHighscoreText.gameObject.SetActive(true);
                break;
            case "time":
                newBestTimeText.gameObject.SetActive(true);
                break;
        }
    }    

    public void SaveData()
    {
        loading.SetActive(true);
        StartCoroutine(PlayerDataManager.instance.SavePlayerDataCoroutine());
        AudioManager.instance.PlaySFX("ButtonClick");
    }

    public void SaveDataSuccessful(string message)
    {
        loading.SetActive(false);
        HideEnterName();
        saveDataMessage.color = Color.green;
        saveDataMessage.text = message;
    }

    public void SaveDataFailed(string message)
    {
        loading.SetActive(false);
        saveDataMessage.color = Color.red;
        saveDataMessage.text = message;
    }
    
    public void GetPlayerName()
    {
        PlayerStats.instance.playerName = playerNameInput.text;
    }
}
