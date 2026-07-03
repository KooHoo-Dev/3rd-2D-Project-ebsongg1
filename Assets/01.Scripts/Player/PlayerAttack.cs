using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

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

    [Header("마법별 실시간 쿨타임 초시계")]
    private float basicTimer = 0f;
    private float penetrateTimer = 0f;
    private float meteorTimer = 0f;

    [Header("쿨타임 UI 연동")]
    public Image penetrateShadow;  
    public Image meteorShadow;


    [Header("해금 상태")]
    public bool hasPenetrate = false;
    public bool hasMeteor = false;

    [Header("운석 세팅")]
    public int meteorCount = 15;

    [Header("시스템 메세지 UI")]
    public GameObject systemMessageUI; // 하단 대화창
    public TextMeshProUGUI systemMessageText;
    private Animator anim;

    public void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }


    public void Update()
    {
        if (basicTimer > 0f) basicTimer -= Time.deltaTime;
        if (penetrateTimer > 0f) penetrateTimer -= Time.deltaTime;
        if (meteorTimer > 0f) meteorTimer -= Time.deltaTime;

        UpdateSkillUI();

        if (Input.GetKeyDown(KeyCode.Q) && hasPenetrate == true)
        {

            if (currentMagic == MagicType.Penetrate)
            {
                currentMagic = MagicType.Basic; // 이미 관통이면 기본으로 해제
                ShowSystemMessage("마법 해제 : 기본 상태");
            }
            else
            {
                currentMagic = MagicType.Penetrate; // 아니면 관통으로 장착
                ShowSystemMessage("마법 장착 : 관통 마법 (Q)");
            }

            Debug.Log("무기 교체 : 관통 마법(Q)");
        }

        if (Input.GetKeyDown(KeyCode.E) && hasMeteor == true)
        {
            if (currentMagic == MagicType.Meteor)
            {
                currentMagic = MagicType.Basic; 
                ShowSystemMessage("마법 해제 : 기본 상태");
            }
            else
            {
                currentMagic = MagicType.Meteor; 
                ShowSystemMessage("마법 장착 : 운석 마법 (E)");
            }
            
            Debug.Log("무기 교체 : 운석 마법(E)");
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Attack();
        }
    }

    public void UpdateSkillUI()
    {
        penetrateShadow.fillAmount = penetrateTimer / penetrateCoolTime;

        meteorShadow.fillAmount = meteorTimer / meteorCoolTime;
    }


    public void Attack()
    {
        switch(currentMagic)
        {
            case MagicType.Basic:
                // 기본 마법 초시계가 다 돌았는지 검사
                if (basicTimer <= 0f)
                {
                    Shoot();
                    basicTimer = basicCoolTime; // 공용 말고 전용 초시계에 쿨타임 세팅

                    if (anim != null) anim.SetTrigger("Attack");
                }
                break;

            case MagicType.Penetrate:
                if (penetrateTimer <= 0f)
                {
                    ShootPenetrate();
                    penetrateTimer = penetrateCoolTime;

                    if (anim != null) anim.SetTrigger("Attack");
                }
                break;

            case MagicType.Meteor:
                if (meteorTimer <= 0f)
                {
                    ShootMeteor();
                    meteorTimer = meteorCoolTime;

                    if (anim != null) anim.SetTrigger("Attack");
                }
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
            float randomX = transform.position.x + Random.Range(-10f, 10f);
            float randomY = transform.position.y + 7f + Random.Range(0f, 2f); // 위에서 떨어지게
            Vector3 spawnPos = new Vector3(randomX, randomY, 0);

            Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(0.08f);
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

        Debug.Log($"마법 각성! {currentMagic}이 영구 해금!");
    }


    public void ShowSystemMessage(string msg)
    {
        // 대화창의 글씨를 넘겨받은 문구로 바꿈
        if (systemMessageText != null)
        {
            systemMessageText.text = msg;
        }

        // 2. 이미 대화창이 떠있을 때 또 메시지가 오면 꼬일 수 있으니, 기존 초시계를 끄고 새로 켬
        StopCoroutine("SystemMessageRoutine");
        StartCoroutine("SystemMessageRoutine");
    }

    IEnumerator SystemMessageRoutine() // 대화창 껐다 키기
    {
        systemMessageUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        systemMessageUI.SetActive(false);
    }

}
