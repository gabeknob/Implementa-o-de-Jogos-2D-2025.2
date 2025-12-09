using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class AutoAttackController : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private bool isShooting = true;
    public float coolDownShoot;
    public Vector2 directionNormalized;
    public int attackDamage;
    public float attackSpeed;
    void Start()
    {
        StartCoroutine(FireRoutine());
    }
    void Update() 
    {
        
    }
    IEnumerator FireRoutine()
    {
        while (isShooting)
        {
            float closestDistance = float.PositiveInfinity;
        EnemyAI closestEnemy = null;

        // Adicione verificação de null aqui
        if(EnemyManager.allEnemies != null) 
        {
            foreach (var enemy in EnemyManager.allEnemies)
            {
                if (enemy == null) continue; // Pula inimigos mortos que ainda estão na lista

                float enemyDistance = Vector2.Distance(this.gameObject.transform.position, enemy.transform.position);
                if(enemyDistance < closestDistance)
                {
                    closestDistance = enemyDistance;
                    closestEnemy = enemy; // Já é EnemyAI, não precisa de GetComponent novamente
                }
            }
        }
            
            if(closestEnemy != null && closestDistance<8f)
            {
                GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);

                Vector2 direction = closestEnemy.transform.position - transform.position;
                directionNormalized = direction.normalized;
                
                newProjectile.GetComponent<ProjectileMovement>().Setup(
                    directionNormalized, 
                    attackSpeed, 
                    attackDamage, 
                    0, 
                    0f
                );
            }
            yield return new WaitForSeconds(coolDownShoot);
        }
    }
}
