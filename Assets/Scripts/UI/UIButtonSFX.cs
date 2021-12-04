using UnityEngine;

public class UIButtonSFX : MonoBehaviour
{
    private AudioSource fxSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private void Start()
    {
        fxSource = GetComponent<AudioSource>();
    }

    public void PlayHoverSound()
    {
        if(fxSource != null)
            fxSource.PlayOneShot(hoverSound);
    }

    public void PlayClickSound()
    {
        if (fxSource != null)
            fxSource.PlayOneShot(clickSound);
    }
}
