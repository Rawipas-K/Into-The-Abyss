using UnityEngine;
using System.IO;
using System.Collections;
using System;

[Serializable]
public class PlayerData
{
    public string ID;
    public int highscore;
    public float bestTime;
}

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;
    private string saveFilePath;
    private bool isNewScoreOrTime;
    private PlayerData existingData;

    private void Awake()
    {
        instance = this;

        // Define the path to your JSON file
        saveFilePath = Application.persistentDataPath + "/playerData.json";

        // Check if the player data JSON file exists
        if (File.Exists(saveFilePath))
        {
            LoadPlayerData();
        }
        else
        {
            // If it doesn't exist, create a new PlayerData object with default values
            existingData = new PlayerData();
            existingData.ID = Guid.NewGuid().ToString();
            existingData.highscore = 0;
            existingData.bestTime = float.PositiveInfinity;
            
            // Save the new player data
            SavePlayerData(existingData);
        }
    }

    public void GetPlayerData()
    {
        PlayerData playerData = new PlayerData();
        playerData.highscore = PlayerStats.instance.currentScore;

        if (PlayerStats.instance.isWin)
        {
            playerData.bestTime = PlayerStats.instance.elapsedTime;
            StartCoroutine(CheckPlayerData(playerData));
        }
        else
        {
            StartCoroutine(CheckPlayerData(playerData));
        }   
    }

    public IEnumerator CheckPlayerData(PlayerData playerData)
    {
        // Load existing player data
        existingData = LoadPlayerData();

        // If no existing data, set bestTime to positive infinity
        if (existingData == null)
        {
            existingData = playerData;
            existingData.bestTime = float.PositiveInfinity; // Set bestTime to positive infinity
            isNewScoreOrTime = true;
            UIController.instance.ShowNewBest("highscore");
        }

        // If new highscore is higher, update highscore
        if (playerData.highscore > existingData.highscore)
        {
            existingData.highscore = playerData.highscore;
            isNewScoreOrTime = true;
            UIController.instance.ShowNewBest("highscore");
        }

        if (PlayerStats.instance.isWin)
        {
            // If new best time is lower, update best time
            if (playerData.bestTime < existingData.bestTime)
            {
                existingData.bestTime = playerData.bestTime;
                isNewScoreOrTime = true;
                UIController.instance.ShowNewBest("time");
            }
        }
        
        if (isNewScoreOrTime)
        {
            UIController.instance.ShowEnterName();
        }

        yield return null;
    }

    private void SavePlayerData(PlayerData playerData)
    {
        // Serialize the player data object to JSON format
        string jsonData = JsonUtility.ToJson(playerData);

        // Write the data to the file
        File.WriteAllText(saveFilePath, jsonData);
    }

    private PlayerData LoadPlayerData()
    {
        // Check if the file exists
        if (File.Exists(saveFilePath))
        {
            // Read the data from the file
            string jsonData = File.ReadAllText(saveFilePath);

            // Deserialize the JSON data back to PlayerData object
            return JsonUtility.FromJson<PlayerData>(jsonData);
        }
        else
        {
            Debug.LogWarning("Save file not found.");
            return null;
        }
    }

    public IEnumerator SavePlayerDataCoroutine()
    {
        // Save the existing player data
        SavePlayerData(existingData);

        // If the player wins, submit the data
        if (PlayerStats.instance.isWin)
        {
            yield return StartCoroutine(SubmitDataManager.instance.SubmitPlayerDataWin(existingData.ID, PlayerStats.instance.playerName, PlayerStats.instance.currentScore, PlayerStats.instance.elapsedTime, (success, message) =>
            {
                if (success)
                {
                    Debug.Log(message);
                    isNewScoreOrTime = false;
                    UIController.instance.SaveDataSuccessful(message);
                }
                else
                {
                    Debug.LogError(message);
                    isNewScoreOrTime = true;
                    // Show a message to the player indicating that their name already exists
                    UIController.instance.SaveDataFailed(message);
                }
            }));
        }
        else
        {
            yield return StartCoroutine(SubmitDataManager.instance.SubmitPlayerData(existingData.ID, PlayerStats.instance.playerName, PlayerStats.instance.currentScore, (success, message) =>
            {
                if (success)
                {
                    Debug.Log(message);
                    isNewScoreOrTime = false;
                    UIController.instance.SaveDataSuccessful(message);
                }
                else
                {
                    Debug.LogError(message);
                    isNewScoreOrTime = true;
                    // Show a message to the player indicating that their name already exists
                    UIController.instance.SaveDataFailed(message);
                }
            }));
        }
    }
}