using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

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
    public GameObject basicPrefab;
    public GameObject penetratePrefab;
    public GameObject meteorPrefab;

    [Header("마법 스킬 선택")]
    public MagicType currentMagic = MagicType.Basic;

    [Header("쿨타임 세팅")]
    public float basicCoolTime = 0.5f;
    public float penetrateCoolTime = 5f;
    public float meteorCoolTime = 10f;

    private float currentCoolTime = 0f;

    [Header("해금 상태")]
    public bool hasPenetrate = false;
    public bool hasMeteor = false;

    [Header("운석 세팅")]
    public int meteorCount = 10;
    

    public void Update()
    {
        if (currentCoolTime > 0)
        {
            currentCoolTime -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentMagic = MagicType.Basic;
            currentCoolTime = 0f;
            Debug.Log("무기 교체 : 기본 마법(R)");
        }

        if (Input.GetKeyDown(KeyCode.Q) && hasPenetrate == true)
        {
            currentMagic = MagicType.Penetrate;
            currentCoolTime = 0f;
            Debug.Log("무기 교체 : 관통 마법(Q)");
        }

        if (Input.GetKeyDown(KeyCode.E) && hasMeteor == true)
        {
            currentMagic = MagicType.Meteor;
            currentCoolTime = 0f;
            Debug.Log("무기 교체 : 연사 마법(E)");
        }

        if (Input.GetKeyDown(KeyCode.Z) && currentCoolTime <=0)
        {
            Attack();
        }
    }

    public void Attack()
    {
        switch(currentMagic)
        {
            case MagicType.Basic:
                Shoot();
                currentCoolTime = basicCoolTime;
                break;

            case MagicType.Penetrate:
            ShootPenetrate();
            currentCoolTime = penetrateCoolTime;
            break;

            case MagicType.Meteor:
            ShootMeteor();
            currentCoolTime = meteorCoolTime;
            break;
        }
    }

    public void Shoot()
    {
        Instantiate(basicPrefab, firePoint.position, firePoint.rotation); // 설정해둔 위치에 프리팹 복제
    }

    public void ShootPenetrate()
    {
        if (penetratePrefab != null && firePoint != null)
        {
            Instantiate(penetratePrefab, firePoint.position, firePoint.rotation);
        }
    }

    public void ShootMeteor()
    {
        StartCoroutine(DropMeteors());
    }

    IEnumerator DropMeteors()
    {
        for (int i = 0; i  < meteorCount; i++)
        {
            float targetX = firePoint.position.x + (firePoint.right.x * 5f);
            float randomX = targetX + Random.Range(-5f, 5f);
            float randomY = transform.position.y + 10f + Random.Range(0f, 2f); // 위에서 떨어지게
            Vector3 spawnPos = new Vector3(randomX, randomY, 0);

            Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void UnlockMagic(MagicType newMagic)
    {
        if ( newMagic == MagicType.Penetrate )
        {
            hasPenetrate = true;
        }
        if ( newMagic == MagicType.Meteor )
        {
            hasMeteor = true;
        }

        currentMagic = newMagic;
        Debug.Log($"마법 각성! {currentMagic}이 영구 해금!");
    }
}
