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
            Debug.Log("Cámara movida a la posición del objeto: " + nombreObjeto);
        }
        else
        {
            Debug.LogError("No se encontró la Main Camera. Asegúrate de que una cámara en la escena tenga el tag \"MainCamera\".");
        }
    }
}