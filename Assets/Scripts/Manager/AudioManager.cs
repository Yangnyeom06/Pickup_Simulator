using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Effect Clips")]
    public AudioClip itemPickupClip;
    public AudioClip itemSellClip;
    public AudioClip jumpClip;
    
    private AudioSource audioSource;

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
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayItemPickup() => PlaySound(itemPickupClip);
    public void PlayItemSell() => PlaySound(itemSellClip);
    public void PlayJump() => PlaySound(jumpClip);
}