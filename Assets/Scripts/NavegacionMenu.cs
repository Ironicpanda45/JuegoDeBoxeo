using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavegacionMenu : MonoBehaviour
{
    public Button[] botones;
    int indice = 0;
    bool bloqueo = false;

    void Start()
    {
        if (botones.Length > 0)
            Seleccionar(0);
    }

    void Update()
    {
        float vertical = Input.GetAxisRaw("Vertical");

        // Evita que una sola pulsación salte varias opciones
        if (!bloqueo)
        {
            if (vertical > 0.5f)
            {
                indice--;
                if (indice < 0) indice = botones.Length - 1;
                Seleccionar(indice);
                StartCoroutine(Bloquear());
            }
            else if (vertical < -0.5f)
            {
                indice++;
                if (indice >= botones.Length) indice = 0;
                Seleccionar(indice);
                StartCoroutine(Bloquear());
            }
        }

        if (Input.GetButtonDown("Submit"))
        {
            botones[indice].onClick.Invoke();
        }
    }

    IEnumerator Bloquear()
    {
        bloqueo = true;
        yield return new WaitForSeconds(0.2f);
        bloqueo = false;
    }

    void Seleccionar(int i)
    {
        EventSystem.current.SetSelectedGameObject(botones[i].gameObject);
    }
}
