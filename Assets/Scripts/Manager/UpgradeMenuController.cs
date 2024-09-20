using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeMenuController : MonoBehaviour
{
    public static UpgradeMenuController instance;
    public GameObject[] upgradeButtons; // Array of upgrade button prefabs
    public Transform[] positions; // Array to hold positions where upgrades can appear
    private List<Button> selectedUpgrades; // List to hold selected upgrade buttons
    private List<int> usedIndexes; // List to hold indexes of used upgrades

    void Awake()
    {
        instance = this;
        selectedUpgrades = new List<Button>();
        usedIndexes = new List<int>();
    }

    public void RandomUpgrade()
    {
        // Clear the previously selected upgrades and destroy their game objects
        foreach (Button upgrade in selectedUpgrades)
        {
            Destroy(upgrade.gameObject);
        }
        selectedUpgrades.Clear();
        usedIndexes.Clear();

        // Randomly select 3 unique upgrades to display or the total number of positions if it's less than 3
        int numberOfUpgrades = Mathf.Min(3, positions.Length);
        for (int i = 0; i < numberOfUpgrades; i++)
        {
            int randomIndex = GetUniqueRandomIndex();
            GameObject buttonGO = Instantiate(upgradeButtons[randomIndex], positions[i].position, Quaternion.identity, transform);
            Button button = buttonGO.GetComponent<Button>();
            selectedUpgrades.Add(button);
            usedIndexes.Add(randomIndex);
        }
    }

    // Function to get a unique random index
    int GetUniqueRandomIndex()
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, upgradeButtons.Length);
        } while (usedIndexes.Contains(randomIndex));
        return randomIndex;
    }
}
