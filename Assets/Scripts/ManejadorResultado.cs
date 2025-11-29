using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ManejadorResultado : MonoBehaviour
{
    [Header("Datos Manuales")]
    public int rondasJugador = 0;
    public int rondasEnemigo = 0;

    [Header("Siguiente Escena")]
    public string escenaSiguiente = "";

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetButtonDown("Siguiente"))
        {
            if (!string.IsNullOrEmpty(escenaSiguiente))
                SceneManager.LoadScene(escenaSiguiente);
        }
    }
}
