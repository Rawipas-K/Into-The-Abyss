using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    private bool canSelect;
    public GameObject message;
    public PlayerController characterToSpawn;
    public bool shouldUnlock;

    // Start is called before the first frame update
    void Start()
    {
        LoadUnlockedCharacters();

        if (shouldUnlock)
        {
            if (IsCharacterUnlocked(characterToSpawn.name))
            {
                // Do not activate the game object here
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canSelect)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Vector3 characterPos = PlayerController.instance.transform.position;
                Destroy(PlayerController.instance.gameObject);

                PlayerController newCharacter = Instantiate(characterToSpawn, characterPos, characterToSpawn.transform.rotation);
                PlayerController.instance = newCharacter;

                gameObject.SetActive(false);

                CharacterSelectManager.instance.activePlayer = newCharacter;
                CharacterSelectManager.instance.activeCharacterSelect.gameObject.SetActive(true);
                CharacterSelectManager.instance.activeCharacterSelect = this;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canSelect = true;
            message.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canSelect = false;
            message.SetActive(false);
        }
    }

    void LoadUnlockedCharacters()
    {
        Debug.Log("Loading unlocked characters...");
        CharacterUnlockCage.UnlockData data = CharacterUnlockCage.LoadUnlockData();

        if (data != null)
        {
            if (data.unlockedCharacters.Contains(characterToSpawn.name))
            {
                UnlockCharacter(characterToSpawn.name);  // Call to activate or deactivate (see below)
                Debug.Log(characterToSpawn.name + " is unlocked.");
            }
            else
            {
                Debug.Log(characterToSpawn.name + " is locked.");
            }
        }
        else
        {
            Debug.Log("Unlock data is null.");
        }
    }
    
    bool IsCharacterUnlocked(string characterName)
    {
        CharacterUnlockCage.UnlockData data = CharacterUnlockCage.LoadUnlockData();
        return data != null && data.unlockedCharacters.Contains(characterName);
    }

    void UnlockCharacter(string characterName)
    {
        gameObject.SetActive(true); // Activate the game object when unlocking character
    }
}
