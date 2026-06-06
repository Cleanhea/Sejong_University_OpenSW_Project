using UnityEngine;

public class BgmManager : MonoBehaviour
{
    public static BgmManager Instance { get; private set;}

    [Header("Audio Components")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip bgmClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (bgmSource == null)
        {
            bgmSource = GetComponent<AudioSource>();
            if(bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    void Start()
    {
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        if(bgmClip != null)
        {
            PlayBgm(bgmClip);
        }
    }

    public void PlayBgm(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void SetVolume(float volume)
    {
        if(bgmSource != null)
        {
            bgmSource.volume = volume;
        }
    }

    public float GetVolume()
    {
        return bgmSource != null ? bgmSource.volume : 0.5f;
    }
}
