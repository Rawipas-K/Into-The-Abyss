using UnityEngine;

public class DamageNumberController : MonoBehaviour
{
    public static DamageNumberController instance;
    private void Awake()
    {
        instance = this;
    }

    public DamageNumber numberToSpawn;
    public Transform numberCanvas;

    public void SpawnDamage(float damageAmount, Vector3 location, bool isCritical)
    {
        int rounded = Mathf.RoundToInt(damageAmount);

        // Define a random offset range
        float offsetX = Random.Range(-0.5f, 0.5f);
        float offsetY = Random.Range(-0.2f, 0.2f);

        // Apply random offset to the original location
        Vector3 spawnPosition = location + new Vector3(offsetX, offsetY, 0f);

        DamageNumber newDamage = Instantiate(numberToSpawn, spawnPosition, Quaternion.identity, numberCanvas);
        if (isCritical)
        {
            newDamage.Setup(rounded, true);
        }
        else
        {
            newDamage.Setup(rounded, false);
        }
        
        newDamage.gameObject.SetActive(true);
    }
}
