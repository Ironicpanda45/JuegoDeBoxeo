using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public enum Estado
    {
        Inactivo,
        GolpeIzquierdo,
        GolpeDerecho,
        RecibirDaño
    }

    public Estado estadoActual;
    Animator animator;
    public ManejadorSonido manejadorSonido;

    void Start()
    {
        animator = GetComponent<Animator>();
        estadoActual = Estado.Inactivo;
    }

    // ---------------------------
    // Llamadas desde ManejadorGameplay
    // ---------------------------

    // Llamar cuando el jugador falla cualquier nota
    public void JugadorFallo()
    {
        // Siempre golpea
        GolpearAleatorio();
    }

    // Llamar cuando el jugador acierta un golpe (izq o der)
    public void JugadorAcertoGolpe()
    {
        RecibirDaño();
    }

    // Llamar cuando el jugador acierta movimiento o cubrirse
    public void JugadorAcertoMovimientoOCubrirse()
    {
        GolpearAleatorio();
    }

    // ---------------------------
    // Estados internos
    // ---------------------------

    void GolpearAleatorio()
    {
        int v = Random.Range(0, 2);
        if (v == 0) ActivarEstado(Estado.GolpeIzquierdo);
        else ActivarEstado(Estado.GolpeDerecho);
    }

    void RecibirDaño()
    {
        ActivarEstado(Estado.RecibirDaño);
    }

    void ActivarEstado(Estado nuevoEstado)
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
                animator.ResetTrigger("RecibirDaño");
                break;

            case Estado.GolpeIzquierdo:
                animator.SetTrigger("GolpeIzquierdo");
                if (manejadorSonido != null)
                    manejadorSonido.ReproducirSonido(manejadorSonido.sonidoAirePuñetazo);
                break;

            case Estado.GolpeDerecho:
                animator.SetTrigger("GolpeDerecho");
                if (manejadorSonido != null)
                    manejadorSonido.ReproducirSonido(manejadorSonido.sonidoAirePuñetazo);
                break;

            case Estado.RecibirDaño:
                animator.SetTrigger("RecibirDaño");
                if (manejadorSonido != null)
                    manejadorSonido.ReproducirSonido(manejadorSonido.sonidoImpactoPuñetazo);
                break;
        }

        // Opcional: volver a inactivo tras 1 segundo
        Invoke("VolverInactivo", 1f);
    }

    void VolverInactivo()
    {
        estadoActual = Estado.Inactivo;
    }
}
