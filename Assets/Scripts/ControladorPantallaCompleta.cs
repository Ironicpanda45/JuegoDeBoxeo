using UnityEngine;
using UnityEngine.UI;

public class ControladorPantallaCompleta : MonoBehaviour
{
    public Toggle pantallaCompleta;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Screen.fullScreen)
        {
            pantallaCompleta.isOn = true;
        }
        else
        {
            pantallaCompleta.isOn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivarPantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }
}
