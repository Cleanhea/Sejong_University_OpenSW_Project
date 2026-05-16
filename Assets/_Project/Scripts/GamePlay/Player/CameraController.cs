using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform target = null;
    Vector3 offset = new Vector3(0, 0, -10); // 카메라와 타겟 간의 거리
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform; // "Player" 태그를 가진 게임 오브젝트를 찾아 타겟으로 설정
    }
    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5); // 카메라가 타겟을 부드럽게 따라가도록 보간
    }
}
