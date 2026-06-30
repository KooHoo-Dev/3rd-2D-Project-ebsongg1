using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float speed = 7f; 
    public int damage = 2;   

    private Vector2 moveDirection;

    public void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Vector3 targetPos = player.transform.position;
            targetPos.y += 0.5f;

            moveDirection = (targetPos - transform.position).normalized;
        }
        else
        {
            moveDirection = transform.right;
        }
    }

    public void Update()
    {
        //운석은 무조건 아래로 떨어짐
        transform.Translate(Vector2.down * speed * Time.deltaTime, Space.World);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject); 
        }
     
        else if (other.CompareTag("Ground")) 
        {
            Destroy(gameObject);
        }
        
    }
}
