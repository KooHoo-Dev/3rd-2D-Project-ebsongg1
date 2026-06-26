using UnityEngine;

public class SpellBook : MonoBehaviour
{
    [Header("마법책 스킬 종류")]
    public PlayerAttack.MagicType bookMagicType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerAttack attackScript = collision.GetComponent<PlayerAttack>();

            if (attackScript != null )
            {
                attackScript.UnlockMagic(bookMagicType);
            }

            Debug.Log("마법책 획득");
            Destroy(gameObject);

        }

    }
}
