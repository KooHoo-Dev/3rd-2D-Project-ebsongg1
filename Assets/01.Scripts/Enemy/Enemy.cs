using UnityEngine;
using System.Collections; // 코루틴

public class Enemy : MonoBehaviour
{
    public int hp = 3; // 마법 3방
    private Animator anim;
    private SpriteRenderer sr;

    [Header("드롭할 아이템 세팅")]
    public GameObject dropItemPrefap;

    public void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log("남은 체력:" +  hp);

        CameraShake.instance.Shake(0.1f, 0.2f);

        StartCoroutine(FlashCoroutine());

        if (hp <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("Hit");
        }
    }

    IEnumerator FlashCoroutine()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    public void Die()
    {
        Debug.Log("적 쓰러짐!");

        anim.SetTrigger("Die");

        DataManager.killCount++;

        GetComponent<Collider2D>().enabled = false; // 죽은 적을 계속 때리는 걸 방지, 유령 상태
        this.enabled = false; // 스크립트 정지

        if ( dropItemPrefap != null )
        {
            Instantiate(dropItemPrefap, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 2f);
    }
}
