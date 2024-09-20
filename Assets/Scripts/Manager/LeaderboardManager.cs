using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager instance;

    public int selfHighscore;
    public string selfBestTime;
    public GameObject Loading;

    // First leaderboard UI elements
    public RectTransform highscoreEntryTemplate;
    public RectTransform highscoreContainer;

    // Second leaderboard UI elements
    public RectTransform timeEntryTemplate;
    public RectTransform timeContainer;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Loading.SetActive(true);
    }

    public void GetDataForLeaderboards()
    {
        StartCoroutine(FetchLeaderboardData("https://wcnzfndf0f.execute-api.ap-southeast-1.amazonaws.com/leaderboard/best_score", highscoreEntryTemplate, highscoreContainer));
        StartCoroutine(FetchLeaderboardData("https://wcnzfndf0f.execute-api.ap-southeast-1.amazonaws.com/leaderboard/best_time", timeEntryTemplate, timeContainer));
    }

    private IEnumerator FetchLeaderboardData(string url, RectTransform entryTemplate, RectTransform container)
    {
        Loading.SetActive(true);
        // Fetch data from the server
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = www.downloadHandler.text;
                Debug.Log("json = " + jsonResult);

                // Parse JSON data
                List<LeaderboardEntry> leaderboardEntries = ParseLeaderboardJson(jsonResult);

                // Update UI
                if (container == highscoreContainer)
                    UpdateLeaderboardUI(leaderboardEntries, entryTemplate, container, "highscore");
                else if (container == timeContainer)
                    UpdateLeaderboardUI(leaderboardEntries, entryTemplate, container, "time");
            }
            else
            {
                Debug.LogError("Error fetching leaderboard: " + www.error);
            }
        }
    }

    private List<LeaderboardEntry> ParseLeaderboardJson(string json)
    {
        // Use a wrapper class to parse the JSON array
        LeaderboardEntriesWrapper wrapper = JsonUtility.FromJson<LeaderboardEntriesWrapper>("{\"entries\":" + json + "}");
        return wrapper.entries;
    }

    private void UpdateLeaderboardUI(List<LeaderboardEntry> leaderboardEntries, RectTransform entryTemplate, RectTransform container, string leaderboardType)
    {
        // Destroy existing leaderboard entries except for the template
        DestroyLeaderboardEntries(container, entryTemplate);

        // Display leaderboard with ranks, player names, and player scores
        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            LeaderboardEntry entry = leaderboardEntries[i];
            CreateLeaderboardEntryTransform(entry, i + 1, entryTemplate, container, leaderboardType);
        }

        Loading.SetActive(false);
    }

    private void CreateLeaderboardEntryTransform(LeaderboardEntry entry, int rank, RectTransform entryTemplate, RectTransform container, string leaderboardType)
    {
        RectTransform entryTransform = Instantiate(entryTemplate, container);
        entryTransform.gameObject.SetActive(true);

        entryTransform.Find("RankText").GetComponent<TextMeshProUGUI>().text = rank.ToString();
        entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().text = entry.playerName;

        if (leaderboardType == "highscore")
        {
            entryTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = entry.playerScore.ToString();
        }
        else if (leaderboardType == "time")
        {
            // Format time and set the text
            string formattedTime = FormatTime(entry.playerTime);
            entryTransform.Find("TimeText").GetComponent<TextMeshProUGUI>().text = formattedTime;
        }

        if (rank == 1)
        {
            if (leaderboardType == "highscore")
            {
                entryTransform.Find("RankText").GetComponent<TextMeshProUGUI>().color = Color.yellow;
                entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().color = Color.yellow;
                entryTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>().color = Color.yellow;
            }
            else if (leaderboardType == "time")
            {
                entryTransform.Find("RankText").GetComponent<TextMeshProUGUI>().color = Color.yellow;
                entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().color = Color.yellow;
                entryTransform.Find("TimeText").GetComponent<TextMeshProUGUI>().color = Color.yellow;
            }
        }
    }

    private void DestroyLeaderboardEntries(RectTransform container, RectTransform entryTemplate)
    {
        foreach (Transform child in container)
        {
            // Skip destroying the template
            if (child == entryTemplate)
                continue;

            Destroy(child.gameObject);
        }
    }

    private string FormatTime(float totalTime)
    {
        if (float.IsInfinity(totalTime) || totalTime == 0F)
        {
            return "N/A";
        }
        else
        {
            int minutes = Mathf.FloorToInt(totalTime / 60f);
            int seconds = Mathf.FloorToInt(totalTime % 60f);
            int milliseconds = Mathf.FloorToInt((totalTime * 1000f) % 1000f);

            return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }

    public void GetSelfData()
    {
        string saveFilePath = Application.persistentDataPath + "/playerData.json";

        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);

            PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);

            // Access the highscore and bestTime fields
            selfHighscore = playerData.highscore;
            selfBestTime = FormatTime(playerData.bestTime);
        }
        else
        {
            Debug.LogError("JSON file not found at path: " + saveFilePath);
        }

        Debug.Log("GetSelfData Work");
    }
}

[System.Serializable]
public class SelfPlayerData
{
    public int highscore;
    public float bestTime;
}

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int playerScore;
    public float playerTime;
}

[System.Serializable]
public class LeaderboardEntriesWrapper
{
    public List<LeaderboardEntry> entries;
}
