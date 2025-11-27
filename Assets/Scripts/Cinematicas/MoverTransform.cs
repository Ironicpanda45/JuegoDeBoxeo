using UnityEngine;

public class MoverTransform : MonoBehaviour
{
    public void ColocarCamaraPrincipal()
    {
        Camera camaraPrincipal = Camera.main;
        if (camaraPrincipal != null)
        {
            camaraPrincipal.transform.position = this.transform.position;
            camaraPrincipal.transform.rotation = this.transform.rotation;
            string nombreObjeto = this.gameObject.name;
        }
    }
}