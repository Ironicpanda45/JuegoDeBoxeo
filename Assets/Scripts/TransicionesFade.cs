using UnityEngine;
using UnityEngine.SceneManagement;

public class TransicionesFade : MonoBehaviour
{
    public CanvasGroup panelNegro;
    public float duracion = 1f;

    bool haciendoFade = false;
    float tiempo = 0f;
    string escenaObjetivo = "";

    void Update()
    {
        if (!haciendoFade) return;

        tiempo += Time.deltaTime;
        panelNegro.alpha = tiempo / duracion;

        if (tiempo >= duracion)
        {
            SceneManager.LoadScene(escenaObjetivo);
        }
    }

    public void CambiarEscena(string nombre)
    {
        escenaObjetivo = nombre;
        tiempo = 0f;
        haciendoFade = true;
    }
}
