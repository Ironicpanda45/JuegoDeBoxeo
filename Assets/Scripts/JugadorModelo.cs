using UnityEngine;

public class JugadorModelo : MonoBehaviour
{
    Jugador jugadorScript;
    Enemigo enemigoScript;

    void Start()
    {
        jugadorScript = GetComponentInParent<Jugador>();
        if (jugadorScript == null)
        {
            enemigoScript = GetComponentInParent<Enemigo>();
        }
    }

    public void EjecutarFuncionGolpe()
    {
        jugadorScript.Golpear();
    }
}
