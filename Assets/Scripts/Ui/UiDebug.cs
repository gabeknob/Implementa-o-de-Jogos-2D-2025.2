using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiDebug : MonoBehaviour
{
    [Header("WaveInfo")]
    public TextMeshProUGUI leveltxt;
    public TextMeshProUGUI statMultitxt;
    public TextMeshProUGUI cdMultitxt;

    [Header("SkillsInfo")] 
    public SkillQueueManager skillManager; 
    public GameObject debugUI;
    public bool isDebugUIActive;
    public TextMeshProUGUI cooldownText; 

    [Header("PlayerStats")]
    public GameObject player;
    private PlayerStats stats;
    public TextMeshProUGUI currentHealthtxt;
    public TextMeshProUGUI maxHealthtxt;

    void Start()
    {
        if (player != null)
        {
            stats = player.GetComponent<PlayerStats>();

            if (skillManager == null) 
                skillManager = player.GetComponent<SkillQueueManager>();
        }
    }

    void Update()
    {
        UpdatePlayerStats();
        
        if (!isDebugUIActive)
        {
            debugUI.SetActive(false);
        }
        else
        {
            debugUI.SetActive(true);
            UpdateCooldownsText(); 
            UpdateWaveInfo();
        }
    }

    void UpdateCooldownsText()
    {
        if (skillManager == null) return;

        string cdString = "Cooldowns Ativos:\n";

        for (int i = 0; i < skillManager.activeSlots.Length; i++)
        {
            SkillState state = skillManager.activeSlots[i];
            
            string keyName = "";
            if (i == 0) keyName = "[Q] ";
            if (i == 1) keyName = "[W] ";
            if (i == 2) keyName = "[E] ";
            if (i == 3) keyName = "[R] ";


            if (state != null && state.data != null)
            {
                string skillName = state.data.skillName;
                float duration = state.data.cooldown;
                

                float timePassed = Time.time - state.lastUsedTime;
                float timeRemaining = duration - timePassed;

                if (timeRemaining > 0)
                {
                    cdString += keyName + skillName + ": " + timeRemaining.ToString("F1") + "s\n";
                }
                else
                {
                    cdString += keyName + skillName + ": Pronto (" + state.currentCharges + "x)\n";
                }
            }
            else
            {
                cdString += keyName + "Vazio\n";
            }
        }

        cooldownText.text = cdString;
    }

    void UpdatePlayerStats()
    {
        if(player != null && stats != null)
        {
            maxHealthtxt.text = "Max: " + stats.maxHealth.ToString();
            currentHealthtxt.text = "Current: " + stats.currentHealth.ToString();
        }
    }

    void UpdateWaveInfo()
    {
        GameObject spawnerObj = GameObject.FindWithTag("EnemySpawner");
        if (spawnerObj != null)
        {
            EnemySpawner enemySpawner = spawnerObj.GetComponent<EnemySpawner>();
            leveltxt.text = "Wave: " + enemySpawner.WaveLevelUI;
            statMultitxt.text = "Stat Multiplier: " + enemySpawner.StatMultiplierUI;
            cdMultitxt.text = "CooldownMulti: " + enemySpawner.CooldownMultiplierUI;
        }
    }
}