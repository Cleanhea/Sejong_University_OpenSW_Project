using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("추적 설정")]
    [Tooltip("비워두면 Start()에서 'Player' 태그로 자동 탐색")]
    public Transform target = null;

    public Vector3 offset = new Vector3(0, 0, -10);

    [Range(1f, 20f)]
    [Tooltip("카메라 추적 속도 (높을수록 빠르게 따라감)")]
    public float followSpeed = 5f;

    [Header("카메라 크기 자동 맞춤")]
    [Tooltip("켜면 타겟 Bounds에 맞춰 카메라 크기를 자동 조정")]
    public bool autoFit = true;

    [Range(0f, 1f)]
    public float padding = 1f;

    [Tooltip("매 프레임 크기 재조정 (움직이는/크기 변하는 오브젝트)")]
    public bool updateFitEveryFrame = false;

    [Tooltip("화면 비율 변화 감지 시 자동 재조정")]
    public bool detectAspectChange = true;

    public float cameraSize = 10f;

    // ── 내부 변수 ──────────────────────────────────────────
    private Camera _cam;
    private float _lastAspect;

    // ───────────────────────────────────────────────────────
    void Start()
    {
        _cam = GetComponent<Camera>();
        _lastAspect = _cam.aspect;

        // target이 Inspector에서 지정되지 않았으면 태그로 탐색
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                Debug.LogWarning("[CameraController] 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }

        if (autoFit) FitCamera();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // ── 1. 카메라 추적 ──
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        // ── 2. 카메라 크기 조정 ──
        if (!autoFit) return;

        if (updateFitEveryFrame)
        {
            FitCamera();
        }
        else if (detectAspectChange && !Mathf.Approximately(_cam.aspect, _lastAspect))
        {
            _lastAspect = _cam.aspect;
            FitCamera();
        }
    }

    // ── 카메라 크기 맞춤 ──────────────────────────────────

    [ContextMenu("카메라 맞추기 (지금 실행)")]
    public void FitCamera()
    {
        Bounds bounds = GetTargetBounds();
        if (bounds.size == Vector3.zero) return;

        if (_cam.orthographic)
            FitOrthographic(bounds);
        else
            FitPerspective(bounds);
    }

    void FitOrthographic(Bounds bounds)
    {
        float verticalSize = bounds.extents.y;
        float horizontalSize = bounds.extents.x / _cam.aspect;

        float size = Mathf.Max(verticalSize, horizontalSize);
        _cam.orthographicSize = size * (1f + padding) * cameraSize;
    }

    void FitPerspective(Bounds bounds)
    {
        float halfFovRad = _cam.fieldOfView * 0.5f * Mathf.Deg2Rad;

        float distV = bounds.extents.y / Mathf.Tan(halfFovRad);
        float distH = bounds.extents.x / (_cam.aspect * Mathf.Tan(halfFovRad));

        float distance = Mathf.Max(distV, distH) * (1f + padding);

        // offset의 Z값을 거리 기준으로 덮어씀 (추적 위치에 반영)
        offset.z = -distance - bounds.extents.z;
    }

    // ── Bounds 계산 ────────────────────────────────────────

    Bounds GetTargetBounds()
    {
        if (target == null) return new Bounds();

        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("[CameraController] Renderer를 찾을 수 없습니다.");
            return new Bounds();
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        return bounds;
    }

    // ── 에디터 시각화 ──────────────────────────────────────
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Bounds b = GetTargetBounds();
        if (b.size == Vector3.zero) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(b.center, b.size);
    }
#endif
}