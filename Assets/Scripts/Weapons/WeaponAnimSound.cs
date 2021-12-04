using UnityEngine;

public class WeaponAnimSound : MonoBehaviour
{
    private AudioSource aSource;

    void Start()
    {
        aSource = transform.root.GetComponent<AudioSource>();
    }

    public void PlayEventSound(AudioClip audioToPlay)
    {
        aSource.PlayOneShot(audioToPlay);
    }
}
