using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    //Basic Movement
    Vector3 move;
    Rigidbody2D rb;
    InputHandler characterInput;
    [Header("Movement")]
    public float forwardMovSpeed, backwardMovSpeed;

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
    public bool isDashingForward;
    public float doubleTapTime;
    public float dashDistance;
    public float dashMoveSpeed;
    KeyCode lastKeyCode;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        characterInput = GetComponent<InputHandler>();
        doubleJumpHeight = normalJumpHeight / 1.3f;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isJumping && !isDashingBack)
        {
            checkForMoveInput();
        }

        if (!isDashingBack)
        {
            checkForDashInput();
        }

        checkForJumpInput();
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



        rb.velocity = new Vector2(move.x * moveSpeed, rb.velocity.y);

        
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
                if(doubleTapTime > Time.time && lastKeyCode == KeyCode.A && !isDashingForward)
                {
                    isDashingForward = true;
                }
                else
                {
                    doubleTapTime = Time.time + .5f;
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
            }
        } else if (Input.GetKeyUp(KeyCode.D))
        {
            if (isDashingForward)
            {
                isDashingForward = false;
            }
        }
    }

    public IEnumerator Dash()
    {
        isDashingBack = true;
        rb.velocity = new Vector2(rb.velocity.x, 0f);

        rb.AddForce(new Vector2(dashDistance * transform.localScale.x * -1, 0f), ForceMode2D.Impulse);

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
            isJumping = true;
        }

        if (isGrounded)
        {
            jumpCount = jumpCountVal;
            isJumping = false;
        }
        else
        {
            isJumping = true;
        }

        if (isCrouching)
        {
            if(highJumpTimer > 0 && Input.GetButtonDown("Jump") && jumpCount > 0){
                Debug.Log("HIGH JUMP");
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(feetPos.position, feetRadius);
    }
}
