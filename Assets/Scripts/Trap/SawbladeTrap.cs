using UnityEngine;

public class SawbladeTrap : MonoBehaviour
{
    public float speed;
    public float movementDistance;
    public bool isStartMovingUp, isStartMovingDown, isStartMovingLeft, isStartMovingRight;

    private Vector2 startingPosition;
    private bool isMovingUp, isMovingLeft;
    private Vector2 moveDir;
    private float playSFXTime = 10;
    private float Timer;

    void Start()
    {
        startingPosition = transform.position;
        Timer = playSFXTime;

        if (isStartMovingUp)
        {   
            isMovingUp = true;
            moveDir = Vector2.up * speed * Time.deltaTime;
        }
        else if (isStartMovingDown)
        {   
            isMovingUp = false;
            moveDir = Vector2.down * speed * Time.deltaTime;
        }
        else if (isStartMovingLeft)
        {
            isMovingLeft = true;
            moveDir = Vector2.left * speed * Time.deltaTime;
        }
        else if (isStartMovingRight)
        {
            isMovingLeft = false;
            moveDir = Vector2.right * speed * Time.deltaTime;
        }
    }

    void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0)
        {
            AudioManager.instance.PlaySFX("SawBlade");
            Timer = playSFXTime;
        }

        if(isStartMovingUp)
        {
            // Movement logic based on current direction
            if (isMovingUp)
            {
                // Move up until reaching the target distance
                if (transform.position.y - startingPosition.y >= movementDistance)
                {
                    isMovingUp = false;
                }
                else
                {
                    transform.Translate(Vector2.up * speed * Time.deltaTime);
                }
            }
            else 
            {
                // Move down until reaching the starting position
                if (transform.position.y <= startingPosition.y)
                {
                    isMovingUp = true; // Switch to moving up when reaching starting position
                }
                else
                {
                    transform.Translate(Vector2.down * speed * Time.deltaTime);
                }
            }
        }

        if (isStartMovingDown)
        {
            // Movement logic based on current direction
            if (isMovingUp)
            {
                // Move up until reaching the target distance
                if (transform.position.y >= startingPosition.y)
                {
                    isMovingUp = false;
                }
                else
                {
                    transform.Translate(Vector2.up * speed * Time.deltaTime);
                }
            }
            else 
            {
                // Move down until reaching the starting position
                if (startingPosition.y - transform.position.y >= movementDistance)
                {
                    isMovingUp = true; // Switch to moving up when reaching starting position
                }
                else
                {
                    transform.Translate(Vector2.down * speed * Time.deltaTime);
                }
            }
        }

        if (isStartMovingLeft)
        {
            if (isMovingLeft)
            {
                if (startingPosition.x - transform.position.x >= movementDistance)
                {
                    isMovingLeft = false;
                }
                else
                {
                    transform.Translate(Vector2.left * speed * Time.deltaTime);
                }
            }
            else 
            {
                if (transform.position.x >= startingPosition.x)
                {
                    isMovingLeft = true;
                }
                else
                {
                    transform.Translate(Vector2.right * speed * Time.deltaTime);
                }
            }
        }

        if (isStartMovingRight)
        {
            if (isMovingLeft)
            {
                if (transform.position.x <= startingPosition.x)
                {
                    isMovingLeft = false;
                }
                else
                {
                    transform.Translate(Vector2.left * speed * Time.deltaTime);
                }
            }
            else 
            {
                if (transform.position.x - startingPosition.x >= movementDistance)
                {
                    isMovingLeft = true;
                }
                else
                {
                    transform.Translate(Vector2.right * speed * Time.deltaTime);
                }
            }
        }
    }
}
