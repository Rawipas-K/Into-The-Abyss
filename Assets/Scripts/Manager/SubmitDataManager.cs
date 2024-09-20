using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class SubmitDataManager : MonoBehaviour
{
    public static SubmitDataManager instance;

    void Awake()
    {
        instance = this;
    }

    // Method to submit player data to the server
    public IEnumerator SubmitPlayerData(string playerID, string playerName, int playerScore, Action<bool, string> callback)
    {
        // Prepare JSON data to send
        string jsonData = "{\"playerID\":\"" + playerID + "\", \"playerName\":\"" + playerName + "\", \"playerScore\":" + playerScore + "}";
        Debug.Log("jsonData = " + jsonData);
        yield return StartCoroutine(PostPlayerData(jsonData, (success, errorMessage) =>
        {
            if (success)
            {
                callback(true, "Score submitted successfully!");
            }
            else
            {
                callback(false, errorMessage);
            }
        }));
    }

    // Method to submit player data to the server
    public IEnumerator SubmitPlayerDataWin(string playerID, string playerName, int playerScore, float playerTime, Action<bool, string> callback)
    {
        // Prepare JSON data to send
        string jsonData = "{\"playerID\":\"" + playerID + "\", \"playerName\":\"" + playerName + "\", \"playerScore\":" + playerScore + ", \"playerTime\":" + playerTime + "}";
        Debug.Log("jsonData = " + jsonData);
        yield return StartCoroutine(PostPlayerData(jsonData, (success, errorMessage) =>
        {
            if (success)
            {
                callback(true, "Score submitted successfully!");
            }
            else
            {
                callback(false, errorMessage);
            }
        }));
    }

    private IEnumerator PostPlayerData(string jsonData, Action<bool, string> callback)
    {
        using (UnityWebRequest www = new UnityWebRequest("https://wcnzfndf0f.execute-api.ap-southeast-1.amazonaws.com/submit_playerdata", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                callback(true, "Score submitted successfully!");
            }
            else
            {
                //Parse the response JSON
                string responseJson = www.downloadHandler.text;

                // Deserialize JSON response
                ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseJson);

                // Check if the response contains an error or message field
                if (!string.IsNullOrEmpty(responseData.error))
                {
                    callback(false, responseData.error);
                }
                else if (!string.IsNullOrEmpty(responseData.message))
                {
                    callback(true, responseData.message);
                }
                else
                {
                    callback(false, "Unknown error occurred.");
                }
            }
        }
    }
}

// Define a class to represent the structure of the response JSON
[Serializable]
public class ResponseData
{
    public string error;
    public string message;
}