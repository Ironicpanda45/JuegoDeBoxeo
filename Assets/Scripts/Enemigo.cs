using System;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public enum Estado
    {
        Inactivo,
        GolpeIzquierdo,
        GolpeDerecho,
        Cubrirse,
        MovimientoDerecha,
        MovimientoIzquierda,
        RecibirDaño
    }

    public Estado estadoActual;
    Estado estadoAnterior;

    public Animator animator;

    public ManejadorSonido manejadorSonido;
    public ManejadorGameplay manejadorGameplay;
    public Jugador jugador;

    float tiempoRestanteCooldown;
    float retrasoDecision = 0.2f;

    float estaminaActual = 100f;
    float estaminaMaxima = 100f;
    float tasaRegeneracion = 40f;
    float retrasoRegeneracion = 1f;
    float tiempoUltimoUsoEstamina;

    float costoGolpe = 12.5f;
    float costoCubrirse = 50f;

    public bool EstaCubierto;
    public int direccionCubierta = 0;

    bool RecibeDaño = false;
    float contadorDecision;

    // fallos intencionales
    float probabilidadFallarBloqueo = 0.25f;
    float probabilidadFallarGolpe = 0.15f;

    void Start()
    {
        estadoActual = Estado.Inactivo;
        estadoAnterior = estadoActual;

        estaminaActual = estaminaMaxima;
        tiempoUltimoUsoEstamina = Time.time;

        contadorDecision = retrasoDecision;
    }

    void Update()
    {
        if (RecibeDaño)
        {
            estadoActual = Estado.RecibirDaño;
            RecibeDaño = false;
        }

        // cooldown
        if (tiempoRestanteCooldown > 0)
        {
            tiempoRestanteCooldown -= Time.deltaTime;
        }

        // stamina
        if (estadoActual == Estado.Cubrirse)
        {
            if (estaminaActual <= 1)
            {
                estaminaActual = 0;
                estadoActual = Estado.Inactivo;
                tiempoUltimoUsoEstamina = Time.time;
            }
            else
            {
                estaminaActual -= costoCubrirse * Time.deltaTime;
                estaminaActual = Mathf.Clamp(estaminaActual, 0, estaminaMaxima);
                tiempoUltimoUsoEstamina = Time.time;
            }
        }
        else
        {
            if (Time.time > tiempoUltimoUsoEstamina + retrasoRegeneracion &&
                estaminaActual < estaminaMaxima &&
                estadoActual != Estado.Cubrirse)
            {
                estaminaActual += tasaRegeneracion * Time.deltaTime;
                estaminaActual = Mathf.Clamp(estaminaActual, 0, estaminaMaxima);
            }
        }

        // cubierta activa
        if (estadoActual == Estado.Cubrirse ||
            estadoActual == Estado.MovimientoDerecha ||
            estadoActual == Estado.MovimientoIzquierda)
        {
            EstaCubierto = true;
        }
        else
        {
            EstaCubierto = false;
            direccionCubierta = 0;
        }

        // IA
        contadorDecision -= Time.deltaTime;
        if (contadorDecision <= 0)
        {
            DecidirAccion();
            contadorDecision = retrasoDecision;
        }

        if (estadoActual != estadoAnterior)
        {
            ManejarEstado();
            estadoAnterior = estadoActual;
        }
    }

    void DecidirAccion()
    {
        if (tiempoRestanteCooldown > 0) return;

        // leer al jugador de forma directa
        bool jugadorCubierto = jugador.EstaCubierto;
        int direccionJugador = jugador.direccionCubierta;

        // decidir golpear
        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
        {
            if (UnityEngine.Random.Range(0f, 1f) > probabilidadFallarGolpe &&
                estaminaActual >= costoGolpe)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    estadoActual = Estado.GolpeIzquierdo;
                }
                else
                {
                    estadoActual = Estado.GolpeDerecho;
                }
                return;
            }
        }

        // decidir cubrirse
        if (jugadorCubierto == false)
        {
            if (UnityEngine.Random.Range(0f, 1f) > probabilidadFallarBloqueo &&
                estaminaActual > costoCubrirse / 2)
            {
                estadoActual = Estado.Cubrirse;
                direccionCubierta = 2;
                return;
            }
        }

        // decidir esquivar
        if (UnityEngine.Random.Range(0f, 1f) < 0.3f)
        {
            int lado = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;

            if (lado == -1)
            {
                estadoActual = Estado.MovimientoIzquierda;
                direccionCubierta = -1;
            }
            else
            {
                estadoActual = Estado.MovimientoDerecha;
                direccionCubierta = 1;
            }
            return;
        }

        estadoActual = Estado.Inactivo;
    }

    void ManejarEstado()
    {
        switch (estadoActual)
        {
            case Estado.Inactivo:
                animator.SetBool("Cubriendose", false);
                direccionCubierta = 0;
                break;

            case Estado.GolpeIzquierdo:
                estaminaActual -= costoGolpe;
                tiempoUltimoUsoEstamina = Time.time;
                tiempoRestanteCooldown = 0.15f;
                animator.SetTrigger("GolpeIzquierdo");
                manejadorGameplay.RegistrarGolpe(this.gameObject);
                break;

            case Estado.GolpeDerecho:
                estaminaActual -= costoGolpe;
                tiempoUltimoUsoEstamina = Time.time;
                tiempoRestanteCooldown = 0.15f;
                animator.SetTrigger("GolpeDerecho");
                manejadorGameplay.RegistrarGolpe(this.gameObject);
                break;

            case Estado.Cubrirse:
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoCubrirse);
                animator.SetBool("Cubriendose", true);
                break;

            case Estado.MovimientoDerecha:
                tiempoRestanteCooldown = 0.5f;
                tiempoUltimoUsoEstamina = Time.time;
                direccionCubierta = 1;
                animator.SetBool("Cubriendose", true);
                animator.SetTrigger("AnimacionEjecutandose");
                break;

            case Estado.MovimientoIzquierda:
                tiempoRestanteCooldown = 0.5f;
                tiempoUltimoUsoEstamina = Time.time;
                direccionCubierta = -1;
                animator.SetBool("Cubriendose", true);
                animator.SetTrigger("AnimacionEjecutandose");
                break;

            case Estado.RecibirDaño:
                tiempoRestanteCooldown = 0.5f;
                animator.SetTrigger("RecibirDaño");
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoImpactoPuñetazo);
                break;
        }
    }
    public void DesbloquearEstado()
    {
        estadoActual = Estado.Inactivo;
        estadoAnterior = Estado.Inactivo;
    }
    public void RecibirDanoDeJugador()
    {
        RecibeDaño = true;
    }

    public void Golpear()
    {
        manejadorGameplay.RegistrarGolpe(this.gameObject);
    }

    public void CubrirseExitoso()
    {
        manejadorSonido.ReproducirSonido(manejadorSonido.sonidoImpactoPuñetazo);
        RecibeDaño = false;
    }
}