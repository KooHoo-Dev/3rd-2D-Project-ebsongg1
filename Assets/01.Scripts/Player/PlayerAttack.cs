using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public enum MagicType
    {
        Basic,
        Penetrate,
        Meteor
    }

    [Header("마법 발사 세팅")]
    public Transform firePoint; // 마법이 튀어나올 위치
    public GameObject magicPrefab;

    [Header("마법 스킬 선택")]
    public MagicType currentMagic = MagicType.Basic;

    [Header("쿨타임 세팅")]
    public float attackCoolTime = 1.0f;
    private float currentCoolTime = 0f;
    

    public void Update()
    {
        if (currentCoolTime > 0)
        {
            currentCoolTime -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Z) && currentCoolTime <=0)
        {
            Shoot();
            Attack();
        }
    }

    public void Attack()
    {

    }

    public void Shoot()
    {
        Instantiate(magicPrefab, firePoint.position, firePoint.rotation); // 설정해둔 위치에 프리팹 복제
    }

    public void ChangeMagic(MagicType newMagic)
    {
        currentMagic = newMagic;
        Debug.Log($"마법 각성! 주인공의 현재 스킬이 {currentMagic}으로 변경 됨");
    }
}
