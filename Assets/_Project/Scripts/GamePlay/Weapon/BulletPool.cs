using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;     // 풀이 관리할 총알 프리팹
    [SerializeField] private int defaultCapacity = 32; // 초기 예약 크기
    [SerializeField] private int maxSize = 200;        // 풀 상한
    [SerializeField] private bool collectionCheck = true; // 중복 반납 검출 (디버그)

    private ObjectPool<Bullet> pool;  // 내부 풀

    void Awake()
    {
        pool = new ObjectPool<Bullet>(
            createFunc: CreateBullet,
            actionOnGet: OnGetBullet,
            actionOnRelease: OnReleaseBullet,
            actionOnDestroy: OnDestroyBullet,
            collectionCheck: collectionCheck,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    void OnDestroy()
    {
        // 1. 씬 종료 시 풀에 남은 비활성 인스턴스를 정리합니다.
        pool?.Clear();
    }

    /// <summary>지정한 위치와 회전으로 풀에서 총알을 꺼내 활성화합니다.</summary>
    /// <param name="position">발사 시작 위치입니다.</param>
    /// <param name="rotation">초기 회전값입니다.</param>
    /// <returns>활성화된 총알 인스턴스를 반환합니다.</returns>
    public Bullet Get(Vector3 position, Quaternion rotation)
    {
        Bullet bullet = pool.Get();
        bullet.transform.SetPositionAndRotation(position, rotation);
        return bullet;
    }

    // 풀이 비었을 때 새 총알을 생성하고 반납 콜백을 바인딩합니다.
    private Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, transform);
        bullet.Bind(ReleaseBullet);
        return bullet;
    }

    // 풀에서 꺼낼 때 활성화합니다.
    private void OnGetBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    // 풀로 반납될 때 비활성화합니다.
    private void OnReleaseBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    // 풀 상한을 초과해 실제 파괴될 때의 처리입니다.
    private void OnDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    // Bullet이 자신을 반납할 때 호출하는 내부 진입점입니다.
    private void ReleaseBullet(Bullet bullet)
    {
        pool.Release(bullet);
    }
}
