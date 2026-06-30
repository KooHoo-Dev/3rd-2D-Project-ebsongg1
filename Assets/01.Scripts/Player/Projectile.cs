using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public GameObject hitEffectPrefab; // 이펙트 빈칸

    public void Start()
    {
        Rigidbody2D rbody = GetComponent<Rigidbody2D>();

        rbody.linearVelocity = transform.right * speed;

        Destroy(gameObject, 3f);
    }

    public void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player")) return;

        Enemy enemy = hitInfo.GetComponent<Enemy>(); // enemy를 가지고 있는지 확인
        BossHealth boss = hitInfo.GetComponent<BossHealth>();

        if ( enemy != null )
        {
            enemy.TakeDamage(damage);
        }

        if (boss != null)
        {
            boss.TakeDamage(damage);
        }

        if ( hitEffectPrefab != null )
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, transform.rotation);

            Destroy(effect, 1f);
        }

        Destroy(gameObject);
    }
}
