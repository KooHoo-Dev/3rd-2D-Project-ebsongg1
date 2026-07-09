using UnityEngine;

public class GhostProjectile : MonoBehaviour
{
    public float speed = 10f; // 날아가는 속도
    public int damage = 1;   

    // 날아갈 고정된 궤도 (방향)
    private Vector2 moveDirection;

    public void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // (목표 위치 - 내 위치) = 목표를 향하는 방향
            // (.normalized를 붙이면 속도는 빼고 순수한 '방향' 데이터만 쏙 뽑아낼 수 있음)

            // 플레이어의 중심부를 정확히 노리기 위해 위치를 살짝 위로 보정 
            Vector3 targetPos = player.transform.position;
            targetPos.y += 0.5f; // 몸통 높이쯤으로 조준점 올리기

            moveDirection = (targetPos - transform.position).normalized;
        }
        else
        {
            // 혹시 죽었거나 없으면 그냥 바라보던 앞쪽(오른쪽/왼쪽)으로 날아감
            moveDirection = transform.right;
        }
    }

    public void Update()
    {
        // 쏠 때 정해진 'moveDirection' 방향으로만 똑바로 날아감 (유도탄 아님)
        // (Space.World를 써야 보스가 뒤집혀도 우주 기준(절대 좌표)으로 정확히 날아감)
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject); 
        }
        
        /*
        else if (other.CompareTag("Ground")) 
        {
            Destroy(gameObject);
        }
        */
    }
}
