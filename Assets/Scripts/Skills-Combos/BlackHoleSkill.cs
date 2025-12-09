using UnityEngine;

public class BlackHoleSkill : MonoBehaviour
{
    private float pullForce;
    private float radius;
    private int damage;
    private float duration;

    public void Setup(float durationTime, float force, float range, int dmg)
    {
        duration = durationTime;
        
        pullForce = Mathf.Abs(force); 
        
        radius = range;
        damage = dmg;

        Destroy(gameObject, duration);
    }

    void FixedUpdate()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D target in targets)
        {
            if (target.CompareTag("Player") || target.gameObject == gameObject) 
                continue;

            if (!target.CompareTag("Enemy"))
                continue;

            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();

            if (targetRb != null)
            {
                Vector2 directionToCenter = (transform.position - target.transform.position).normalized;
                
                float distance = Vector2.Distance(transform.position, target.transform.position);

                if (distance > 0.5f)
                {
                    targetRb.AddForce(directionToCenter * pullForce, ForceMode2D.Force);
                }
                else
                {
                    targetRb.linearVelocity = Vector2.Lerp(targetRb.linearVelocity, Vector2.zero, Time.fixedDeltaTime * 5f);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0, 0.5f, 0.3f); // Roxo
        Gizmos.DrawWireSphere(transform.position, radius > 0 ? radius : 3f);
    }
}