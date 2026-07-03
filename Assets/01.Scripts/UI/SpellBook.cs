using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellBook : MonoBehaviour
{
    [Header("마법책 스킬 종류")]
    public PlayerAttack.MagicType bookMagicType;

    [Header("타이머 연동")]
    public GameObject timerUI;

    [Header("이 책을 먹었을 때 켤 UI")]
    public GameObject myBookUI;

    [Header("책 닫고 나서 띄울 안내 문구")]
    public string messageAfterClose = "새로운 마법 획득! [Q] 키로 장착/해제하세요.";

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
                attackScript.UnlockMagic(bookMagicType); // 스토리 주고 ui 열기
            }

            if (myBookUI != null)
            {
                myBookUI.SetActive(true);
                Time.timeScale = 0f;
            }

            Debug.Log("마법책 획득");
            if (timerUI != null) timerUI.SetActive(true);

            GetComponent<Collider2D>().enabled = false;
            if (GetComponent<SpriteRenderer>() != null)
            {
                GetComponent<SpriteRenderer>().enabled = false;
            }

        }

    }

    public void OnClickCloseBook()
    {
        // 책 덮었으니 시간 다시 1배속으로 복구
        Time.timeScale = 1f;

        // 양피지 UI 화면에서 끄기
        if (myBookUI != null)
        {
            myBookUI.SetActive(false);
        }

        // 레이를 찾아서 하단 메시지 띄우기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerAttack attackScript = player.GetComponent<PlayerAttack>();

            // 빈칸이 아닐 때만 메시지를 띄움
            if (attackScript != null && !string.IsNullOrEmpty(messageAfterClose))
            {
                attackScript.ShowSystemMessage(messageAfterClose);
            }
        }
    }

}
