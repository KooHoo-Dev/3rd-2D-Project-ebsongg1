using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 3; // 마법 3방
    private Animator anim;

    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log("남은 체력:" +  hp);

        if (hp <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("Hit");
        }
    }

    public void Die()
    {
        Debug.Log("적 쓰러짐!");

        anim.SetTrigger("Die");

        GetComponent<Collider2D>().enabled = false; // 죽은 적을 계속 때리는 걸 방지, 유령 상태
        this.enabled = false; // 스크립트 정지

        Destroy(gameObject, 2f);
    }
}
