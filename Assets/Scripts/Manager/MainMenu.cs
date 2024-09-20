using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public string levelToLoad;
    public GameObject startButton, optionButton, exitButton, leaderboardButton;
    public GameObject optionMenu, leaderboard, highscoreBoard, timeBoard;
    public Slider musicSlider, sfxSlider;
    public TextMeshProUGUI selfHighscore, selfBestTime;
    public Button scoreBoardButton, timeBoardButton;
    private bool isScoreBoardOpen, isTimeBoardOpen;
    private ColorBlock originalButtonColor;

    void Awake()
    {
        instance = this;
        originalButtonColor = scoreBoardButton.colors;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void OpenOptionMenu()
    {
        if(optionMenu.activeInHierarchy == true)
        {
            optionMenu.SetActive(false);
            startButton.SetActive(true);
            optionButton.SetActive(true);
            exitButton.SetActive(true);
            leaderboardButton.SetActive(true);
        } 
        else
        {
            optionMenu.SetActive(true);
            startButton.SetActive(false);
            optionButton.SetActive(false);
            exitButton.SetActive(false);
            leaderboardButton.SetActive(false);
        }
        AudioManager.instance.PlaySFX("ButtonClick");
    }

    public void OpenLeaderboard()
    {
        
        if(leaderboard.activeInHierarchy == true)
        {
            leaderboard.SetActive(false);
            startButton.SetActive(true);
            optionButton.SetActive(true);
            exitButton.SetActive(true);
            leaderboardButton.SetActive(true);
        } 
        else
        {   
            leaderboard.SetActive(true);
            startButton.SetActive(false);
            optionButton.SetActive(false);
            exitButton.SetActive(false);
            leaderboardButton.SetActive(false);

            if (highscoreBoard.activeInHierarchy == true)
            {
                isScoreBoardOpen = true;
                isTimeBoardOpen = false;
                scoreBoardButton.Select();
            }
            else if (timeBoard.activeInHierarchy == true)
            {
                isScoreBoardOpen = false;
                isTimeBoardOpen = true;
                timeBoardButton.Select();
            }

            ButtonColor();
            LeaderboardManager.instance.GetSelfData();
            selfHighscore.text = LeaderboardManager.instance.selfHighscore.ToString();
            selfBestTime.text = LeaderboardManager.instance.selfBestTime;
            LeaderboardManager.instance.GetDataForLeaderboards();
        }
        AudioManager.instance.PlaySFX("ButtonClick");
    }

    public void SwitchBoard(string board)
    {
        if (board == "Highscore")
        {
            highscoreBoard.SetActive(true);
            timeBoard.SetActive(false);
            isScoreBoardOpen = true;
            isTimeBoardOpen = false;
        }
        else if (board == "BestTime")
        {
            highscoreBoard.SetActive(false);
            timeBoard.SetActive(true);
            isTimeBoardOpen = true;
            isScoreBoardOpen = false;
        }

        ButtonColor();
        AudioManager.instance.PlaySFX("ButtonClick");
    }

    private void ButtonColor()
    {
        ColorBlock scoreButtonColors = scoreBoardButton.colors;
        ColorBlock timeButtonColors = timeBoardButton.colors;

        // Reset button colors to normal
        scoreButtonColors.normalColor = originalButtonColor.normalColor;
        timeButtonColors.normalColor = originalButtonColor.normalColor;

        if (isScoreBoardOpen)
        {
            // Set score button color to pressed color
            scoreButtonColors.normalColor = scoreButtonColors.pressedColor;
        }
        else if (isTimeBoardOpen)
        {
            // Set time button color to pressed color
            timeButtonColors.normalColor = timeButtonColors.pressedColor;
        }

        // Set the modified colors back to the buttons
        scoreBoardButton.colors = scoreButtonColors;
        timeBoardButton.colors = timeButtonColors;
    }

    public void ExitGame()
    {
         Application.Quit();
    }
}
