using UnityEngine;

public class ManejadorGameplay : MonoBehaviour
{
    public Jugador jugador;
    public Enemigo enemigo;

    public float partidaPorcentaje = 50f;
    float danoPorGolpe = 2f;

    void Start()
    {
        partidaPorcentaje = 50f;
    }

    public void RegistrarGolpe(GameObject golpeador)
    {
        bool EsElJugador = (golpeador == jugador.gameObject);

        GameObject objetivo;
        bool targetEstaCubierto;
        int targetDireccionCubierta = 0;

        int direccionAtaque = 0;

        if (EsElJugador)
        {
            objetivo = enemigo.gameObject;
            targetEstaCubierto = enemigo.EstaCubierto;
            targetDireccionCubierta = enemigo.direccionCubierta;

            if (jugador.estadoActual == Jugador.Estado.GolpeIzquierdo)
            {
                direccionAtaque = -1;
            }
            else if (jugador.estadoActual == Jugador.Estado.GolpeDerecho) 
            {
                direccionAtaque = 1; 
            }
        }
        else
        {
            objetivo = jugador.gameObject;
            targetEstaCubierto = jugador.EstaCubierto;
            targetDireccionCubierta = jugador.direccionCubierta;

            if (enemigo.estadoActual == Enemigo.Estado.GolpeIzquierdo)
            {
                direccionAtaque = -1;
            }
            else if (enemigo.estadoActual == Enemigo.Estado.GolpeDerecho) 
            {
                direccionAtaque = 1; 
            }
        }

        bool golpeAcierta = true;

        if (targetEstaCubierto)
        {
            if (targetDireccionCubierta == 2)
            {
                golpeAcierta = false;
            }
            else
            {
                if (targetDireccionCubierta == direccionAtaque && direccionAtaque != 0)
                {
                    golpeAcierta = true;
                }
                else
                {
                    golpeAcierta = false;
                }
            }
        }
        else
        {
            golpeAcierta = true;
        }

        if (golpeAcierta)
        {
            if (EsElJugador)
            {
                partidaPorcentaje -= danoPorGolpe;
                enemigo.RecibirDanoDeJugador();
            }
            else
            {
                partidaPorcentaje += danoPorGolpe;
                jugador.RecibirDanoDeEnemigo();
            }
        }
        else
        {
            if (EsElJugador)
            {
                enemigo.CubrirseExitoso();
            }
            else
            {
                jugador.CubrirseExitoso();
            }
        }

        partidaPorcentaje = Mathf.Clamp(partidaPorcentaje, 0f, 100f);

        if (partidaPorcentaje <= 0f)
        {
            Destroy(objetivo);
        }
        else if (partidaPorcentaje >= 100f)
        {
            Destroy(objetivo);
        }
    }
}