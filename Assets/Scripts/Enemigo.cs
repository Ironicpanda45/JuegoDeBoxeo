using UnityEngine;

public class Enemigo : MonoBehaviour
{
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
    }

    void ChecarCondiciones()
    {
        if (Input.GetButtonDown("Golpe1"))
        {
            estadoActual = Estado.RecibirDaño;
        }
        else if (Input.GetButtonDown("Golpe2"))
        {
            estadoActual = Estado.RecibirDaño;
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
