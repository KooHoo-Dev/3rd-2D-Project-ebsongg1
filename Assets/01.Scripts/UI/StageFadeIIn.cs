using System.Collections;
using UnityEngine;

public class StageFadeIn : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.alpha = 1f; // 1. 완전히 까만 상태로 시작!
            StartCoroutine(FadeInRoutine());
        }
    }

    IEnumerator FadeInRoutine()
    {
        float timer = 3f;
        // 2. 1초 동안 서서히 투명하게 만들기
        while (timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;
            canvasGroup.alpha = timer;
            yield return null;
        }

        // 3. 페이드가 끝나면 클릭을 방해하지 않게 오브젝트 끄기
        canvasGroup.gameObject.SetActive(false);
    }
}
