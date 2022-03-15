using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("Input string record")]
    public string attackInput = "";
    [Header("Facing Right, is attacking, etc.")]
    public bool isFacingRight = true;
    [SerializeField] bool isAttacking;
    bool timerStart;

    public float inputClearTimerVal;
    float inputClearTimer;

    PlayerCharacter player;

    [Header ("Inputs - Tracking, Attack Buttons, etc.")]
    public List<KeyCode> Inputs = new List<KeyCode>();
    public List<KeyCode> AttackButtons = new List<KeyCode>();
    // Start is called before the first frame update
    void Start()
    {
        inputClearTimer = inputClearTimerVal;
        player = GetComponent<PlayerCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking)
        {
            checkForInput();
            checkForRelease();
            checkForAttackInput();
        }

        if(timerStart && inputClearTimer > 0)
        {
            inputClearTimer -= Time.deltaTime;
        }

        if(inputClearTimer <= 0)
        {
            attackInput = "";
            Inputs.Clear();
        }

        
    }


    public void checkForInput()
    {
        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {

                attackInput = attackInput + "Down";
                Inputs.Add(KeyCode.S);
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            player.isCrouching = true;
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    if (isFacingRight)
                    {
                        attackInput = attackInput + "BackDiag";

                    }
                    else
                    {
                        attackInput = attackInput + "ForwardDiag";
                    }

                    Inputs.Add(KeyCode.A);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    if (isFacingRight)
                    {
                        attackInput = attackInput + "ForwardDiag";
                    }
                    else
                    {
                        attackInput = attackInput + "BackDiag";
                    }

                    Inputs.Add(KeyCode.D);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (isFacingRight)
                {
                    attackInput = attackInput + "Forward";
                }
                else
                {
                    attackInput = attackInput + "Back";
                }
                Inputs.Add(KeyCode.D);

                
            }

            if (Input.GetKey(KeyCode.D))
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    Debug.Log("AAAAA");
                    if (isFacingRight)
                    {
                        attackInput = attackInput + "ForwardDiag";
                    }
                    else
                    {
                        attackInput = attackInput + "BackDiag";
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                if (isFacingRight)
                {
                    attackInput = attackInput + "Back";
                }
                else
                {
                    attackInput = attackInput + "Forward";
                }
                Inputs.Add(KeyCode.A);

                
            }
            if (Input.GetKey(KeyCode.A))
            {
               
                if (Input.GetKeyDown(KeyCode.S))
                {
                    
                    if (isFacingRight)
                    {
                        attackInput = attackInput + "BackDiag";
                    }
                    else
                    {
                        attackInput = attackInput + "ForwardDiag";
                    }
                }
            }
        }

        //Note: Will likely need array of keys/buttons that correspond to attack buttons
        //Run a for loop check that determines whether or not the pressed button is an attack button. If so, then
        //Trigger attack

        if (!Input.anyKey)
        {
            timerStart = true;
        }
        else
        {
            timerStart = false;
            inputClearTimer = inputClearTimerVal;
        }
    }

    public void checkForRelease()
    {
        foreach(KeyCode kCode in Inputs)
        {
            if (Input.GetKeyUp(kCode))
            {
                if(kCode == KeyCode.S)
                {
                    player.isCrouching = false;
                    player.setHighJumpTimer();
                    if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    {
                        
                        if (Input.GetKey(KeyCode.A))
                        {
                            if (isFacingRight)
                            {
                                attackInput = attackInput + "Back";
                            }
                            else
                            {
                                attackInput = attackInput + "Forward";
                            }
                        } else if (Input.GetKey(KeyCode.D))
                        {
                            if (isFacingRight)
                            {
                                attackInput = attackInput + "Forward";
                            }
                            else
                            {
                                attackInput = attackInput + "Back";
                            }
                        }
                    }
                }
                else
                {
                    if(kCode == KeyCode.A || kCode == KeyCode.D)
                    {
                        if (Input.GetKey(KeyCode.S))
                        {
                            attackInput = attackInput + "Down";
                        }
                    }
                }
            }
        }
    }

    public void checkForAttackInput()
    {
        foreach(KeyCode attackButton in AttackButtons)
        {
            if (Input.GetKeyDown(attackButton))
            {
                Debug.Log("ATTACK!");
            }
        }
    }
}
