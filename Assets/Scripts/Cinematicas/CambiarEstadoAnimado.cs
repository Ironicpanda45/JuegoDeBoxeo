using System;
using UnityEngine;

public class CambiarEstadoAnimado : MonoBehaviour
{
    public Animator anim;

    public void SetEstado0() { anim.SetInteger("Estado", 0); }
    public void SetEstado1() { anim.SetInteger("Estado", 1); }
    public void SetEstado2() { anim.SetInteger("Estado", 2); }
    public void SetEstado3() { anim.SetInteger("Estado", 3); }
    public void SetEstado4() { anim.SetInteger("Estado", 4); }
    public void SetEstado5() { anim.SetInteger("Estado", 5); }
}