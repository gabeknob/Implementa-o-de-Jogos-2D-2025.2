using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UpgradeManagerStats : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private UpgradeData[] allUpgrades; // Todos os 15 upgrades
    [SerializeField] private int upgradesPerLevel = 4; // Quantos mostrar por vez
    
    [Header("UI")]
    [SerializeField] private GameObject upgradePanel; // Painel que aparece ao upar
    [SerializeField] private UpgradeOption[] upgradeOptions; // 4 slots de UI
    
    [Header("Balanceamento")]
    [SerializeField] private bool allowDuplicates = false; // Pode aparecer o mesmo upgrade 2x?
    [SerializeField] private bool useRarityWeight = true; // Usa raridade para peso?
    
    private List<UpgradeData> currentUpgradeSelection = new List<UpgradeData>();
    private int currentPlayerLevel = 1;
    
    public static UpgradeManagerStats Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Esconde o painel no início
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
        
        // Reseta todos os upgrades
        ResetAllUpgrades();
    }
    
    public void OnPlayerLevelUp(int newLevel)
    {
        currentPlayerLevel = newLevel;
        ShowUpgradeOptions();
    }
    
    public void ShowUpgradeOptions()
    {
        // Gera 4 upgrades aleatórios
        currentUpgradeSelection = SelectRandomUpgrades(upgradesPerLevel);
        
        // Se não conseguiu 4 upgrades, algo está errado
        if (currentUpgradeSelection.Count == 0)
        {
            Debug.LogWarning("Nenhum upgrade disponível!");
            return;
        }
        
        // Mostra o painel
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
            Time.timeScale = 0f; // Pausa o jogo
        }
        
        // Atualiza a UI
        for (int i = 0; i < upgradeOptions.Length; i++)
        {
            if (i < currentUpgradeSelection.Count)
            {
                upgradeOptions[i].Setup(currentUpgradeSelection[i], this);
                upgradeOptions[i].gameObject.SetActive(true);
            }
            else
            {
                // Se tiver menos de 4 upgrades disponíveis, esconde os slots vazios
                upgradeOptions[i].gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Seleciona X upgrades aleatórios baseado nas regras
    /// </summary>
    private List<UpgradeData> SelectRandomUpgrades(int count)
    {
        List<UpgradeData> selected = new List<UpgradeData>();
        
        // Filtra upgrades que podem ser oferecidos
        List<UpgradeData> availableUpgrades = allUpgrades
            .Where(u => u.CanBeOffered(currentPlayerLevel))
            .ToList();
        
        if (availableUpgrades.Count == 0)
        {
            Debug.LogWarning("Nenhum upgrade disponível para o nível atual!");
            return selected;
        }
        
        // Seleciona até 'count' upgrades
        int attempts = 0;
        int maxAttempts = 100; // Previne loop infinito
        
        while (selected.Count < count && attempts < maxAttempts)
        {
            UpgradeData randomUpgrade;
            
            if (useRarityWeight)
            {
                randomUpgrade = SelectWeightedRandom(availableUpgrades);
            }
            else
            {
                randomUpgrade = availableUpgrades[Random.Range(0, availableUpgrades.Count)];
            }
            
            // Evita duplicatas se configurado
            if (!allowDuplicates && selected.Contains(randomUpgrade))
            {
                attempts++;
                continue;
            }
            
            selected.Add(randomUpgrade);
            
            // Se não permite duplicatas, remove da lista disponível
            if (!allowDuplicates)
            {
                availableUpgrades.Remove(randomUpgrade);
            }
            
            // Se acabaram os upgrades disponíveis, para
            if (availableUpgrades.Count == 0)
                break;
            
            attempts++;
        }
        
        return selected;
    }
    
    /// <summary>
    /// Seleciona um upgrade aleatório considerando raridade como peso
    /// Upgrades mais raros têm menos chance de aparecer
    /// </summary>
    private UpgradeData SelectWeightedRandom(List<UpgradeData> upgrades)
    {
        // Calcula peso total (inversamente proporcional à raridade)
        // Rarity 1 (comum) = peso 10
        // Rarity 10 (raro) = peso 1
        float totalWeight = 0f;
        foreach (var upgrade in upgrades)
        {
            totalWeight += 11f - upgrade.rarity; // Inverte a raridade
        }
        
        // Seleciona um valor aleatório
        float randomValue = Random.Range(0f, totalWeight);
        
        // Encontra qual upgrade foi selecionado
        float currentWeight = 0f;
        foreach (var upgrade in upgrades)
        {
            currentWeight += 11f - upgrade.rarity;
            if (randomValue <= currentWeight)
            {
                return upgrade;
            }
        }
        
        // Fallback (não deveria chegar aqui)
        return upgrades[0];
    }
    
    /// <summary>
    /// Chame quando o player escolher um upgrade
    /// </summary>
    public void OnUpgradeSelected(UpgradeData upgrade)
    {
        // Incrementa o nível do upgrade
        upgrade.currentLevel++;
        
        // Aplica o efeito do upgrade no player
        ApplyUpgrade(upgrade);
        
        // Fecha o painel
        CloseUpgradePanel();
    }
    
    /// <summary>
    /// Aplica o efeito do upgrade no player
    /// </summary>
    private void ApplyUpgrade(UpgradeData upgrade)
    {
        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats não encontrado!");
            return;
        }
        
        float valueToAdd = upgrade.value;
        
        // Se for porcentagem, calcula baseado no valor atual
        if (upgrade.isPercentage)
        {
            // Exemplo: se dano atual é 10 e upgrade dá +20%, adiciona 2
            // Isso depende de como seu PlayerStats está estruturado
        }
        
        // Aplica baseado no tipo
        switch (upgrade.upgradeType)
        {
            case UpgradeType.MaxHealth:
                playerStats.maxHealth += (int)valueToAdd;
                break;
            
            case UpgradeType.Damage:
                playerStats.damage += (int)valueToAdd;
                break;
            
            case UpgradeType.MoveSpeed:
                playerStats.moveSpeed += valueToAdd;
                break;
            
            case UpgradeType.CriticalChance:
                playerStats.criticalChance += (int)valueToAdd;
                break;
            
            case UpgradeType.CriticalMultiplier:
                playerStats.criticalMultiplier += valueToAdd;
                break;
            
            case UpgradeType.AttackSpeed:
                playerStats.attackSpeed += valueToAdd;
                break;
            
            // Adicione os outros casos conforme seus atributos
            
            default:
                Debug.LogWarning($"Tipo de upgrade não implementado: {upgrade.upgradeType}");
                break;
        }
        
        Debug.Log($"Upgrade aplicado: {upgrade.upgradeName} (Nível {upgrade.currentLevel})");
    }
    
    /// <summary>
    /// Fecha o painel de upgrades e despausa o jogo
    /// </summary>
    public void CloseUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
        
        Time.timeScale = 1f; // Despausa o jogo
    }
    
    /// <summary>
    /// Reseta todos os upgrades (útil ao reiniciar o jogo)
    /// </summary>
    public void ResetAllUpgrades()
    {
        foreach (var upgrade in allUpgrades)
        {
            upgrade.Reset();
        }
        currentPlayerLevel = 1;
    }
    
    /// <summary>
    /// Retorna todos os upgrades que o player já pegou
    /// </summary>
    public List<UpgradeData> GetAcquiredUpgrades()
    {
        return allUpgrades.Where(u => u.currentLevel > 0).ToList();
    }
}
