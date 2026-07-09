using UnityEngine;

public class PenetrateProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public GameObject hitEffectPrefab; 

    public void Start()
    {
        Rigidbody2D rbody = GetComponent<Rigidbody2D>();

        rbody.linearVelocity = transform.right * speed;

        Destroy(gameObject, 3f);
    }

    public void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player")) return;

        Enemy enemy = hitInfo.GetComponent<Enemy>(); 
        BossHealth boss = hitInfo.GetComponent<BossHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        if (boss != null)
        {
            boss.TakeDamage(damage);
        }

        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, transform.rotation);

            Destroy(effect, 1f);
        }

        
    }
}
