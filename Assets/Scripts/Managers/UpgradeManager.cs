using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject upgradeCanvasUI; 
    public GameObject skillLearnUI;    
    
    [Header("Systems")]
    public PlayerStats playerStats;
    public PlayerExp playerXp;         
    public SkillQueueManager skillManager;

    [Header("Status Upgrades")]
    public Button btnHealth; public TextMeshProUGUI healthtxt; public int healthToAdd = 20;
    public Button btnDamage; public TextMeshProUGUI damagetxt; public float damageToAdd= 0.1f;
    public Button btnSpeed;  public TextMeshProUGUI speedtxt;  public float speedToAdd = 0.5f;
    public Button btnCooldown; public TextMeshProUGUI cooldowntxt; public float cooldownToAdd = 0.1f;

    [Header("Skill Unlocks")]
    
    public Button btnUnlockFire;   public SkillData fireSkillData;   public TextMeshProUGUI txtFire;
    public Button btnUnlockWater;  public SkillData waterSkillData;  public TextMeshProUGUI txtWater;
    public Button btnUnlockAir;  public SkillData airSkillData;  public TextMeshProUGUI txtAir;
    public Button btnUnlockEarth;  public SkillData earthSkillData;  public TextMeshProUGUI txtEarth;
    public Button btnUnlockLightning;  public SkillData lightningSkillData;  public TextMeshProUGUI txtLightning;
    public Button btnUnlockDark;  public SkillData darkSkillData;  public TextMeshProUGUI txtDark;
    public Button btnUnlockLight;  public SkillData lightSkillData;  public TextMeshProUGUI txtLight;
    public Button btnUnlockNature; public SkillData natureSkillData; public TextMeshProUGUI txtNature;
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            if(playerStats == null) playerStats = player.GetComponent<PlayerStats>();
            if(skillManager == null) skillManager = player.GetComponent<SkillQueueManager>();
            if(playerXp == null) playerXp = player.GetComponent<PlayerExp>();
        }

        if(upgradeCanvasUI != null) upgradeCanvasUI.SetActive(false);
        if(skillLearnUI != null) skillLearnUI.SetActive(false);

        if(btnHealth) btnHealth.onClick.AddListener(() => ApplyUpgrade("health"));
        if(btnDamage) btnDamage.onClick.AddListener(() => ApplyUpgrade("damage"));
        if(btnSpeed) btnSpeed.onClick.AddListener(() => ApplyUpgrade("speed"));
        if(btnCooldown) btnCooldown.onClick.AddListener(() => ApplyUpgrade("cooldown"));


        SetupSkillButton(btnUnlockFire, txtFire, fireSkillData);
        SetupSkillButton(btnUnlockWater, txtWater, waterSkillData);
        SetupSkillButton(btnUnlockAir, txtAir, airSkillData);
        SetupSkillButton(btnUnlockEarth, txtEarth, earthSkillData);
        SetupSkillButton(btnUnlockNature, txtNature, natureSkillData);
        SetupSkillButton(btnUnlockLightning, txtLightning, lightningSkillData);
        SetupSkillButton(btnUnlockDark, txtDark, darkSkillData);
        SetupSkillButton(btnUnlockLight, txtLight, lightSkillData);

        
        UpdateUI();
    }


    void SetupSkillButton(Button btn, TextMeshProUGUI txt, SkillData data)
    {
        if (btn != null && data != null)
        {
            btn.onClick.AddListener(() => ApplySkillUnlock(data));
            if(txt != null) txt.text = data.skillName; 
        }
    }

    public void OpenUpgradeMenu()
    {
        UpdateUI();
        
        int level = playerXp.level;

        // Regra: Nível PAR (2, 4, 6...) E Menor ou igual a 14
        bool isSkillLevel = (level % 2 == 0) && (level <= 14);

        if (isSkillLevel)
        {
            if(skillLearnUI) skillLearnUI.SetActive(true);
            if(upgradeCanvasUI) upgradeCanvasUI.SetActive(false);
            
            RefreshSkillButtonsState(); 
        }
        else
        {
            if(upgradeCanvasUI) upgradeCanvasUI.SetActive(true);
            if(skillLearnUI) skillLearnUI.SetActive(false);
        }
        
        Time.timeScale = 0f; 
    }

    void RefreshSkillButtonsState()
    {
        CheckButtonState(btnUnlockFire, txtFire, fireSkillData);
        CheckButtonState(btnUnlockWater, txtWater, waterSkillData);
        CheckButtonState(btnUnlockAir, txtAir, airSkillData);
        CheckButtonState(btnUnlockEarth, txtEarth, earthSkillData);
        CheckButtonState(btnUnlockNature, txtNature, natureSkillData);
        CheckButtonState(btnUnlockLightning, txtLightning, lightningSkillData);
        CheckButtonState(btnUnlockDark, txtDark, darkSkillData);
        CheckButtonState(btnUnlockLight, txtLight, lightSkillData);
    }

    void CheckButtonState(Button btn, TextMeshProUGUI txt, SkillData data)
    {
        if (btn == null || data == null || skillManager == null) return;

        if (skillManager.HasLearnedSkill(data))
        {
            // Já aprendeu: Desativa clique e deixa transparente
            btn.interactable = false;
            SetAlpha(btn, txt, 0.5f); // 50% visível
            if(txt) txt.text = data.skillName + " (Unlocked)";
        }
        else
        {
            // Disponível
            btn.interactable = true;
            SetAlpha(btn, txt, 1f); // 100% visível
            if(txt) txt.text = data.skillName;
        }
    }

    void SetAlpha(Button btn, TextMeshProUGUI txt, float alpha)
    {
        var colors = btn.colors;
        Color c = colors.normalColor;
        c.a = alpha;
        colors.disabledColor = new Color(c.r, c.g, c.b, alpha);
        btn.colors = colors;

        if (txt != null) txt.alpha = alpha;
    }

    public void ApplySkillUnlock(SkillData skill)
    {
        if(skillManager != null && skill != null)
        {
            skillManager.LearnNewSkill(skill);
        }
        CloseUpgradeMenu();
    }

    public void ApplyUpgrade(string type)
    {
        if (playerStats == null) return;
        switch (type)
        {
            case "health": playerStats.UpgradeHealth(healthToAdd); break;
            case "damage": playerStats.UpgradeDamage(damageToAdd); break;
            case "speed": playerStats.UpgradeSpeed(speedToAdd); break;
            case "cooldown": playerStats.UpgradeCooldown(cooldownToAdd); break;
        }
        CloseUpgradeMenu();
    }

    public void CloseUpgradeMenu()
    {
        if(upgradeCanvasUI) upgradeCanvasUI.SetActive(false);
        if(skillLearnUI) skillLearnUI.SetActive(false);
        Time.timeScale = 1f;
    }
    
    private void UpdateUI()
    {
        if(healthtxt!=null) healthtxt.text = "+ "+ healthToAdd+ " HP";
        if(damagetxt!=null) damagetxt.text = "+ "+ (damageToAdd*100)+"% Dmg";
        if(speedtxt!=null) speedtxt.text = "+ "+ speedToAdd+ " Spd";
        if(cooldowntxt!=null) cooldowntxt.text = "- "+(cooldownToAdd*100)+"% CD";
    }
}