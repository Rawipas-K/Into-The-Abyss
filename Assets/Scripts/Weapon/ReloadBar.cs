using UnityEngine;
using UnityEngine.UI;

public class ReloadBar : MonoBehaviour
{
    public static ReloadBar instance;
    public Slider reloadSlider;

    public float reloadTime;

    private float timer;
    private bool isReloading = false;

    private void Awake()
    {
        instance = this;
        reloadSlider = GetComponent<Slider>();
        reloadSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isReloading)
        {
            timer += Time.deltaTime;
            reloadSlider.value = Mathf.Clamp(timer, 0f, reloadTime); // Clamp from 0 to max

            if (timer >= reloadTime)
            {
                isReloading = false;
                reloadSlider.gameObject.SetActive(false); // Hide the slider on reload complete
            }
        }
    }
    
    public void StartReload(float reloadTime)
    {
        timer = 0f;
        this.reloadTime = reloadTime;

        reloadSlider.maxValue = reloadTime;
        reloadSlider.value = 0f;
        isReloading = true;

        reloadSlider.gameObject.SetActive(true); // Show the slider on reload start
    }
}
