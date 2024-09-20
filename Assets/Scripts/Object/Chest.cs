using UnityEngine;

public class Chest : MonoBehaviour
{
    [System.Serializable]
    public class ItemProbability
    {
        public GameObject item;
        [Range(1, 100)]
        public int probability;
        public int minCount = 1;
        public int maxCount = 1;
    }

    public ItemProbability[] potentialItems;
    public SpriteRenderer theSR;
    public Sprite chestOpen;
    public GameObject notification;

    public bool canOpen, isOpen;

    public Transform spawnPoint;

    public float scaleSpeed = 2f;

    // Update is called once per frame
    void Update()
    {
        if(canOpen && !isOpen)
        {
            if(Input.GetKeyDown(KeyCode.E) && PlayerStats.instance.currentKey > 0)
            {
                foreach (var itemProbability in potentialItems)
                {
                    int randomNumber = Random.Range(1, 101); // Random number between 1 and 100
                    if (randomNumber <= itemProbability.probability)
                    {
                        int itemCount = Random.Range(itemProbability.minCount, itemProbability.maxCount + 1);
                        for (int i = 0; i < itemCount; i++)
                        {
                            DropItem(itemProbability.item);
                        }
                    }
                }
                AudioManager.instance.PlaySFX("OpenChest");
                PlayerStats.instance.currentKey -= 1;
                UIController.instance.UpdatePlayerUI("Key");
                isOpen = true; // Set isOpen to true when the chest is opened
                theSR.sprite = chestOpen; // Change the sprite to open chest sprite
            }
            else if (Input.GetKeyDown(KeyCode.E) && PlayerStats.instance.currentKey <= 0)
            {
                AudioManager.instance.PlaySFX("NotEnough");
            }
        }

        if(isOpen)
        {
            notification.SetActive(false);
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.deltaTime * scaleSpeed);
        }
    }

    private void DropItem(GameObject item)
    {
        Vector2 spawnPosition = (Vector2)spawnPoint.position + Random.insideUnitCircle * 0.5f; // Adjust spread by changing the multiplier
        Instantiate(item, spawnPosition, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            notification.SetActive(true);
            canOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            notification.SetActive(false);
            canOpen = false;
        }
    }
}
