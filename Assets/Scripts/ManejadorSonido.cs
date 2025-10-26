using UnityEngine;

public class ManejadorSonido : MonoBehaviour
{
    public AudioClip sonidoImpactoPu�etazo;
    public AudioClip sonidoAirePu�etazo;
    public AudioClip sonidoCubrirse;
    public AudioClip sonidoDescubrirse;
    public AudioClip sonidoCampana;
    float variacionPitch = 0.1f;

    public void ReproducirSonido(AudioClip clip, float volumen = 1.0f)
    {
        float PitchAleatorio = Random.Range(1f - variacionPitch, 1f + variacionPitch);
        AudioSource.PlayClipAtPoint(clip, this.transform.position, volumen);
        GameObject tempAudioClip = new GameObject("tempAudioClip " + clip.name);
        tempAudioClip.transform.position = this.transform.position;
        AudioSource tempAudio = tempAudioClip.AddComponent<AudioSource>();
        tempAudio.clip = clip;
        tempAudio.volume = volumen;
        tempAudio.pitch = PitchAleatorio;
        tempAudio.Play();
        Destroy(tempAudioClip, clip.length / PitchAleatorio);
    }
}