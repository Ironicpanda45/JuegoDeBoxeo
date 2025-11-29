using UnityEngine;

public class AnimacionAlInicio : MonoBehaviour
{
    public Animator anim;
    public string animacionInicial = "Idle";

    void Start()
    {
        if (anim != null && !string.IsNullOrEmpty(animacionInicial))
        {
            anim.Play(animacionInicial);
        }
    }
}
