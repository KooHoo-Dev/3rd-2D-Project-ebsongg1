using UnityEngine;

public class Meteor : MonoBehaviour
{
    [Header("운석 세팅")]
    public float fallSpeed = 10f;
    public int damage = 1;
    public float lifeTime = 3f;

    public void Start()
    {
        // 소멸 예약만 남기고 복잡한 계산은 싹 제거
        Destroy(gameObject, lifeTime);
    }

    public void Update()
    {
        // 무조건 아래로 낙하
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}