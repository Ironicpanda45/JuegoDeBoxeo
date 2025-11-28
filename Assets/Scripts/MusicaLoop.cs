using UnityEngine;


public class MusicaLoop : MonoBehaviour
{
    public AudioClip clipIntro;

    public AudioClip clipBucle;

    private AudioSource ComponenteAudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ComponenteAudioSource = GetComponent<AudioSource>();

        ComponenteAudioSource.clip = clipBucle;
        ComponenteAudioSource.loop = true;

        ComponenteAudioSource.PlayOneShot(clipIntro);

        double duracionIntro = (double)clipIntro.samples / clipIntro.frequency;
        ComponenteAudioSource.PlayScheduled(AudioSettings.dspTime + duracionIntro);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
