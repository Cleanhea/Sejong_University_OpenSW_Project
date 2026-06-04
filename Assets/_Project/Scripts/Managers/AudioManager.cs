using UnityEngine;

// 오디오(BGM / 효과음) 재생을 담당하는 싱글톤 매니저
// TODO: 현재는 틀만 작성된 상태. 실제 AudioClip 연결 및 재생 로직은 추후 구현 예정
public class AudioManager : MonoBehaviour
{
    // 볼륨 기본값 (0.0 ~ 1.0)
    private const float DEFAULT_VOLUME = 1f;

    // 싱글톤 인스턴스
    public static AudioManager Instance { get; private set; }

    // TODO: 인스펙터에서 오디오 소스 연결
    // [SerializeField] private AudioSource bgmSource;   // 배경음 전용 (loop)
    // [SerializeField] private AudioSource sfxSource;   // 효과음 전용 (one-shot)

    // TODO: 사용할 오디오 클립 목록 연결
    // [SerializeField] private AudioClip titleBgm;
    // [SerializeField] private AudioClip gameBgm;
    // [SerializeField] private AudioClip hitSfx;

    private void Awake()
    {
        // 중복 인스턴스 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 배경음 재생 (loop 재생)
    public void PlayBGM(AudioClip clip, float volume = DEFAULT_VOLUME)
    {
        // TODO: bgmSource에 clip 할당 후 loop 재생
    }

    // 배경음 정지
    public void StopBGM()
    {
        // TODO: bgmSource.Stop()
    }

    // 효과음 재생 (one-shot)
    public void PlaySFX(AudioClip clip, float volume = DEFAULT_VOLUME)
    {
        // TODO: sfxSource.PlayOneShot(clip, volume)
    }

    // 전체 볼륨 설정 (옵션 UI 연동 예정)
    public void SetMasterVolume(float volume)
    {
        // TODO: AudioListener.volume 또는 AudioMixer 파라미터 조정
    }
}
