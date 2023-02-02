using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRenderer : MonoBehaviour
{
    public static readonly string idleState = "Idle";
    public static readonly string[] lightAttacks = { "Light","Down,ForwardDiag,Forward,Light", "Down,BackDiag,Back,Light" };
    public static readonly string[] lightAttackNames = { "Punch","QCF Light", "QCB Light" };
    public static readonly string[] mediumAttacks = { "Medium" , "Down,ForwardDiag,Forward,Medium", "Down,BackDiag,Back,Medium" };
    public static readonly string[] mediumAttackNames = { "Kick", "QCF Medium", "QCB Medium" };
    public static readonly string[] heavyAttacks = { "Down,ForwardDiag,Forward,Heavy", "Down,BackDiag,Back,Heavy" };
    public static readonly string[] heavyAttackNames = { "QCF Heavy", "QCB Heavy" };

    Animator animator;
    PlayerCharacter player;

    [SerializeField] Transform attackDetect;
    public float attackDetectRadius;

    public LayerMask enemyLayer;

    bool isGainingMeter;
    float meterGain;
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void checkForAttack(string commandInput, string attackType)
    {
        string[] attacksToCheck = null;
        string[] relatedAttackNames = null;

        if(attackType == "Light")
        {
            attacksToCheck = lightAttacks;
            relatedAttackNames = lightAttackNames;
        } else if(attackType == "Medium")
        {
            attacksToCheck = mediumAttacks;
            relatedAttackNames = mediumAttackNames;
        } else if(attackType == "Heavy")
        {
            attacksToCheck = heavyAttacks;
            relatedAttackNames = heavyAttackNames;
        }

        for(int i = 0; i < attacksToCheck.Length; i++)
        {
            if(commandInput == attacksToCheck[i])
            {
                
                animator.Play(relatedAttackNames[i]);
                break;
            }
        }
    }

    public void attackEnemy(float damage)
    {
        Collider2D hitEnemy = Physics2D.OverlapCircle(attackDetect.position, attackDetectRadius, enemyLayer);

        if(hitEnemy == null)
        {
            return;
        }
        else
        {
            EnemyCharacter hurtEnemy = hitEnemy.GetComponent<EnemyCharacter>();
            hurtEnemy.TakeDamage(damage);
            if(hurtEnemy.currentHealth > 0)
            {
                Debug.Log("Knockback!");
                applyKnockback(hurtEnemy);
            }
            //Write logic for damaging the enemy
            if (isGainingMeter)
            {
                player.currentMeter += meterGain;
                if(player.currentMeter > player.maxMeter)
                {
                    player.currentMeter = player.maxMeter;
                }
            }
        }
    }

    //Applies knockback to the enemy
    public void applyKnockback(EnemyCharacter hurtEnemy)
    {
        
        Vector3 knockDirection = hurtEnemy.transform.position - transform.position;
        knockDirection.y = .1f;
        knockDirection = knockDirection.normalized;
        Debug.Log(knockDirection);

        hurtEnemy.knockEnemyBack(knockDirection);
    }

    public void setMeterGain(int meterGainVal)
    {
        meterGain = meterGainVal;
    }

    public void setMeterBool(int willGain)
    {
        if(willGain == 1)
        {
            isGainingMeter = true;
        }
        else
        {
            isGainingMeter = false;
        }
    }

    public void ResetToIdle()
    {
        player.characterInput.isAttacking = false;
        player.isAttacking = false;
        animator.Play(idleState);
    }

    public void DepleteMeter(int meterToDeplete)
    {
        player.currentMeter -= meterToDeplete;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackDetect.position, attackDetectRadius);
    }
}
