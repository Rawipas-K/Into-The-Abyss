using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class CharacterUnlockCage : MonoBehaviour
{
    private bool canUnlock;
    public GameObject message;
    public CharacterSelector charSelect;
    private CharacterSelector characterToUnlock;
    public SpriteRenderer cageSR;
    [SerializeField] private int floorToUnlock;
    private static string savePath;

    [Serializable]
    public class UnlockData
    {
        public List<string> unlockedCharacters = new List<string>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        if (PlayerStats.instance.currentFloor == floorToUnlock)
        {
            gameObject.SetActive(true);
        }
        savePath = Application.persistentDataPath + "/characterUnlockData.json";
        characterToUnlock = charSelect;
        cageSR.sprite = characterToUnlock.characterToSpawn.bodySR.sprite;
        LoadUnlockDataAndSetActive(); // Load unlock data when starting
    }

    // Update is called once per frame
    void Update()
    {
        if (canUnlock && Input.GetKeyDown(KeyCode.E))
        {
            UnlockCharacter(characterToUnlock.characterToSpawn.name);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canUnlock = true;
            message.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canUnlock = false;
            message.SetActive(false);
        }
    }

    void UnlockCharacter(string characterName)
    {
        UnlockData data = LoadUnlockData(); // Load existing data
        if (!data.unlockedCharacters.Contains(characterName)) // Check if character is not already unlocked
        {
            data.unlockedCharacters.Add(characterName); // Add character to unlocked list
            string jsonData = JsonUtility.ToJson(data);

            // Write data to the file
            File.WriteAllText(savePath, jsonData);

            Debug.Log("Character unlocked successfully.");
        }
        else
        {
            Debug.Log("Character is already unlocked.");
        }

        characterToUnlock.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public static UnlockData LoadUnlockData()
    {
        UnlockData data = new UnlockData();
        if (File.Exists(savePath))
        {
            // Read data from the file
            string jsonData = File.ReadAllText(savePath);

            data = JsonUtility.FromJson<UnlockData>(jsonData);
        }
        return data;
    }

    void LoadUnlockDataAndSetActive()
    {
        UnlockData data = LoadUnlockData();

        if (data.unlockedCharacters.Contains(characterToUnlock.characterToSpawn.name))
        {
            gameObject.SetActive(false); // Deactivate cage if already unlocked
            characterToUnlock.gameObject.SetActive(true); // Activate character select
        }
    }
}