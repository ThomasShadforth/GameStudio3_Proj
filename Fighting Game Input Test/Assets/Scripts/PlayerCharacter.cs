using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacter : MonoBehaviour
{
    //Basic Movement
    Vector3 move;
    Rigidbody2D rb;
    public InputHandler characterInput;
    [Header("Movement")]
    public float forwardMovSpeed, backwardMovSpeed;

    //Health Values
    [Header("Health Values - Max, current, etc.")]
    public float maxHealth;
    public float currentHealth;

    //Jump Values
    [Header("Jump Values (Height, number, etc)")]
    public bool isJumping;
    public bool isGrounded;
    public bool isCrouching;
    public float highJumpTimerVal;
    public float normalJumpHeight;
    [SerializeField] float doubleJumpHeight;
    public LayerMask whatIsGround;
    [SerializeField] Transform feetPos;
    public float jumpCountVal;
    public float feetRadius;
    float jumpCount;
    float highJumpTimer;

    //Dash Values
    [Header("Dash Values - Boolean Checks, distance, move speed, etc.")]
    public bool isDashingBack;
    public bool isAirDashing;
    public bool isDashingForward;
    public float doubleTapTime;
    public int airDashCount;
    public int airDashVal;
    public float airDashDistance;
    public float dashDistance;
    public float dashMoveSpeed;
    KeyCode lastKeyCode;

    [Header("Meter Values - Meter cap, current meter, deplete rate, etc.")]
    public float maxMeter;
    public float currentMeter;
    public float meterDepleteRate;
    public float meterMoveMultiplier = 1;

    public bool isAttacking;

    public float knockbackForce;
    public float knockbackTimer;
    float knockbackTime;

    Animator animator;

    Transform targetOpponent;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        characterInput = GetComponent<InputHandler>();
        doubleJumpHeight = normalJumpHeight / 1.3f;
        animator = GetComponent<Animator>();
        targetOpponent = FindObjectOfType<EnemyCharacter>().transform;
        airDashCount = airDashVal;
    }

    // Update is called once per frame
    void Update()
    {
        if (characterInput.isAttacking)
        {
            return;
        }

        if(knockbackTime > 0)
        {
            knockbackTime -= Time.deltaTime;
            return;
        }

        if (!isJumping && !isDashingBack)
        {
            checkForMoveInput();
        }

        if (!isDashingBack && !isAirDashing)
        {
            checkForDashInput();
        }

        if (!isDashingBack)
        {
            checkForJumpInput();
        }

        checkForMeterThreshold();

        //For now, have this method called here
        //Eventually, the player will track the opponent's position on the X-Axis so that it faces them when it's on one side or the other
        checkForDirectionChange();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, feetRadius, whatIsGround);
    }

    #region Basic Movement
    void checkForMoveInput()
    {
        
        move.x = Input.GetAxisRaw("Horizontal");

        float moveSpeed;

        if (!isDashingForward)
        {
            moveSpeed = getDirectionSpeed();
        }
        else
        {
            moveSpeed = dashMoveSpeed;
        }

        animator.SetFloat("MoveX", Mathf.Abs(move.x));
        rb.velocity = new Vector2(move.x * moveSpeed * meterMoveMultiplier, rb.velocity.y);

        currentMeter -= (1 * Mathf.Abs(move.x) * meterDepleteRate) * Time.deltaTime;
    }

    float getDirectionSpeed()
    {
        float directionSpeed = 0;

        if (characterInput.isFacingRight)
        {
            if(move.x > 0)
            {
                directionSpeed = forwardMovSpeed;
            } else if(move.x < 0)
            {
                directionSpeed = backwardMovSpeed;
            }

            
        }
        else
        {
            if(move.x > 0)
            {
                directionSpeed = backwardMovSpeed;
            } else if(move.x < 0)
            {
                directionSpeed = forwardMovSpeed;
            }
        }

        return directionSpeed;

    }
    #endregion

    #region Dash Methods
    void checkForDashInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            
            if (characterInput.isFacingRight)
            {
                if (isGrounded)
                {
                    if (doubleTapTime > Time.time && lastKeyCode == KeyCode.A && !isDashingBack)
                    {
                        StartCoroutine(Dash());
                    }
                    else
                    {
                        doubleTapTime = Time.time + .5f;
                    }
                }
                else
                {

                }
            }

            else
            {
                if (isGrounded)
                {
                    if (doubleTapTime > Time.time && lastKeyCode == KeyCode.A && !isDashingForward)
                    {
                        isDashingForward = true;
                        animator.SetBool("isDashing", true);
                    }
                    else
                    {
                        doubleTapTime = Time.time + .5f;
                    }
                }
                else
                {

                }
            }
            lastKeyCode = KeyCode.A;
        }

        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (characterInput.isFacingRight)
            {
                if (doubleTapTime > Time.time && lastKeyCode == KeyCode.D && !isDashingBack)
                {
                    isDashingForward = true;
                    animator.SetBool("isDashing", true);
                }
                else
                {
                    doubleTapTime = Time.time + .5f;
                }
            }
            else
            {
                if(doubleTapTime > Time.time && lastKeyCode == KeyCode.D && !isDashingBack)
                {
                    StartCoroutine(Dash());
                }
                else
                {
                    doubleTapTime = Time.time + .5f;
                }
            }

            lastKeyCode = KeyCode.D;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            if (isDashingForward)
            {
                isDashingForward = false;
                animator.SetBool("isDashing", false);
            }
        } else if (Input.GetKeyUp(KeyCode.D))
        {
            if (isDashingForward)
            {
                isDashingForward = false;
                animator.SetBool("isDashing", false);
            }
        }
    }

    public IEnumerator Dash()
    {
        isDashingBack = true;
        rb.velocity = new Vector2(rb.velocity.x, 0f);

        rb.AddForce(new Vector2(dashDistance * (transform.localScale.x * -1), 0f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(.3f);
        rb.velocity = new Vector2(0, 0);
        isDashingBack = false;
    }
    #endregion

    #region Jump

    void checkForJumpInput()
    {
        if(Input.GetButtonDown("Jump") && isGrounded && jumpCount == 0)
        {
            rb.velocity = Vector3.up * normalJumpHeight;
            animator.SetBool("isJumping", true);
        }

        if(Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            
            if (isJumping)
            {
                //double jump height here
                rb.velocity = Vector3.up * doubleJumpHeight;
                jumpCount--;
            }
            else
            {
                
                //Normal Jump + set is jumping to true
                rb.velocity = Vector3.up * normalJumpHeight;
                jumpCount--;
                
            }
            animator.SetBool("isJumping", true);
            isJumping = true;
        }

        if (isGrounded)
        {
            jumpCount = jumpCountVal;
            isJumping = false;
            animator.SetBool("isJumping", false);
        }
        else
        {
            isJumping = true;
            animator.SetBool("isJumping", true);
        }

        if (isCrouching)
        {
            if(highJumpTimer > 0 && Input.GetButtonDown("Jump") && jumpCount > 0){
                
                rb.velocity = Vector3.up * (normalJumpHeight * 1.5f);
                jumpCount--;
            }
            else
            {
                highJumpTimer -= Time.deltaTime;
                
            }
        }
    }

    public void setHighJumpTimer()
    {
        highJumpTimer = highJumpTimerVal;
    }

    #endregion

    #region Utility Methods
    void checkForDirectionChange()
    {
        Vector3 scalar = transform.localScale;


        if(targetOpponent.position.x > transform.position.x)
        {
            characterInput.isFacingRight = true;
            scalar.x = 1;
            

        }
        else if(targetOpponent.position.x < transform.position.x)
        {
            characterInput.isFacingRight = false;
            scalar.x = -1;
        }

        transform.localScale = scalar;
    }

    void checkForMeterThreshold()
    {
        if(currentMeter > 75)
        {
            meterMoveMultiplier = 1;
        } else if(currentMeter <= 75 && currentMeter > 50)
        {
            meterMoveMultiplier = .7f;
        } else if(currentMeter <= 50 && currentMeter > 25)
        {
            meterMoveMultiplier = .5f;
        }
    }

    #endregion

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void ApplyKnockback(Vector3 directionOfKnock)
    {
        directionOfKnock *= knockbackForce;
        knockbackTime = knockbackTimer;

        StartCoroutine(knockbackCo(knockbackTime, directionOfKnock));
    }

    IEnumerator knockbackCo(float knockTime, Vector3 direction)
    {
        while(knockTime > 0)
        {
            rb.AddForce(new Vector2(direction.x, direction.y), ForceMode2D.Impulse);
            knockTime -= Time.deltaTime;
        }

        yield return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(feetPos.position, feetRadius);
    }
}
