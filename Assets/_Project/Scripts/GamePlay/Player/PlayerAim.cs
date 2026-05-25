using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;   // 플레이어 상태
    [SerializeField] private Transform aimIndicator;  // 회전시킬 자식

    public Vector2 AimDirection { get; private set; } = Vector2.right;  // 조준 방향
    public float AimAngleDeg { get; private set; }                      // 조준 각도

    private Camera mainCam;  // 카메라 캐시

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // 1. 사망 시 조준 갱신을 중단합니다 (넉백 중에는 계속 갱신합니다).
        if (playerStat.playerdead) return;

        // 2. 마우스 스크린 좌표를 월드 좌표로 변환합니다.
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(mouseScreen);

        // 3. 플레이어에서 마우스를 향하는 방향 벡터를 계산합니다.
        Vector2 toMouse = (Vector2)mouseWorld - (Vector2)transform.position;

        // 4. 마우스가 플레이어와 거의 겹쳤을 때는 직전 조준 방향을 유지합니다.
        if (toMouse.sqrMagnitude < 0.0001f) return;

        // 5. 조준 방향과 각도를 갱신합니다.
        AimDirection = toMouse.normalized;
        AimAngleDeg = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;

        // 6. 인디케이터가 지정되어 있으면 조준 방향으로 회전시킵니다.
        if (aimIndicator != null)
            aimIndicator.rotation = Quaternion.Euler(0f, 0f, AimAngleDeg);
        
    }
}
