using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SistemaDialogos : MonoBehaviour
{
    public TMP_Text textoDialogo;
    float contadorAgregarLetra;
    public string[] frases;
    string texto;
    int letra_actual;
    int frase_actual;
    public GameObject cuadroSiguiente;

    public UnityEvent OnTerminarDialogos;

    public ManejadorSonido manejadorSonido;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cuadroSiguiente.SetActive(false);
        frase_actual = 0;
        letra_actual = 0;
        contadorAgregarLetra = 0;
        texto = "";

    }

    // Update is called once per frame
    void Update()
    {
        if (frase_actual < frases.Length)
        {
            if (letra_actual < frases[frase_actual].Length)
            {
                contadorAgregarLetra += Time.deltaTime;
                if (contadorAgregarLetra >= 0.1)
                {
                    texto += frases[frase_actual][letra_actual];
                    contadorAgregarLetra = 0;
                    letra_actual += 1;
                    textoDialogo.text = texto;
                    manejadorSonido?.ReproducirSonido(manejadorSonido.sonidoDialogo);
                }
                if (Input.GetButtonDown("Siguiente"))
                {
                    texto = frases[frase_actual];
                    letra_actual = frases[frase_actual].Length;
                    textoDialogo.text = texto;
                }
            }
            else
            {
                cuadroSiguiente.SetActive(true);
                if (Input.GetButtonDown("Siguiente"))
                {
                    cuadroSiguiente.SetActive(false);
                    texto = "";
                    textoDialogo.text = texto;
                    frase_actual++;
                    if (frase_actual >= frases.Length)
                    {
                        OnTerminarDialogos.Invoke();
                    }
                    letra_actual = 0;
                }
            }
        }
    }
    public void AgregarNuevosDialogos(string[] nuevasFrases, UnityEvent nuevoEvento)
    {
        OnTerminarDialogos = nuevoEvento;
        frases = nuevasFrases;
        frase_actual = 0;
        letra_actual = 0;
    }
}
