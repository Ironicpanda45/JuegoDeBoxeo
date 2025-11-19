using UnityEngine;

public class ModeloPersonaje : MonoBehaviour
{
    void Start()
    {
    }

    public void RegistrarGolpe()
    {
        Jugador jugador = GetComponentInParent<Jugador>();

        if (jugador != null)
        {
            jugador.Golpear();
            return;
        }

        Enemigo enemigo = GetComponentInParent<Enemigo>();

        if (enemigo != null)
        {
            enemigo.Golpear();
            return;
        }
    }
}