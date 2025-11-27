using System;
using UnityEngine;

public class CambiarEstadoAnimado : MonoBehaviour
{
    public Animator anim;

    public void SetEstado0() { anim.SetInteger("Estado", 0); }
    public void SetEstado1() { anim.SetInteger("Estado", 1); }
    public void SetEstado2() { anim.SetInteger("Estado", 2); }
}