using UnityEngine;

public class Gem : MonoBehaviour
{
    [Header("SFX")]
    public AudioClip gemEatSFX;

    [Header("보석 설정")]
    public int gemValue = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DataManager.gemScore += gemValue;

            Debug.Log($"보석 획득! 현재 획득 점수 : {gemValue}");

            BGMManager.instance.PlaySFX(gemEatSFX);

            Destroy(gameObject);
        }
    }
}
