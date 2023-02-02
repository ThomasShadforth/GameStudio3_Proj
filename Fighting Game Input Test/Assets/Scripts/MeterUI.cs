using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeterUI : MonoBehaviour
{
    PlayerCharacter player;
    EnemyCharacter enemy;

    public Image playerHealthUI;
    public Image enemyHealthUI;
    public Image playerMeterUI;
    public Image enemyMeterUI;


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerCharacter>();
        enemy = FindObjectOfType<EnemyCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        updateMeterUI();
        updateHealthUI();
    }

    void updateMeterUI()
    {
        playerMeterUI.fillAmount = player.currentMeter / player.maxMeter;
        enemyMeterUI.fillAmount = enemy.currentMeter / enemy.maxMeter;
    }

    void updateHealthUI()
    {
        playerHealthUI.fillAmount = player.currentHealth / player.maxHealth;
        enemyHealthUI.fillAmount = enemy.currentHealth / enemy.maxHealth;
    }
}
