using UnityEngine;
using UnityEngine.UI;

public class NotaRitmo : MonoBehaviour
{
    public ManejadorGameplay.TipoNota tipo;

    public Sprite imgMovimientoIzquierda;
    public Sprite imgMovimientoDerecha;
    public Sprite imgGolpeIzquierda;
    public Sprite imgGolpeDerecha;
    public Sprite imgCubrirse;

    Image imagen;
    RectTransform rt;
    RectTransform objetivo;
    float velocidad;
    ManejadorGameplay manejador;

    public float rangoHit = 20f;
    public float fadeDuration = 0.5f;
    public float minScale = 0.6f;

    bool isFading = false;

    public void Inicializar(ManejadorGameplay.TipoNota t, RectTransform hitPoint, float vel, ManejadorGameplay mg)
    {
        tipo = t;
        objetivo = hitPoint;
        velocidad = vel;
        manejador = mg;

        rt = GetComponent<RectTransform>();
        imagen = GetComponent<Image>();

        if (imagen != null)
        {
            if (tipo == ManejadorGameplay.TipoNota.MovimientoIzquierda) imagen.sprite = imgMovimientoIzquierda;
            if (tipo == ManejadorGameplay.TipoNota.MovimientoDerecha) imagen.sprite = imgMovimientoDerecha;
            if (tipo == ManejadorGameplay.TipoNota.GolpeIzquierda) imagen.sprite = imgGolpeIzquierda;
            if (tipo == ManejadorGameplay.TipoNota.GolpeDerecha) imagen.sprite = imgGolpeDerecha;
            if (tipo == ManejadorGameplay.TipoNota.Cubrirse) imagen.sprite = imgCubrirse;
        }
    }

    void Update()
    {
        if (rt == null || objetivo == null || isFading) return;

        // Mover nota
        rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, new Vector2(rt.anchoredPosition.x, -1000f), velocidad * Time.deltaTime);

        // Si pasa la zona de hit y aún no se ha marcado como fallida
        if (!isFading && rt.anchoredPosition.y <= objetivo.anchoredPosition.y - rangoHit)
        {
            isFading = true; // marca que ya se procesó
            if (manejador != null)
            {
                manejador.NotaFallida(this); // avisa al manejador
            }
            StartCoroutine(FadeYRemover(Color.red, false));
        }
    }

    public bool EstaEnZonaHit()
    {
        return Vector2.Distance(rt.anchoredPosition, objetivo.anchoredPosition) <= rangoHit;
    }

    public void Acertar()
    {
        if (isFading) return;
        StartCoroutine(FadeYRemover(Color.white, true));
    }

    public void Fallar()
    {
        if (isFading) return;
        StartCoroutine(FadeYRemover(Color.red, false));
    }

    System.Collections.IEnumerator FadeYRemover(Color targetColor, bool conPop)
    {
        isFading = true;
        manejador.RemoverNota(this);

        Color inicialColor = imagen.color;
        Vector3 inicialScale = rt.localScale;
        float tiempo = 0f;

        float popScale = 1.4f;
        float popDuration = fadeDuration * 0.2f;
        float fadeTime = fadeDuration;

        if (conPop)
        {
            fadeTime -= popDuration;

            // --- Fase de pop ---
            while (tiempo < popDuration)
            {
                tiempo += Time.deltaTime;
                float t = tiempo / popDuration;
                rt.localScale = Vector3.Lerp(inicialScale, Vector3.one * popScale, t);
                yield return null;
            }

            tiempo = 0f;
            inicialScale = rt.localScale; // escala después del pop
        }

        // --- Fase fade y encoger ---
        while (tiempo < fadeTime)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / fadeTime;

            // Color
            imagen.color = Color.Lerp(inicialColor, new Color(targetColor.r, targetColor.g, targetColor.b, 0f), t);

            // Escala
            rt.localScale = Vector3.Lerp(inicialScale, Vector3.one * minScale, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}
