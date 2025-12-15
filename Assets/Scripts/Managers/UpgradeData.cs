using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Game/Upgrade")]
public class UpgradeData : ScriptableObject
{
    [Header("Visual")]
    public string upgradeName = "Nome do Upgrade";
    [TextArea(2, 4)]
    public string description = "Descrição do que o upgrade faz";
    public Sprite icon;
    
    [Header("Tipo de Upgrade")]
    public UpgradeType upgradeType;
    
    [Header("Valores")]
    public float value = 1f; // Valor do upgrade (ex: +10% de dano, +5 de vida)
    public bool isPercentage = false; // Se true, value é %, se false é valor fixo
    
    [Header("Balanceamento")]
    public int maxLevel = 5; // Quantas vezes pode ser escolhido
    [Range(1, 10)]
    public int rarity = 1; // 1 = comum, 10 = raro (afeta chance de aparecer)
    
    [Header("Requisitos (Opcional)")]
    public int minimumPlayerLevel = 1; // Level mínimo do player para aparecer
    public UpgradeData[] requiredUpgrades; // Upgrades necessários antes deste
    
    // Rastreamento
    [HideInInspector] public int currentLevel = 0; // Quantas vezes foi escolhido
    
    public bool CanBeOffered(int playerLevel)
    {
        // Já está no nível máximo?
        if (currentLevel >= maxLevel)
            return false;
        
        // Player tem o nível mínimo?
        if (playerLevel < minimumPlayerLevel)
            return false;
        
        // Tem os upgrades necessários?
        if (requiredUpgrades != null && requiredUpgrades.Length > 0)
        {
            foreach (var required in requiredUpgrades)
            {
                if (required.currentLevel == 0)
                    return false;
            }
        }
        
        return true;
    }
    
    public void Reset()
    {
        currentLevel = 0;
    }
}

public enum UpgradeType
{
    // Atributos Base
    MaxHealth,          // Vida máxima
    HealthRegen,        // Regeneração de vida
    MoveSpeed,          // Velocidade de movimento
    
    // Dano
    Damage,             // Dano base
    CriticalChance,     // Chance de crítico
    CriticalMultiplier, // Multiplicador de crítico
    AttackSpeed,        // Velocidade de ataque
    
    // Defesa
    Armor,              // Armadura
    DamageReduction,    // Redução de dano
    
    // Utilidade
    PickupRange,        // Alcance de coleta de itens
    ExpGain,            // Ganho de experiência
    Cooldown,           // Tempo de recarga das habilidades
    
    // Armas
    ProjectileCount,    // Quantidade de projéteis
    ProjectileSpeed,    // Velocidade dos projéteis
    ProjectilePierce,   // Perfuração de projéteis
    AreaSize            // Tamanho da área de efeito
}
