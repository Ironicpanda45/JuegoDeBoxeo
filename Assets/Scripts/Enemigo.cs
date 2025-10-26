using UnityEngine;

public class Enemigo : MonoBehaviour
{
    int golpes;
    enum Estado
    {
        Inactivo,
        GolpeIzquierdo,
        GolpeDerecho,
        Cubrirse,
        MovimientoDerecha,
        MovimientoIzquierda,
        RecibirDaño
    }

    Estado estadoActual;
    Estado estadoAnterior;
    bool puedeCambiarEstado;

    public Animator animator;
    public ManejadorSonido manejadorSonido;

    void Start()
    {
        estadoActual = Estado.Inactivo;
        estadoAnterior = estadoActual;
        puedeCambiarEstado = true;
    }

    void Update()
    {
        if (puedeCambiarEstado)
        {
            ChecarCondiciones();
        }

        if (estadoActual != estadoAnterior)
        {
            ManejarEstado();
            estadoAnterior = estadoActual;
        }

        if (golpes >= 10)
        {
            manejadorSonido.ReproducirSonido(manejadorSonido.sonidoCampana);
            Destroy(this.gameObject);
        }
    }

    void ChecarCondiciones()
    {
        if (Input.GetButtonDown("Golpe1"))
        {
            manejadorSonido.ReproducirSonido(manejadorSonido.sonidoImpactoPuñetazo);
            estadoActual = Estado.RecibirDaño;
            golpes++;
        }
        else if (Input.GetButtonDown("Golpe2"))
        {
            manejadorSonido.ReproducirSonido(manejadorSonido.sonidoImpactoPuñetazo);
            estadoActual = Estado.RecibirDaño;
            golpes++;
        }
        else
        {
            estadoActual = Estado.Inactivo;
        }
    }

    void ManejarEstado()
    {
        switch (estadoActual)
        {
            case Estado.Inactivo:
                break;
            case Estado.RecibirDaño:
                animator.SetTrigger("RecibirDaño");
                break;
        }
    }
    public void DesbloquearEstado()
    {
        puedeCambiarEstado = true;
        estadoActual = Estado.Inactivo;
        estadoAnterior = Estado.Inactivo;
    }
}
