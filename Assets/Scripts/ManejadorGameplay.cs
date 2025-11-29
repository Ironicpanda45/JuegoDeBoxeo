using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ManejadorGameplay : MonoBehaviour
{
    [Header("Cinemática siguiente")]
    public string CinematicaSiguienteSiGana;
    public string CinematicaSiguienteSiPierde;

    public enum TipoNota
    {
        MovimientoIzquierda,
        MovimientoDerecha,
        GolpeIzquierda,
        GolpeDerecha,
        Cubrirse
    }

    [Header("Notas / Ritmo")]
    public GameObject prefabNota;
    public RectTransform spawnPoint;
    public RectTransform hitPoint;
    public float velocidadNota = 800f;
    public float tiempoEntreNotas = 0.6f;
    float tiempoSiguienteNota;

    [Header("Rondas / Tiempo")]
    public int rondaActual = 1;
    public int rondasTotales = 3;
    public float duracionRonda = 30f;
    float tiempoRonda;

    [Header("Porcentaje de partida")]
    [Range(0f, 1f)]
    public float porcentaje = 0.5f;
    public float cambioPorcentaje = 0.05f;
    public Slider barraPorcentaje;
    public float velocidadTransicionBarra = 5f;
    private float valorObjetivo;

    [Header("UI")]
    public TextMeshProUGUI textoTiempo;
    public TextMeshProUGUI textoCuenta;
    public float duracionCuenta = 1f;

    [Header("UI Rondas / Ganador")]
    public TextMeshProUGUI textoRonda;
    public TextMeshProUGUI textoGanador;
    public float duracionRondaTexto = 2f;
    public float duracionMostrarGanador = 2f;

    List<NotaRitmo> notasActivas = new List<NotaRitmo>();
    public bool autoGenerar = true;

    public ManejadorSonido manejadorSonido;
    public ManejadorCamara manejadorCamara;
    public Jugador jugador;
    public Enemigo enemigo;

    [Header("HUD Puntos")]
    public Image[] puntosJugador = new Image[3];
    public Image[] puntosEnemigo = new Image[3];

    [Header("Rondas Ganadas")]
    public int rondasGanadasJugador = 0;
    public int rondasGanadasEnemigo = 0;
    public int rondasParaGanar = 3;

    public TransicionesFade transiciones;

    void Start()
    {
        valorObjetivo = porcentaje;
        ActualizarBarra();
        StartCoroutine(IntroRonda());
    }

    void Update()
    {
        if (!autoGenerar) return;

        tiempoRonda -= Time.deltaTime;
        if (tiempoRonda <= 0f)
        {
            FinRonda();
            return;
        }

        tiempoSiguienteNota -= Time.deltaTime;
        if (tiempoSiguienteNota <= 0f)
        {
            GenerarNota();
            float factorDificultad = Mathf.Lerp(1.5f, 0.4f, porcentaje);
            float variacion = Random.Range(-0.1f, 0.1f);
            tiempoSiguienteNota = Mathf.Clamp(tiempoEntreNotas * factorDificultad + variacion, 0.2f, 1.5f);
        }

        DetectarInputJugador();

        if (barraPorcentaje != null)
            barraPorcentaje.value = Mathf.Lerp(barraPorcentaje.value, valorObjetivo, Time.deltaTime * velocidadTransicionBarra);

        ActualizarTextoTiempo();

        if (porcentaje <= 0f || porcentaje >= 1f)
        {
            FinRonda();
        }
    }

    IEnumerator IntroRonda()
    {
        autoGenerar = false;

        if (textoRonda != null)
        {
            textoRonda.gameObject.SetActive(true);
            textoRonda.text = "Ronda " + rondaActual;
            textoRonda.fontSize = 400;
        }

        if (textoGanador != null)
        {
            textoGanador.gameObject.SetActive(true);
            textoGanador.text = "";
        }

        yield return new WaitForSeconds(duracionRondaTexto);

        if (textoRonda != null) textoRonda.gameObject.SetActive(false);

        if (textoCuenta != null)
        {
            textoCuenta.gameObject.SetActive(true);
            for (int i = 3; i > 0; i--)
            {
                textoCuenta.text = i.ToString();
                textoCuenta.fontSize = 400 + i * 500;
                manejadorSonido?.ReproducirSonido(manejadorSonido.sonidoReloj);
                yield return new WaitForSeconds(duracionCuenta);
            }
            textoCuenta.gameObject.SetActive(false);
            manejadorSonido?.ReproducirSonido(manejadorSonido.sonidoCampana);
        }

        IniciarRonda();
    }

    void IniciarRonda()
    {
        tiempoRonda = duracionRonda;
        tiempoSiguienteNota = tiempoEntreNotas;
        valorObjetivo = porcentaje;
        ActualizarBarra();
        autoGenerar = true;
    }

    void GenerarNota()
    {
        TipoNota tipo = (TipoNota)Random.Range(0, 5);
        GameObject notaObj = Instantiate(prefabNota, spawnPoint.parent);
        notaObj.GetComponent<RectTransform>().anchoredPosition = spawnPoint.anchoredPosition;

        NotaRitmo nota = notaObj.GetComponent<NotaRitmo>();
        nota.Inicializar(tipo, hitPoint, velocidadNota, this);
        notasActivas.Add(nota);
    }

    void DetectarInputJugador()
    {
        if (Input.GetButtonDown("MoverIzquierda")) RevisarHit(TipoNota.MovimientoIzquierda);
        if (Input.GetButtonDown("MoverDerecha")) RevisarHit(TipoNota.MovimientoDerecha);
        if (Input.GetButtonDown("Golpe1")) RevisarHit(TipoNota.GolpeIzquierda);
        if (Input.GetButtonDown("Golpe2")) RevisarHit(TipoNota.GolpeDerecha);
        if (Input.GetButtonDown("Cubrirse")) RevisarHit(TipoNota.Cubrirse);
    }

    void RevisarHit(TipoNota tipo)
    {
        if (notasActivas.Count == 0) return;

        NotaRitmo nota = notasActivas[0];
        notasActivas.RemoveAt(0);

        if (nota.tipo != tipo || !nota.EstaEnZonaHit())
        {
            nota.Fallar();
            CambiarPorcentaje(-cambioPorcentaje);
            manejadorSonido?.ReproducirSonido(manejadorSonido.sonidoError);
            jugador?.RecibirDano();
            enemigo?.JugadorFallo();
        }
        else
        {
            nota.Acertar();
            CambiarPorcentaje(cambioPorcentaje);
            manejadorSonido?.ReproducirSonido(manejadorSonido.sonidoAcierto);

            switch (tipo)
            {
                case TipoNota.GolpeIzquierda: jugador.GolpearIzquierda(); break;
                case TipoNota.GolpeDerecha: jugador.GolpearDerecha(); break;
                case TipoNota.MovimientoIzquierda: jugador.EsquivarIzquierda(); break;
                case TipoNota.MovimientoDerecha: jugador.EsquivarDerecha(); break;
                case TipoNota.Cubrirse: jugador.CubrirseExitoso(); break;
            }

            if (enemigo != null)
            {
                if (tipo == TipoNota.GolpeIzquierda || tipo == TipoNota.GolpeDerecha)
                    enemigo.JugadorAcertoGolpe();
                else
                    enemigo.JugadorAcertoMovimientoOCubrirse();
            }
        }
    }

    void CambiarPorcentaje(float cantidad)
    {
        porcentaje = Mathf.Clamp01(porcentaje + cantidad);
        valorObjetivo = porcentaje;
    }

    IEnumerator MostrarGanadorYTerminarJuego(bool ganoJugador)
    {
        textoGanador.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);

        if (transiciones != null)
        {
            if (ganoJugador)
                transiciones.CambiarEscena(CinematicaSiguienteSiGana);
            else
                transiciones.CambiarEscena(CinematicaSiguienteSiPierde);
        }
    }

    void FinRonda()
    {
        if (!autoGenerar) return;

        autoGenerar = false;

        foreach (var n in notasActivas)
            if (n != null) Destroy(n.gameObject);
        notasActivas.Clear();

        bool ganoJugadorEnEstaRonda = porcentaje >= 1f || porcentaje > 0.5f;

        if (ganoJugadorEnEstaRonda)
        {
            rondasGanadasJugador++;
            textoGanador.text = "Jugador gana la ronda!";
        }
        else
        {
            rondasGanadasEnemigo++;
            textoGanador.text = "Enemigo gana la ronda!";
        }

        textoGanador.gameObject.SetActive(true);
        ActualizarPuntosHUD(rondasGanadasJugador, rondasGanadasEnemigo);

        if (rondasGanadasJugador >= rondasParaGanar)
        {
            textoGanador.text = "¡Jugador gana el juego!";
            StartCoroutine(MostrarGanadorYTerminarJuego(true));
        }
        else if (rondasGanadasEnemigo >= rondasParaGanar)
        {
            textoGanador.text = "¡Enemigo gana el juego!";
            StartCoroutine(MostrarGanadorYTerminarJuego(false));
        }

        StartCoroutine(MostrarGanadorYResetear());
    }

    IEnumerator MostrarGanadorYResetear()
    {
        yield return new WaitForSeconds(duracionMostrarGanador);

        porcentaje = 0.5f;
        valorObjetivo = porcentaje;
        ActualizarBarra();

        rondaActual++;
        StartCoroutine(IntroRonda());
    }

    void ActualizarBarra()
    {
        if (barraPorcentaje != null) barraPorcentaje.value = porcentaje;
    }

    void ActualizarTextoTiempo()
    {
        if (textoTiempo == null) return;
        int min = Mathf.FloorToInt(tiempoRonda / 60f);
        int sec = Mathf.FloorToInt(tiempoRonda % 60f);
        textoTiempo.text = $"{min:00}:{sec:00}";
    }

    public void RemoverNota(NotaRitmo n)
    {
        if (notasActivas.Contains(n)) notasActivas.Remove(n);
    }

    public void NotaFallida(NotaRitmo nota)
    {
        if (notasActivas.Contains(nota)) notasActivas.Remove(nota);
        CambiarPorcentaje(-cambioPorcentaje);
        manejadorSonido?.ReproducirSonido(manejadorSonido.sonidoDescubrirse);
        jugador?.RecibirDano();
        enemigo?.JugadorFallo();
    }

    public void ActualizarPuntosHUD(int puntosJ, int puntosE)
    {
        for (int i = 0; i < puntosJugador.Length; i++)
            puntosJugador[i].enabled = i < puntosJ;

        for (int i = 0; i < puntosEnemigo.Length; i++)
            puntosEnemigo[i].enabled = i < puntosE;
    }
}
