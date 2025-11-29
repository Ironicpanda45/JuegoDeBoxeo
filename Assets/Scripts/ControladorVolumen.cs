using UnityEngine;
using UnityEngine.UI;

public class ControladorVolumen : MonoBehaviour
{
    [SerializeField] Slider sliderVolumen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PlayerPrefs.HasKey("volumenMusica"))
        {
            PlayerPrefs.SetFloat("volumenMusica", 1);
            Cargar();
        }
        else
        {
            Cargar();
        }
    }

    public void CambiarVolumen()
    {
        AudioListener.volume = sliderVolumen.value;
        Guardar();
    }
    
    private void Cargar()
    {
        sliderVolumen.value = PlayerPrefs.GetFloat("volumenMusica");
    }

    private void Guardar()
    {
        PlayerPrefs.SetFloat("volumenMusica", sliderVolumen.value);
    }
}
