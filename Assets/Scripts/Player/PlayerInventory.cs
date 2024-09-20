using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{   
    public static PlayerInventory instance;

    public List<Gun> availableGuns = new List<Gun>(2); // Maximum of 2 guns
    public List<Gun> droppedGuns = new List<Gun>(100); // Dropped gun list
    public int currentGun;

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Gun
        UIController.instance.currentGun.sprite = availableGuns[currentGun].gunUI;
        UIController.instance.gunText.text = availableGuns[currentGun].weaponName;
        availableGuns[currentGun].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(availableGuns.Count > 0)
            {
                //Swap gun and update UI
                currentGun++;
                if(currentGun >= availableGuns.Count)
                {
                    currentGun = 0;
                }

                SwitchGun();

            } 
            else
            {
                Debug.LogError("Player has no gun");
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            DropGun();
        }

    }

    public void SwitchGun()
    {
        //foreach(Gun theGun in availableGuns)
        for(int i = 0; i < availableGuns.Count; i++)
        {
            //theGun.gameObject.SetActive(false);
            availableGuns[i].gameObject.SetActive(false); // Deactivate all guns
        }

        availableGuns[currentGun].gameObject.SetActive(true); // Activate the new gun

        AudioManager.instance.PlaySFX("PickupGun");
        UIController.instance.currentGun.sprite = availableGuns[currentGun].gunUI;
        UIController.instance.gunText.text = availableGuns[currentGun].weaponName;
    }

    void DropGun()
    {
        if (availableGuns.Count > 1)
        {
            //availableGuns[currentGun].gameObject.SetActive(false);
            //availableGuns.RemoveAt(currentGun);

            // Remove the current gun from available guns and add it to dropped guns
            Gun droppedGun = availableGuns[currentGun];
            availableGuns[currentGun].gameObject.SetActive(false);
            availableGuns[currentGun].DropGunPickup(transform.localPosition);
            availableGuns.RemoveAt(currentGun);
            droppedGuns.Add(droppedGun);

            if (availableGuns.Count > 0)
            {
                // Switch to a different gun
                currentGun = 0;
                SwitchGun();
            }

            AudioManager.instance.PlaySFX("DropGun");
        }
    }

    public void PickUpGun(Gun gun)
    {
        // Remove the gun from dropped guns and add it back to available guns
        droppedGuns.Remove(gun);
        availableGuns.Add(gun);

        currentGun = 1;
        SwitchGun();
    }
}
