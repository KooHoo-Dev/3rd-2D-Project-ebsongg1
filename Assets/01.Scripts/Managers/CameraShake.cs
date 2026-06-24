using UnityEngine;
using System.Collections; // 코루틴(시간 지연 기능)

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance; // 싱글톤

    public void Awake()
    {
        instance = this; // 켜자마자
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null; // 잠깐 대기
        }

        transform.localPosition = originalPos;


    }
}
