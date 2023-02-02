using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : MonoBehaviour
{
    //Health and meter values
    public float maxHealth;
    public float currentHealth;
    public float maxMeter;
    public float currentMeter;


    //Move speed - forward and back
    public float forwardMovSpeed;
    public float backwardMovSpeed;
    //Detect radius to check if the player is in range for an attacl
    public float attackDetectRadius;

    //Radius to detect if player is hit by an attack
    public float attackRadius;
    //Attack bool. Prevents movement
    public bool isAttacking;

    //How long the enemy waits between actions
    public float waitTimer;
    float waitTime;
    //How long the enemy moves towards the player
    public float moveTimer;
    float moveTime;

    //The rate at which the enemy loses meter
    public float meterDepleteRate;

    //How long the enemy is knocked back for
    public float knockbackTimer;
    float knockbackTime;

    //how much the player is knocked back
    public float knockbackForce;

    //Rigidbody and the animator
    Rigidbody2D rb;
    Animator animator;

    //bool for moving back. Prevents forward movement
    bool isMovingBack;
    //Whether or not meter is gained during an attack
    bool isGainingMeter;
    //Meter move multiplier, increases/decreases depending on how much meter the enemy has
    float meterMoveMultiplier;
    //How much meter is gained from an attack
    float meterGain;

    //The layer the player is on, for attacks
    public LayerMask playerLayer;

    //The target's transform
    Transform targetPlayer;
    //reference to the player
    PlayerCharacter player;
    //Where the enemy's hitbox is
    [SerializeField] Transform attackPos;

    void Start()
    {
        targetPlayer = FindObjectOfType<PlayerCharacter>().transform;
        player = FindObjectOfType<PlayerCharacter>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        waitTime = waitTimer;
        moveTime = moveTimer;
    }

    // Update is called once per frame
    void Update()
    {
        checkForDirectionChange();

        if(knockbackTime > 0)
        {
            knockbackTime -= Time.deltaTime;
            return;
        }

        if (player.isAttacking && !isAttacking)
        {
            MoveAwayFromPlayer();
            waitTime = waitTimer;
            moveTime = moveTimer;
        } else if(!player.isAttacking && !isAttacking)
        {
            if (isMovingBack)
            {
                isMovingBack = false;
                animator.SetFloat("MoveX", 0);
            }
            if (waitTime <= 0)
            {
                MoveToPlayer();
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        checkForMeterThreshold();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void checkForDirectionChange()
    {
        Vector3 scalar = transform.localScale;

        if(targetPlayer.position.x < transform.position.x)
        {
            scalar.x = -1;
        } else if(targetPlayer.position.x > transform.position.x)
        {
            scalar.x = 1;
        }

        transform.localScale = scalar;
    }

    public void MoveToPlayer()
    {
        if (moveTime > 0)
        {
            Vector3 moveDirection = targetPlayer.position - transform.position;
            rb.velocity = new Vector2(moveDirection.x * forwardMovSpeed * meterMoveMultiplier, rb.velocity.y);
            animator.SetFloat("MoveX", 1);
            animator.SetBool("isDashing", true);
            currentMeter -= 1 * meterDepleteRate * Time.deltaTime;
            moveTime -= Time.deltaTime;
        }
        else
        {
            waitTime = waitTimer;
            moveTime = moveTimer;
            animator.SetFloat("MoveX", 0);
            animator.SetBool("isDashing", false);
            rb.velocity = Vector2.zero;
        }

        if(Vector2.Distance(transform.position, targetPlayer.position) < attackDetectRadius)
        {
            triggerAttack();
        }
    }

    public void MoveAwayFromPlayer()
    {
        Vector3 directionToMove = transform.position - targetPlayer.position;

        animator.SetFloat("MoveX", 1);

        rb.velocity = new Vector2(directionToMove.x * (backwardMovSpeed / 2) * meterMoveMultiplier, rb.velocity.y);
    }

    public void Attack()
    {

    }

    public void knockEnemyBack(Vector3 direction)
    {
        //Knockback
        knockbackTime = knockbackTimer;
        StartCoroutine(knockbackCo(knockbackTimer, direction));
    }

    //Used for knocking the enemy back
    IEnumerator knockbackCo(float knockTime, Vector3 directionOfKnock)
    {
        while(knockTime > 0)
        {
            rb.AddForce(directionOfKnock * knockbackForce, ForceMode2D.Impulse);
            knockTime -= Time.deltaTime;
            yield return null;
        }
        
    }

    //used to knock the player back (Calculate direction)
    public void knockbackPlayer(PlayerCharacter player)
    {
        Vector3 knockdirection = player.transform.position - transform.position;
        knockdirection = knockdirection.normalized;

        player.ApplyKnockback(knockdirection);
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);
        Gizmos.DrawWireSphere(transform.position, attackDetectRadius);
    }

    public void AttackPlayer(float damage)
    {
        Collider2D hitEnemy = Physics2D.OverlapCircle(attackPos.position, attackRadius, playerLayer);

        if (hitEnemy == null)
        {
            return;
        }
        else
        {
            PlayerCharacter hurtEnemy = hitEnemy.GetComponent<PlayerCharacter>();
            hurtEnemy.TakeDamage(damage);
            if (hurtEnemy.currentHealth > 0)
            {
                
                knockbackPlayer(hurtEnemy);
            }
            //Write logic for damaging the enemy
            if (isGainingMeter)
            {
                currentMeter += meterGain;
                if (currentMeter > maxMeter)
                {
                    currentMeter = maxMeter;
                }
            }
        }
    }
    void checkForMeterThreshold()
    {
        if (currentMeter > 75)
        {
            meterMoveMultiplier = 1;
        }
        else if (currentMeter <= 75 && currentMeter > 50)
        {
            meterMoveMultiplier = .7f;
        }
        else if (currentMeter <= 50 && currentMeter > 25)
        {
            meterMoveMultiplier = .5f;
        }
    }

    public void setMeterGain(int meterGainVal)
    {
        meterGain = meterGainVal;
    }

    public void setMeterBool(int willGain)
    {
        if (willGain == 1)
        {
            isGainingMeter = true;
        }
        else
        {
            isGainingMeter = false;
        }
    }
    public void DepleteMeter(int meterToDeplete)
    {
        currentMeter -= meterToDeplete;
    }
    public void ResetToIdle()
    {
        isAttacking = false;
        waitTime += 2f;
        animator.Play("Idle");
    }

    public void triggerAttack()
    {
        bool hasSelectedAttack = false;

        StartCoroutine(selectAttackCo(hasSelectedAttack));
    }

    IEnumerator selectAttackCo(bool selected)
    {
        string[] attackNames = { "Punch", "QCF Light", "QCB Light", "Kick", "QCF Medium", "QCB Medium", "QCF Heavy", "QCB Heavy" };

        while (!selected)
        {
            int selectedIndex = Random.Range(0, attackNames.Length);

            isAttacking = true;
            animator.Play(attackNames[selectedIndex]);
            selected = true;
        }

        yield return null;
    }
}
