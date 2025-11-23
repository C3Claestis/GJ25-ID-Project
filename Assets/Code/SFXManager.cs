using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Audio Source untuk SFX")]
    [SerializeField] private AudioSource audioSource;

    [Header("List SFX")]
    [SerializeField] private AudioClip[] sfxClips;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Jika audioSource belum diset, ambil dari komponen
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Play SFX berdasarkan index AudioClip
    /// </summary>
    public void PlaySFX(int index, float volume = 1f)
    {
        if (index < 0 || index >= sfxClips.Length)
        {
            Debug.LogWarning($"SFX index {index} tidak ada!");
            return;
        }

        audioSource.PlayOneShot(sfxClips[index], volume);
    }

    /// <summary>
    /// Play SFX berdasarkan nama clip
    /// </summary>
    public void PlaySFX(string clipName, float volume = 1f)
    {
        foreach (AudioClip clip in sfxClips)
        {
            if (clip != null && clip.name == clipName)
            {
                audioSource.PlayOneShot(clip, volume);
                return;
            }
        }

        Debug.LogWarning($"SFX '{clipName}' tidak ditemukan!");
    }
}
