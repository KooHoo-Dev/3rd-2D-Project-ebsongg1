using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    [Header("카메라 세팅")]
    public Transform target;
    public float smoothSpeed = 5f; // 쫓아가는 속도
    public Vector3 offset = new Vector3(0f, 1f, -10f); // 카메라와 캐릭터의 거리

    [Header("보스전 카메라 세팅")]
    public float bossSmoothSpeed = 2f;

    private bool isBossMode = false;
    private Vector3 bossRoomPosition;

    // 시야 조절을 위한 변수들
    private Camera cam;
    private float defaultZoom;
    private float targetZoom;

    public void Start()
    {
        // 내 몸(카메라)에 있는 Camera 부품을 가져와서 기본 줌(Size)을 기억해둠
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            defaultZoom = cam.orthographicSize;
            targetZoom = defaultZoom;
        }
    }

    public void LateUpdate()
    {
        // 1. 위치 이동 (보스 모드 vs 일반 모드)
        if (isBossMode)
        {
            // 보스방 중앙으로 부드럽게 이동 (Z축은 -10 유지)
            Vector3 desiredPosition = bossRoomPosition + new Vector3(0, 0, offset.z);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, bossSmoothSpeed * Time.deltaTime);
        }
        else if (target != null)
        {
            // 평소처럼 레이 쫓아가기
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }

        // 2. 줌인/줌아웃 (카메라 사이즈 부드럽게 조절)
        if (cam != null)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, bossSmoothSpeed * Time.deltaTime);
        }
    }

    // 보스전 시작(BossAI에서 불러줄 함수)
    public void StartBossMode(Vector3 centerPos, float zoomOutAmount)
    {
        isBossMode = true;
        bossRoomPosition = centerPos;
        targetZoom = zoomOutAmount;
    }

    // 보스전 끝(BossHealth에서 불러줄 함수)
    public void EndBossMode()
    {
        isBossMode = false;
        targetZoom = defaultZoom; // 다시 원래 줌으로 복귀
    }
}
