using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("마법 발사 세팅")]
    public Transform firePoint; // 마법이 튀어나올 위치
    public GameObject magicPrefab;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        Instantiate(magicPrefab, firePoint.position, firePoint.rotation); // 설정해둔 위치에 프리팹 복제
    }
}
