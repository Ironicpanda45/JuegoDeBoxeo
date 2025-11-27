using UnityEngine;
using UnityEngine.Events;

public class AgregadorDialogo : MonoBehaviour
{
    public string[] frases;
    public SistemaDialogos sistemaDialogos;
    public UnityEvent OnTerminarDialogos;
    public void AgregarNuevosDialogos()
    {
        sistemaDialogos.AgregarNuevosDialogos(frases, OnTerminarDialogos);
    }
}
