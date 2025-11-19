using UnityEngine;

public class JugadorModelo : MonoBehaviour
{
    private Jugador jugadorScript;

    void Start()
    {
        scriptPadre = GetComponentInParent<Jugador>();
        if (scriptPadre == null)
        {
            scriptPadre = GetComponentInParent<Enemigo>();
        }
    }

    // Esta es la función que será llamada por el Animation Event
    public void EjecutarFuncionGolpe()
    {
        jugadorScript.Golpear();
    }
}
