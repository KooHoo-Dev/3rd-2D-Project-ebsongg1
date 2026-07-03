using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("화면에 띄울 안내 문구")]
    public string message = "[Space] : 점프 [Z] : 공격";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAttack attackScript = other.GetComponent<PlayerAttack>();

            if (attackScript != null)
            {
                attackScript.ShowSystemMessage(message);
            }

            Destroy(gameObject);
        }
    }
}
