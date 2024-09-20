using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public TMP_Text damageText;

    public float lifeTime;
    private float lifeCounter;

    public float floatSpeed = 1f;

    // Colors for normal and critical hits
    public Color normalColor = Color.white;
    public Color criticalColor = Color.yellow;

    // Update is called once per frame
    void Update()
    {
        if(lifeCounter > 0)
        {
            lifeCounter -= Time.deltaTime;

            if(lifeCounter <= 0)
            {
                Destroy(gameObject);
            }
        }

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }
    
    public void Setup(int damageDisplay, bool isCritical)
    {
        lifeCounter = lifeTime;

        damageText.text = damageDisplay.ToString();

        // Change color based on whether it's a critical hit or not
        damageText.color = isCritical ? criticalColor : normalColor;
    }
}
