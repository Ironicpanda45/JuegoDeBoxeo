using UnityEngine;
using UnityEngine.UI;

public class BarraEstaminaUI : MonoBehaviour
{
    public Slider sliderEstamina;

    public Jugador jugador;

    [Tooltip("Velocidad de transición del Slider (cuanto mayor, más rápido y menos suave).")]
    public float velocidadTransicion = 5f;

    private float valorObjetivo;

    void Start()
    {
        if (jugador == null)
        {
            Debug.LogError("La referencia al Jugador no está asignada en el script BarraEstaminaUI.");
            return;
        }

        if (sliderEstamina == null)
        {
            Debug.LogError("La referencia al Slider de Estamina no está asignada en el script BarraEstaminaUI.");
            return;
        }
        sliderEstamina.maxValue = jugador.estaminaMaxima;
        valorObjetivo = jugador.estaminaActual;
        sliderEstamina.value = valorObjetivo;
    }

    void Update()
    {
        if (jugador != null && sliderEstamina != null)
        {
            valorObjetivo = jugador.estaminaActual;
            sliderEstamina.value = Mathf.Lerp(
                sliderEstamina.value,
                valorObjetivo,
                Time.deltaTime * velocidadTransicion
            );
        }
    }

    public void ActualizarMaximoEstamina(float nuevoMaximo)
    {
        sliderEstamina.maxValue = nuevoMaximo;
        valorObjetivo = jugador.estaminaActual;
        sliderEstamina.value = jugador.estaminaActual;
    }
}