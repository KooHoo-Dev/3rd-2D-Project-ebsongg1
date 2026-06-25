using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    [Header("카메라 세팅")]
    public Transform target;
    public float smoothSpeed = 5f; // 쫓아가는 속도
    public Vector3 offset = new Vector3(0f, 1f, -10f); // 카메라와 캐릭터의 거리

    public void LateUpdate()
    {
        if(target != null)
        {
            Vector3 desirePosition = target.position + offset; 
            Vector3 smoothedPosition = Vector3.Lerp(target.position, desirePosition, smoothSpeed * Time.deltaTime);

            transform.position = smoothedPosition;
        }
    }
}
