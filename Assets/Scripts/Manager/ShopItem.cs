using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public GameObject buyMessage;

    private bool inBuyZone;

    public bool isMedkit, isHealthUpgrade, isAmmoBox, isExpCard, isKey, isWeapon;

    public int itemCost;

    public Gun[] potentialGuns;
    private Gun theGun;
    public SpriteRenderer gunSprite;
    public TextMeshProUGUI infoText;

    // Start is called before the first frame update
    void Start()
    {
        if(isWeapon)
        {
            int selectedGun = Random.Range(0, potentialGuns.Length);
            theGun = potentialGuns[selectedGun];

            gunSprite.sprite = theGun.gunUI;
            infoText.text = theGun.weaponName + "\n- " + theGun.itemCost + " Gold -";
            itemCost = theGun.itemCost;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(inBuyZone)
        {
            if(Input.GetKeyDown(KeyCode.E) && !LevelManager.instance.isPaused)
            {
                if(PlayerStats.instance.currentCoins >= itemCost)
                {
                    LevelManager.instance.SpendCoins(itemCost);

                    if(isMedkit)
                    {
                        PlayerStats.instance.HealPlayer(PlayerStats.instance.maxHealth);

                        gameObject.SetActive(false);
                    }

                    if(isAmmoBox)
                    {
                        Gun currentGun = PlayerInventory.instance.availableGuns[PlayerInventory.instance.currentGun];
                        currentGun.maxAmmo += currentGun.ammoPerPickup;
                    }

                    if(isExpCard)
                    {
                        PlayerStats.instance.GetExp(10);
                    }

                    if(isKey)
                    {
                        LevelManager.instance.GetKey(1);
                    }

                    if(isWeapon)
                    {
                        Gun gunClone = Instantiate(theGun);
                        gunClone.transform.parent = PlayerController.instance.gunArm;
                        gunClone.transform.position = PlayerController.instance.gunArm.position;
                        gunClone.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        gunClone.transform.localScale = Vector3.one;

                        PlayerInventory.instance.availableGuns.Add(gunClone);
                        PlayerInventory.instance.currentGun = PlayerInventory.instance.availableGuns.Count - 1;
                        PlayerInventory.instance.SwitchGun();

                        gameObject.SetActive(false);
                    }

                    AudioManager.instance.PlaySFX("Buy");
                } else
                {
                    AudioManager.instance.PlaySFX("NotEnough");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            buyMessage.SetActive(true);
            inBuyZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            buyMessage.SetActive(false);
            inBuyZone = false;
        }
    }
}
