using UnityEngine;
using System.Collections;

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

    Animator animator;
    public ManejadorCamara manejadorCamara;
    public ManejadorSonido manejadorSonido;

    void Start()
    {
        animator = GetComponent<Animator>();
        estadoActual = Estado.Inactivo;
    }

    // Llamada desde ManejadorGameplay para activar un estado
    public void ActivarEstado(Estado nuevoEstado)
    {
        estadoActual = nuevoEstado;
        ManejarEstado();
    }

    void ManejarEstado()
    {
        switch (estadoActual)
        {
            case Estado.Inactivo:
                animator.ResetTrigger("GolpeIzquierdo");
                animator.ResetTrigger("GolpeDerecho");
                animator.SetBool("Cubriendose", false);
                animator.SetBool("EsquivarDerecha", false);
                animator.SetBool("EsquivarIzquierda", false);
                break;

            case Estado.GolpeIzquierdo:
                animator.SetTrigger("GolpeIzquierdo");
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoAirePuñetazo);
                manejadorCamara.IniciarShake(0.2f, 0.04f);
                break;

            case Estado.GolpeDerecho:
                animator.SetTrigger("GolpeDerecho");
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoAirePuñetazo);
                manejadorCamara.IniciarShake(0.2f, 0.04f);
                break;

            case Estado.MovimientoDerecha:
                animator.SetBool("EsquivarDerecha", true);
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoDescubrirse);
                manejadorCamara.IniciarZoomTemporal(60f, 0.05f);
                break;

            case Estado.MovimientoIzquierda:
                animator.SetBool("EsquivarIzquierda", true);
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoDescubrirse);
                manejadorCamara.IniciarZoomTemporal(60f, 0.05f);
                break;

            case Estado.Cubrirse:
                // Se ejecuta la animación de cubrirse 1 vez y termina automáticamente
                animator.SetTrigger("AnimacionEjecutandose");
                animator.SetBool("Cubriendose", true);
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoCubrirse);
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoImpactoPuñetazo);
                manejadorCamara.IniciarZoom(65f);
                StartCoroutine(TerminarCubrirse(1f));
                break;

            case Estado.RecibirDaño:
                animator.SetTrigger("RecibirDaño");
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoImpactoPuñetazo);
                manejadorCamara.IniciarShake(0.3f, 0.1f);
                break;
        }
    }

    IEnumerator TerminarCubrirse(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        animator.SetBool("Cubriendose", false);
        estadoActual = Estado.Inactivo;
    }

    public void CubrirseExitoso() => ActivarEstado(Estado.Cubrirse);
    public void GolpearIzquierda() => ActivarEstado(Estado.GolpeIzquierdo);
    public void GolpearDerecha() => ActivarEstado(Estado.GolpeDerecho);
    public void EsquivarIzquierda() => ActivarEstado(Estado.MovimientoIzquierda);
    public void EsquivarDerecha() => ActivarEstado(Estado.MovimientoDerecha);
    public void RecibirDano() => ActivarEstado(Estado.RecibirDaño);
}
