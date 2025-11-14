using UnityEngine;

public class ManejadorCamara : MonoBehaviour
{
    private Camera camaraPrincipal;
    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;
    private float campoVisionOriginal;

    private float fovObjetivo;
    public float zoomVelocidad = 2f;

    private bool zoomTemporalActivo = false;
    private float duracionEsperaZoom;
    private float tiempoEsperaTranscurrido = 0f;
    private float fovMedioObjetivo;

    public float shakeVelocidad = 10f;
    private Vector3 shakeOffsetObjetivo;
    private bool shakeActivo = false;
    private float duracionShake;
    private float magnitudShake;
    private float tiempoShakeTranscurrido = 0f;
    private Vector3 posicionInicialShake;

    void Start()
    {
        camaraPrincipal = Camera.main;
        if (camaraPrincipal != null)
        {
            posicionOriginal = camaraPrincipal.transform.localPosition;
            campoVisionOriginal = camaraPrincipal.fieldOfView;
            rotacionOriginal = camaraPrincipal.transform.localRotation;
        }
        fovObjetivo = campoVisionOriginal;
    }

    void Update()
    {
        if (!shakeActivo)
        {
            ManejarRetornoPosicionSuave();
        }

        ManejarZoomEnUpdate();

        if (zoomTemporalActivo)
        {
            ManejarZoomTemporalEnUpdate();
        }

        if (shakeActivo)
        {
            ManejarShakeEnUpdate();
        }
    }
    private void ManejarRetornoPosicionSuave()
    {
        if (Vector3.Distance(camaraPrincipal.transform.localPosition, posicionOriginal) > 0.001f)
        {
            camaraPrincipal.transform.localPosition = Vector3.Lerp(
                camaraPrincipal.transform.localPosition,
                posicionOriginal,
                Time.deltaTime * (shakeVelocidad * 2)
            );
        }
    }

    private void ManejarZoomEnUpdate()
    {
        camaraPrincipal.fieldOfView = Mathf.Lerp(
            camaraPrincipal.fieldOfView,
            fovObjetivo,
            Time.deltaTime * zoomVelocidad
        );
        if (Mathf.Abs(camaraPrincipal.fieldOfView - fovObjetivo) < 0.01f)
        {
            camaraPrincipal.fieldOfView = fovObjetivo;
        }
    }

    private void ManejarZoomTemporalEnUpdate()
    {
        if (Mathf.Abs(camaraPrincipal.fieldOfView - fovMedioObjetivo) < 0.01f)
        {
            tiempoEsperaTranscurrido += Time.deltaTime;

            if (tiempoEsperaTranscurrido >= duracionEsperaZoom)
            {
                IniciarZoom(campoVisionOriginal);
                zoomTemporalActivo = false;
            }
        }
    }

    private void ManejarShakeEnUpdate()
    {
        tiempoShakeTranscurrido += Time.deltaTime;

        if (tiempoShakeTranscurrido < duracionShake)
        {
            if (tiempoShakeTranscurrido % 0.05f < Time.deltaTime)
            {
                float x = Random.Range(-1f, 1f) * magnitudShake;
                float y = Random.Range(-1f, 1f) * magnitudShake;
                shakeOffsetObjetivo = new Vector3(x, y, 0);
            }
            camaraPrincipal.transform.localPosition = Vector3.Lerp(
                camaraPrincipal.transform.localPosition,
                posicionOriginal + shakeOffsetObjetivo,
                Time.deltaTime * shakeVelocidad
            );
        }
        else
        {
            shakeActivo = false;
        }
    }

    public void RestaurarCamara()
    {
        shakeActivo = false;
        zoomTemporalActivo = false;
        fovObjetivo = campoVisionOriginal;
        camaraPrincipal.transform.localPosition = posicionOriginal;
        camaraPrincipal.transform.localRotation = rotacionOriginal;
    }

    public void DetenerEfectos()
    {
        shakeActivo = false;
        zoomTemporalActivo = false;
    }

    public void IniciarZoom(float fov)
    {
        DetenerEfectos();
        fovObjetivo = fov;
    }

    public void IniciarZoomTemporal(float fovObjetivoTemporal, float duracionPausa)
    {
        DetenerEfectos();

        fovMedioObjetivo = fovObjetivoTemporal;
        duracionEsperaZoom = duracionPausa;
        tiempoEsperaTranscurrido = 0f;

        IniciarZoom(fovMedioObjetivo);

        zoomTemporalActivo = true;
    }

    public void IniciarRetornoZoomSuave()
    {
        IniciarZoom(campoVisionOriginal);
    }

    public void IniciarShake(float duracion, float magnitud)
    {
        DetenerEfectos();
        duracionShake = duracion;
        magnitudShake = magnitud;
        tiempoShakeTranscurrido = 0f;
        posicionInicialShake = camaraPrincipal.transform.localPosition;
        shakeActivo = true;
    }
}