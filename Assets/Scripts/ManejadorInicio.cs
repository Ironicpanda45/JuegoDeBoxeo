using UnityEngine;
using UnityEngine.UI;


public class ManejadorInicio : MonoBehaviour
{


    public GameObject menuOpciones;
    public GameObject menuPrincipal;
    public GameObject menuCreditos;

    void Start()
    {
        
    }

    public void AbrirPanelOpciones()
    {
        menuPrincipal.SetActive(false);
        menuOpciones.SetActive(true);
    }

    public void AbrirMenuPrincipal()
    {
        menuPrincipal.SetActive(true);
        menuOpciones.SetActive(false);
        menuCreditos.SetActive(false);
    }

    public void AbrirMenuCreditos()
    {
        menuPrincipal.SetActive(false);
        menuCreditos.SetActive(true);
    }
}
