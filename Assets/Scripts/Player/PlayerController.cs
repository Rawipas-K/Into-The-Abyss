using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private Vector2 moveInput;
    public Rigidbody2D theRB;
    public Transform gunArm;
    public Transform reloadBar;
    public Animator anim;

    public SpriteRenderer bodySR;
    public TrailRenderer bodyTR;

    public float moveSpeed;
    public float baseMoveSpeed;
    public float activeMoveSpeed;
    public float dashSpeed, dashLength, dashCooldown, dashInvincibility;
    public float dashTimer;
    public float dashCooldownTimer;

    [HideInInspector] public bool canMove = true;
    private bool isKnockedBack = false;
    [HideInInspector] public float confusionDuration;
    [HideInInspector] public bool isSlowed = false;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlaySFX("Appearing");
        baseMoveSpeed = PlayerStats.instance.moveSpeed;
        moveSpeed = baseMoveSpeed;
        activeMoveSpeed = moveSpeed;
        dashSpeed = PlayerStats.instance.dashSpeed;
        dashLength = PlayerStats.instance.dashLength;
        dashCooldown = PlayerStats.instance.dashCooldown;
        dashInvincibility = PlayerStats.instance.dashInvincibility;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyStatUpgraded();

        if(canMove && !LevelManager.instance.isPaused)
        {
            //moving
            // If inside confusion smoke, reverse player's input
            confusionDuration -= Time.deltaTime;
            if (confusionDuration > 0)
            {
                // Reverse player's input
                moveInput.x = -Input.GetAxisRaw("Horizontal");
                moveInput.y = -Input.GetAxisRaw("Vertical");
            }
            else
            {
                // Normal input
                moveInput.x = Input.GetAxisRaw("Horizontal");
                moveInput.y = Input.GetAxisRaw("Vertical");
            }

            moveInput.Normalize();

            //transform.position += new Vector3(moveInput.x * Time.deltaTime * moveSpeed, moveInput.y * Time.deltaTime * moveSpeed, 0f);
            //character rotate follow by mouse cursor
            if (!isKnockedBack) // Only set velocity if not knocked back
            {
                theRB.velocity = moveInput * activeMoveSpeed;
            }
            
            Vector3 mousePos = Input.mousePosition;
            Vector3 screenPoint = CameraController.instance.mainCamera.WorldToScreenPoint(transform.localPosition);
        
            if(mousePos.x < screenPoint.x)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                gunArm.localScale = new Vector3(-1f, -1f, 1f);
            } 
            else
            {
                transform.localScale = Vector3.one;
                gunArm.localScale = Vector3.one;
            }

            //rotate gun arm
            Vector2 offset = new Vector2(mousePos.x - screenPoint.x, mousePos.y - screenPoint.y);
            float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            gunArm.rotation = Quaternion.Euler(0, 0, angle);

            //dash
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Dash();
            }

            if(dashTimer > 0)
            {
                dashTimer -= Time.deltaTime;
                if(dashTimer <= 0)
                {
                    activeMoveSpeed = moveSpeed;
                    dashCooldownTimer = dashCooldown;
                    bodyTR.enabled = false;
                }
            }

            if(dashCooldownTimer > 0)
            {
                dashCooldownTimer -= Time.deltaTime;
            }

            if(moveInput != Vector2.zero)
            {
                anim.SetBool("isMoving", true);
            } else
            {
                anim.SetBool("isMoving", false);
            }
        } 
        else
        {
            theRB.velocity = Vector2.zero;
            anim.SetBool("isMoving", false);
        }
    }

    void Dash()
    {
        if(dashCooldownTimer <= 0 && dashTimer <= 0)
        {
            bodyTR.enabled = true;
            activeMoveSpeed = dashSpeed + moveSpeed;
            Debug.Log("activeMoveSpeed = " + activeMoveSpeed);
            dashTimer = dashLength;

            anim.SetTrigger("dash");

            PlayerStats.instance.MakeInvincible(dashInvincibility);

            AudioManager.instance.PlaySFX("PlayerDash");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        List<string> enemyTags = new List<string>{"Enemy","EnemyBullet","Boss","BossBullet"};
        if (enemyTags.Contains(collision.gameObject.tag) && PlayerStats.instance.isInvinc)
        {
            Vector2 difference = (transform.position - collision.transform.position).normalized;
            Vector2 force = difference * 10f; // Adjust knockback strength as needed

            // Apply temporary knockback
            theRB.AddForce(force, ForceMode2D.Impulse);
            isKnockedBack = true; // Set knockback flag
            Debug.Log("Force Work");
            StartCoroutine(KnockbackDuration(0.2f));
        }
    }
    

    IEnumerator KnockbackDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        theRB.velocity = Vector2.zero; // Reset velocity
        isKnockedBack = false; // Clear knockback flag
    }

    public void ApplyStatUpgraded()
    {
        moveSpeed = CalculateStat(baseMoveSpeed, PlayerStats.instance.bonusMoveSpeed);
    }

    private float CalculateStat(float baseStat, float bonus)
    {
        return baseStat * (1 + bonus);
    }
}
