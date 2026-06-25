using UnityEngine;

public class Gem : MonoBehaviour
{
    [Header("보석 설정")]
    public int gemValue = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"보석 획득! 현재 획득량 : {gemValue}");

            Destroy(gameObject);
        }
    }
}
