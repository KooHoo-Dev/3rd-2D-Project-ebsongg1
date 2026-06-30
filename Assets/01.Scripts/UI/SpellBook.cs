using UnityEngine;
using UnityEngine.UI;

public class SpellBook : MonoBehaviour
{
    [Header("마법책 스킬 종류")]
    public PlayerAttack.MagicType bookMagicType;

    [Header("타이머 연동")]
    public GameObject timerUI;

    [Header("이 책의 스토리 내용")]
    [TextArea(3, 10)] // 엔터 치면서 여러 줄을 편하게 쓰게 해줌
    public string myLoreText;

    public void Start()
    {
        if (timerUI != null) timerUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerAttack attackScript = collision.GetComponent<PlayerAttack>();

            if (attackScript != null )
            {
                attackScript.UnlockMagic(bookMagicType);

                attackScript.OpenLoreBook(myLoreText); // 스토리 주고 ui 열기
            }

            Debug.Log("마법책 획득");
            if (timerUI != null) timerUI.SetActive(true);

            Destroy(gameObject);

        }

    }
}
