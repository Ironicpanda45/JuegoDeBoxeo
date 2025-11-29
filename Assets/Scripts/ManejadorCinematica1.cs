using UnityEngine;
using System.Collections;

public class ManejadorCinematica1 : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    public Camera camara;
    public CambiarEstadoAnimado animControl;

    [Header("Animación inicial")]
    public string animacionInicial = "Idle";

    [Header("Configuración de movimientos")]
    public Vector3 offsetCamara = new Vector3(0, 2, -5);
    public float tiempoAcercamiento = 2f;

    [Header("Escena siguiente")]
    public string escenaSiguiente = "NombreEscena";

    void Start()
    {
        if (camara == null) camara = Camera.main;

        if (animControl != null && !string.IsNullOrEmpty(animacionInicial))
        {
            animControl.PlayAnimacion(animacionInicial);
        }

        StartCoroutine(AnimarCinematica());
    }

    IEnumerator AnimarCinematica()
    {
        // Movimiento de cámara
        Vector3 posInicialCamara = camara.transform.position;
        Vector3 posObjetivoCamara = jugador.position + offsetCamara;

        float tiempo = 0f;
        while (tiempo < tiempoAcercamiento)
        {
            tiempo += Time.deltaTime;
            camara.transform.position = Vector3.Lerp(posInicialCamara, posObjetivoCamara, tiempo / tiempoAcercamiento);
            yield return null;
        }

        camara.transform.position = posObjetivoCamara;

        yield return new WaitForSeconds(0.5f);

        // --- FADE ---
        TransicionesFade transicion = FindFirstObjectByType<TransicionesFade>();
        if (transicion != null)
        {
            transicion.CambiarEscena(escenaSiguiente);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(escenaSiguiente);
        }
    }
}
