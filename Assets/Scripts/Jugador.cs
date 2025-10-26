using System;
using UnityEngine;

public class Jugador : MonoBehaviour
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

    float posicionHorizontal;
    float posicionVertical;
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
        posicionHorizontal = Input.GetAxisRaw("Horizontal");
        posicionVertical = Input.GetAxisRaw("Vertical");

        if (puedeCambiarEstado)
        {
            ChecarCondiciones();
        }

        if (estadoActual != estadoAnterior)
        {
            if (estadoAnterior == Estado.Cubrirse && estadoActual != Estado.Cubrirse)
            {
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoDescubrirse);
            }
            ManejarEstado();
            estadoAnterior = estadoActual;
        }
    }

    void ChecarCondiciones()
    {
        if (Input.GetButtonDown("Golpe1"))
        {
            estadoActual = Estado.GolpeIzquierdo;
        }
        else if (Input.GetButtonDown("Golpe2"))
        {
            estadoActual = Estado.GolpeDerecho;
        }
        else if (posicionVertical == -1)
        {
            estadoActual = Estado.Cubrirse;
        }
        else if (posicionHorizontal == 1)
        {
            estadoActual = Estado.MovimientoDerecha;
        }
        else if (posicionHorizontal == -1)
        {
            estadoActual = Estado.MovimientoIzquierda;
        }
        else if (Input.GetButtonDown("Jump"))
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
                animator.SetBool("Cubriendose", false);
                break;

            case Estado.GolpeIzquierdo:
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoAirePuñetazo);
                animator.SetTrigger("GolpeIzquierdo");
                break;

            case Estado.GolpeDerecho:
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoAirePuñetazo);
                animator.SetTrigger("GolpeDerecho");
                break;

            case Estado.Cubrirse:
                manejadorSonido.ReproducirSonido(manejadorSonido.sonidoCubrirse);
                animator.SetTrigger("AnimacionEjecutandose");
                animator.SetBool("Cubriendose", true);
                break;

            case Estado.MovimientoDerecha:
                transform.position += new Vector3(0.1f, 0, 0);
                break;

            case Estado.MovimientoIzquierda:
                transform.position += new Vector3(-0.1f, 0, 0);
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
