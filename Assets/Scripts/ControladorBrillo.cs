using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ControladorBrillo : MonoBehaviour
{
    public Slider sliderBrillo;
    public float sliderValue;
    public Image panelBrillo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sliderBrillo.value = PlayerPrefs.GetFloat("brillo", 0.5f);
        
        panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, sliderBrillo.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CambiarBrillo(float valor)
    {
        sliderValue = valor;
        PlayerPrefs.SetFloat("brillo", sliderValue);
        panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, sliderBrillo.value);
    }
}
