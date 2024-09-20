using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public int GunID;
    public Gun theGun;
    public bool isNewGun = true;
    public GameObject message;
    public float waitToBeCollected = .5f;

    private bool canPickup;

    void Awake()
    {
        GunID = Random.Range(1000000,9999999);
    }

    // Update is called once per frame
    void Update()
    {
        if(waitToBeCollected > 0)
        {
            waitToBeCollected -= Time.deltaTime;
        }

        if(canPickup && Input.GetKeyDown(KeyCode.E) && waitToBeCollected <= 0 && PlayerInventory.instance.availableGuns.Count < 2)
        {
            bool alreadyHaveGun = false;

            // Check if it's an old gun that the player dropped and if the player doesn't already have it
            if (!isNewGun)
            {   
                foreach (Gun gunToCheck in PlayerInventory.instance.availableGuns)
                {
                    if (theGun.weaponName == gunToCheck.weaponName)
                    {
                        //Player already have this type of gun
                        alreadyHaveGun = true;
                    }
                }

                if(!alreadyHaveGun)
                {
                    foreach (Gun gunToCheck in PlayerInventory.instance.droppedGuns)
                    {
                        if (GunID == gunToCheck.GunID)
                        {
                            PlayerInventory.instance.PickUpGun(gunToCheck);
                            PlayerInventory.instance.droppedGuns.Remove(gunToCheck); // Remove the gun from dropped guns
                            alreadyHaveGun = true;
                            Destroy(gameObject);
                            AudioManager.instance.PlaySFX("PickupGun");
                            Debug.Log("Add old gun to player work");
                            break; // Exit the loop once the gun is found
                        }
                    }
                }
            }

            // Check if it's a new gun and if the player doesn't already have it
            if (isNewGun)
            {
                foreach (Gun gunToCheck in PlayerInventory.instance.availableGuns)
                {
                    if (theGun.weaponName == gunToCheck.weaponName)
                    {
                        alreadyHaveGun = true;
                        break;
                    }
                }

                if (!alreadyHaveGun)
                {
                    // Instantiate the new gun and add it to the player's inventory
                    Gun gunClone = Instantiate(theGun);
                    gunClone.transform.parent = PlayerController.instance.gunArm;
                    gunClone.transform.position = PlayerController.instance.gunArm.position;
                    gunClone.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    gunClone.transform.localScale = Vector3.one;
                    gunClone.GunID = GunID;
                    PlayerInventory.instance.availableGuns.Add(gunClone);
                    PlayerInventory.instance.currentGun = PlayerInventory.instance.availableGuns.Count - 1;
                    PlayerInventory.instance.SwitchGun();
                    Debug.Log("Add new gun to player work");
                }

                // Destroy the pickup object
                Destroy(gameObject);
                AudioManager.instance.PlaySFX("PickupGun");
            }
        }
    }

    public void OldGun(int gunID)
    {
        GunID = gunID;
        isNewGun = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Player")
        {
            canPickup = true;
            message.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.tag == "Player")
        {
            canPickup = false;
            message.SetActive(false);
        }
    }
}

