using UnityEngine;
using UnityEngine.UI;

public class ParpadearUI : MonoBehaviour
{
    public float velocidad = 2f;
    Graphic graphic;

    float contador;

    void Awake()
    {
        graphic = GetComponent<Graphic>();
    }

    void OnEnable()
    {
        contador = 3f * Mathf.PI / 2f;
        ColocarAlpha(0f);
    }

    void Update()
    {
        contador += Time.deltaTime * velocidad;

        float alpha = (Mathf.Sin(contador) + 1f) * 0.5f;
        ColocarAlpha(alpha);
    }

    void ColocarAlpha(float a)
    {
        var c = graphic.color;
        c.a = a;
        graphic.color = c;
    }
}
