using UnityEngine;

public class ManejadorSonido : MonoBehaviour
{
    public AudioClip sonidoImpactoPuñetazo;
    public AudioClip sonidoAirePuñetazo;
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