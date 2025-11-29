using UnityEngine;

public class CambiarEstadoAnimado : MonoBehaviour
{
    public Animator anim;

    public void PlayAnimacion(string nombreAnimacion)
    {
        anim.Play(nombreAnimacion);
    }
}