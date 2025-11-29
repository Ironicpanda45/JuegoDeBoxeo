using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransicionesFade : MonoBehaviour
{
    public Image panelNegro;
    public float duracion = 1f;

    bool haciendoFadeOut = false;
    bool haciendoFadeIn = true;
    float tiempo = 0f;
    string escenaObjetivo = "";

    void Start()
    {
        panelNegro.color = Color.black;
    }

    void Update()
    {
        if (haciendoFadeIn)
        {
            tiempo += Time.deltaTime;
            panelNegro.color = new Color(0f, 0f, 0f, 1f - Mathf.Clamp01(tiempo / duracion));

            if (tiempo >= duracion)
            {
                haciendoFadeIn = false;
                tiempo = 0f;
            }
        }
        else if (haciendoFadeOut)
        {
            tiempo += Time.deltaTime;
            panelNegro.color = new Color(0f, 0f, 0f, Mathf.Clamp01(tiempo / duracion));

            if (tiempo >= duracion)
            {
                SceneManager.LoadScene(escenaObjetivo);
            }
        }
    }

    public void CambiarEscena(string nombre)
    {
        escenaObjetivo = nombre;
        tiempo = 0f;
        haciendoFadeOut = true;
    }
}
