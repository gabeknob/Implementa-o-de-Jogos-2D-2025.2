using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Componente UI que representa uma opção de upgrade
/// Anexe este script a cada um dos 4 slots de upgrade
/// </summary>
public class UpgradeOption : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelText; // Ex: "Nível 2/5"
    [SerializeField] private Button selectButton;
    
    [Header("Visual Feedback (Opcional)")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color commonColor = Color.white;
    [SerializeField] private Color uncommonColor = Color.green;
    [SerializeField] private Color rareColor = Color.blue;
    [SerializeField] private Color epicColor = Color.magenta;
    [SerializeField] private Color legendaryColor = Color.yellow;
    
    private UpgradeData currentUpgrade;
    private UpgradeManagerStats upgradeManagerStats;
    
    private void Awake()
    {
        // Configura o botão
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnButtonClick);
        }
    }
    
    /// <summary>
    /// Configura esta opção com um upgrade específico
    /// </summary>
    public void Setup(UpgradeData upgrade, UpgradeManagerStats manager)
    {
        currentUpgrade = upgrade;
        upgradeManagerStats = manager;
        
        // Atualiza UI
        UpdateUI();
    }
    
    /// <summary>
    /// Atualiza todos os elementos visuais
    /// </summary>
    private void UpdateUI()
    {
        if (currentUpgrade == null) return;
        
        // Ícone
        if (iconImage != null)
        {
            iconImage.sprite = currentUpgrade.icon;
            iconImage.enabled = currentUpgrade.icon != null;
        }
        
        // Nome
        if (nameText != null)
        {
            nameText.text = currentUpgrade.upgradeName;
        }
        
        // Descrição
        if (descriptionText != null)
        {
            string desc = currentUpgrade.description;
            
            // Adiciona o valor na descrição
            string valueStr = currentUpgrade.isPercentage 
                ? $"+{currentUpgrade.value}%" 
                : $"+{currentUpgrade.value}";
            
            desc = desc.Replace("{value}", valueStr);
            descriptionText.text = desc;
        }
        
        // Nível atual
        if (levelText != null)
        {
            int nextLevel = currentUpgrade.currentLevel + 1;
            levelText.text = $"Nível {nextLevel}/{currentUpgrade.maxLevel}";
        }
        
        // Cor baseada na raridade
        if (backgroundImage != null)
        {
            backgroundImage.color = GetRarityColor(currentUpgrade.rarity);
        }
    }
    
    /// <summary>
    /// Retorna cor baseada na raridade
    /// </summary>
    private Color GetRarityColor(int rarity)
    {
        if (rarity >= 9) return legendaryColor;      // 9-10
        if (rarity >= 7) return epicColor;           // 7-8
        if (rarity >= 5) return rareColor;           // 5-6
        if (rarity >= 3) return uncommonColor;       // 3-4
        return commonColor;                          // 1-2
    }
    
    /// <summary>
    /// Chamado quando o botão é clicado
    /// </summary>
    private void OnButtonClick()
    {
        if (currentUpgrade == null || upgradeManagerStats == null)
        {
            Debug.LogError("Upgrade ou Manager não configurado!");
            return;
        }

        // Som de clique (opcional)
        // AudioManager.Instance.PlaySound("UpgradeSelect");

        // Notifica o manager que este upgrade foi selecionado
        upgradeManagerStats.OnUpgradeSelected(currentUpgrade);
    }
    
    /// <summary>
    /// Adiciona efeito hover no botão (opcional)
    /// </summary>
    public void OnPointerEnter()
    {
        if (selectButton != null)
        {
            // Adiciona feedback visual
            transform.localScale = Vector3.one * 1.05f;
        }
    }
    
    /// <summary>
    /// Remove efeito hover
    /// </summary>
    public void OnPointerExit()
    {
        if (selectButton != null)
        {
            transform.localScale = Vector3.one;
        }
    }
}
