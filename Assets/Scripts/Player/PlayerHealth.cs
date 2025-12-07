using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // --- REFERÊNCIAS ---
    private PlayerStats stats;
    private SpriteRenderer spriteRenderer;
    public GameObject gameOverScreen;
    
    [Header("Interface (UI)")]
    public HealthBar healthBar; // <--- ARRASTE A BARRA DE VIDA AQUI NO INSPECTOR

    // --- CONFIGURAÇÕES DE IMPACTO ---
    [Header("Efeitos Visuais")]
    public Material flashMaterial; 
    private Material originalMaterial; 

    [Tooltip("Tempo congelado (Hit Stop)")]
    public float hitStopDuration = 0.15f; 

    // --- CONFIGURAÇÕES DE DEFESA ---
    [Header("Invulnerabilidade")]
    public float iFrameDuration = 1.0f; 
    public int numberOfFlashes = 5;     
    private bool isInvulnerable = false; 

    [Header("Knockback")]
    public float knockbackForce = 10f; 
    public float knockbackRadius = 3f; 

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    void Start()
    {
        // Garante que a barra de vida comece sincronizada com o status atual do player
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(stats.maxHealth, false);
            healthBar.SetHealth(stats.currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        stats.currentHealth -= damage;
        
        // --- ATUALIZAÇÃO DA UI ---
        if (healthBar != null)
        {
            healthBar.TakeDamage(damage); // Usa o método da própria barra para animar
            // OU: healthBar.SetHealth(stats.currentHealth); // Se preferir setar direto
        }

        if (stats.currentHealth <= 0)
        {
            PlayerDeath();
            return;
        }

        StartCoroutine(HitStopRoutine());
        StartCoroutine(InvulnerabilityRoutine());
        ApplyAreaKnockback();
    }

    private IEnumerator HitStopRoutine()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        
        float flashInterval = iFrameDuration / (numberOfFlashes * 2);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.material = flashMaterial;
            yield return new WaitForSeconds(flashInterval);
            
            spriteRenderer.material = originalMaterial;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRenderer.material = originalMaterial;
        isInvulnerable = false;
    }

    private void ApplyAreaKnockback()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, knockbackRadius);
        foreach (Collider2D target in colliders)
        {
            if (target.CompareTag("Enemy"))
            {
                EnemyAI enemyAI = target.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    Vector2 direction = (target.transform.position - transform.position).normalized;
                    enemyAI.ApplyKnockback(direction, knockbackForce);
                }
            }
        }
    }
    
    public void Heal(int amount)
    {
        stats.currentHealth += amount;
        if(stats.currentHealth > stats.maxHealth) stats.currentHealth = stats.maxHealth;
        
        // Atualiza a UI ao curar também!
        if (healthBar != null)
        {
            healthBar.Heal(amount);
        }
    }

    public void PlayerDeath()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
        Destroy(gameObject);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, knockbackRadius);
    }
}