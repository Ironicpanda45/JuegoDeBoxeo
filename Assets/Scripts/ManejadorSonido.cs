using UnityEngine;

public class ManejadorSonido : MonoBehaviour
{
    public AudioClip sonidoImpactoPu�etazo;
    public AudioClip sonidoAirePu�etazo;
    public AudioClip sonidoCubrirse;
    public AudioClip sonidoDescubrirse;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ReproducirSonido(AudioClip clip, float volumen = 1.0f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volumen);
        }
    }
}