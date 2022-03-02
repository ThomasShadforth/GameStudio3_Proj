using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    Vector3 move;
    Rigidbody2D rb;
    InputHandler characterInput;
    public float forwardMovSpeed, backwardMovSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        characterInput = GetComponent<InputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        checkForMoveInput();
    }

    void checkForMoveInput()
    {
        move.x = Input.GetAxisRaw("Horizontal");

        float moveSpeed;

        moveSpeed = getDirectionSpeed();

        rb.MovePosition(transform.position + move * moveSpeed * Time.deltaTime);

        
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
}
