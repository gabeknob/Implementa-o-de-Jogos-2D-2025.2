using UnityEngine;

public enum SkillBehaviorType
{
    projectile,
    orbiting,
    groundArea,
    aoe,
    chain,
    summonMinion,
    selfBuff,
}
//para adicionar a opção no menu Assets> Create> Game> ComboRecipe
[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/SimpleSkill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public GameObject effectPrefab;
    public float cooldown;
    public int maxCharges;
    [Header("Visual")]
    public Sprite icon;
    
    [Header("Configurações Específicas")]
    public int damage;
    [Tooltip("Quantidade de ''pulos'' (Raio)")]
    public int bounceCount;
    [Tooltip("Distância dos ''pulos'' (Raio)")]
    public float bounceRange;
    [Tooltip("Duração de Buff (Luz)")] 
    public float buffDuration;  
    [Tooltip("Taxa do Buff (Luz)")]
    public float moveSpeedMultiplier; // 1.5 = +50% Velocidade

    public SkillBehaviorType behaviorType;
    //
}