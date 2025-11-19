using System;
using UnityEngine;

public class Jugador : MonoBehaviour
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
    public ManejadorCamara manejadorCamara;

    float tiempoRestanteCooldown;
    float bufferTiempoTotal = 0.4f;
    float bufferContador;
    Estado inputBuffer;

    public float estaminaActual = 100f;
    public float estaminaMaxima = 100f;
    float tasaRegeneracion = 40f;
    float retrasoRegeneracion = 1f;
    float tiempoUltimoUsoEstamina;

    float costoGolpe = 12.5f;
    float costoCubrirse = 50f;

    bool CubrirseActivoBoton = false;
    bool RecibeDaño = false;

    public ManejadorGameplay manejadorGameplay;
    public bool EstaCubierto;

    public int direccionCubierta = 0;

    void Start()
    {
        estadoActual = Estado.Inactivo;
        estadoAnterior = estadoActual;
        tiempoRestanteCooldown = 0;
        bufferContador = 0;
        inputBuffer = Estado.Inactivo;
        estaminaActual = estaminaMaxima;
        tiempoUltimoUsoEstamina = Time.time;
        direccionCubierta = 0;
    }

    void Update()
    {
        // Cooldown
        if (tiempoRestanteCooldown > 0)
        {
            tiempoRestanteCooldown -= Time.deltaTime;
            if (tiempoRestanteCooldown <= 0)
            {
                ProcesarBuffer();
            }
        }

        // Buffer
        if (bufferContador > 0)
        {
            bufferContador -= Time.deltaTime;
            if (bufferContador <= 0)
            {
                inputBuffer = Estado.Inactivo;
            }
        }

        // Consumo de stamina
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
            if (Time.time > tiempoUltimoUsoEstamina + retrasoRegeneracion && estaminaActual < estaminaMaxima && estadoActual != Estado.Cubrirse && !CubrirseActivoBoton)
            {
                estaminaActual += tasaRegeneracion * Time.deltaTime;
                estaminaActual = Mathf.Clamp(estaminaActual, 0, estaminaMaxima);
            }
        }

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

        ChecarCondiciones();

        // Transición
        if (estadoActual != estadoAnterior)
        {
            if (estadoAnterior == Estado.Cubrirse && estadoActual != Estado.Cubrirse)
            {
                manejadorCamara.IniciarRetornoZoomSuave();
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoDescubrirse);
            }
            ManejarEstado();
            estadoAnterior = estadoActual;
        }
    }

    void ChecarCondiciones()
    {
        if (RecibeDaño == true)
        {
            estadoActual = Estado.RecibirDaño;
            RecibeDaño = false;
            return;
        }

        if (estadoActual == Estado.Cubrirse)
        {
            if (!Input.GetButton("Cubrirse"))
            {
                estadoActual = Estado.Inactivo;
            }
            else
            {
                if (Input.GetButton("MoverIzquierda"))
                    direccionCubierta = -1;
                else if (Input.GetButton("MoverDerecha"))
                    direccionCubierta = 1;
                else
                    direccionCubierta = 2;
            }
            return;
        }

        // PRIORIDAD 2: ACCIONES DE EVENTO (Usan Buffer)
        else if (Input.GetButtonDown("Golpe1"))
        {
            if (estaminaActual > 0)
            {
                RegistrarInput(Estado.GolpeIzquierdo);
            }
        }
        else if (Input.GetButtonDown("Golpe2"))
        {
            if (estaminaActual > 0)
            {
                RegistrarInput(Estado.GolpeDerecho);
            }
        }
        else if (Input.GetButtonDown("MoverDerecha"))
        {
            RegistrarInput(Estado.MovimientoDerecha);
            direccionCubierta = -1;
        }
        else if (Input.GetButtonDown("MoverIzquierda"))
        {
            RegistrarInput(Estado.MovimientoIzquierda);
            direccionCubierta = 1;
        }

        // PRIORIDAD 3: ACCIONES DE ESTADO O INACTIVIDAD
        else if (tiempoRestanteCooldown <= 0)
        {
            if (Input.GetButton("Cubrirse"))
            {
                if (estaminaActual > 0)
                {
                    estadoActual = Estado.Cubrirse;
                    CubrirseActivoBoton = true;
                }
            }
            else
            {
                estadoActual = Estado.Inactivo;
                CubrirseActivoBoton = false;
            }
        }
    }

    void RegistrarInput(Estado estadoDeseado)
    {
        if (tiempoRestanteCooldown <= 0)
        {
            estadoActual = estadoDeseado;
        }
        else
        {
            inputBuffer = estadoDeseado;
            bufferContador = bufferTiempoTotal;
        }
    }

    void ProcesarBuffer()
    {
        if (inputBuffer != Estado.Inactivo && bufferContador > 0)
        {
            estadoActual = inputBuffer;
            inputBuffer = Estado.Inactivo;
            bufferContador = 0;
        }
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
                manejadorCamara.RestaurarCamara();
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoAirePuñetazo);
                animator.SetTrigger("GolpeIzquierdo");
                manejadorCamara.IniciarShake(0.2f, 0.04f);
                break;

            case Estado.GolpeDerecho:
                estaminaActual -= costoGolpe;
                tiempoUltimoUsoEstamina = Time.time;
                tiempoRestanteCooldown = 0.15f;
                manejadorCamara.RestaurarCamara();
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoAirePuñetazo);
                animator.SetTrigger("GolpeDerecho");
                manejadorCamara.IniciarShake(0.2f, 0.04f);
                break;

            case Estado.Cubrirse:
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoCubrirse);
                animator.SetTrigger("AnimacionEjecutandose");
                animator.SetBool("Cubriendose", true);
                manejadorCamara.IniciarZoom(65f);
                break;

            case Estado.MovimientoDerecha:
                tiempoRestanteCooldown = 0.5f;
                tiempoUltimoUsoEstamina = Time.time;
                direccionCubierta = 1;
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoDescubrirse);
                manejadorCamara.IniciarZoomTemporal(60f, 0.05f);
                animator.SetTrigger("AnimacionEjecutandose");
                animator.SetBool("Cubriendose", true);
                break;

            case Estado.MovimientoIzquierda:
                tiempoRestanteCooldown = 0.5f;
                tiempoUltimoUsoEstamina = Time.time;
                direccionCubierta = -1;
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoDescubrirse);
                manejadorCamara.IniciarZoomTemporal(60f, 0.05f);
                animator.SetTrigger("AnimacionEjecutandose");
                animator.SetBool("Cubriendose", true);
                break;

            case Estado.RecibirDaño:
                tiempoRestanteCooldown = 0.5f;
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoImpactoPuñetazo);
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoAirePuñetazo);
                animator.SetTrigger("RecibirDaño");
                manejadorCamara.IniciarShake(0.3f, 0.1f);
                break;
        }
    }

    public void DesbloquearEstado()
    {
        estadoActual = Estado.Inactivo;
        estadoAnterior = Estado.Inactivo;
    }
    public void RecibirDanoDeEnemigo()
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
