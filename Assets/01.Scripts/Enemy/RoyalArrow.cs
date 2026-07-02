using UnityEngine;

public class RoyalArrow : MonoBehaviour
{
    public float speed = 15f; // 화살 날아가는 속도
    private Vector2 targetDirection;

    // GameClearManager가 화살을 만들 때 목표물을 설정해주는 함수
    public void SetTarget(Vector2 targetPos)
    {
        // 타겟(플레이어)을 향한 방향벡터 구하기
        targetDirection = (targetPos - (Vector2)transform.position).normalized;

        // 화살촉이 날아가는 방향을 쳐다보도록 회전 
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 45f);
    }

    public void Update()
    {
        // 설정된 방향으로 무조건 직진
        transform.Translate(targetDirection * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 주인공에게 치명상을 입혀서 확정 죽음(Death 애니메이션) 처리
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(999);

            Destroy(gameObject);
        }
    }
}