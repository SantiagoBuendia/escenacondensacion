using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class MensajeVRPro : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshProUGUI textoMensaje;

    [Header("Ajustes visuales")]
    public float distancia = 1.5f;
    public float fadeSpeed = 4f;

    Transform camara;
    Coroutine rutinaActual;
    CanvasGroup cg;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        if (Camera.main != null)
            camara = Camera.main.transform;

        SetAlpha(0f);
    }

    void LateUpdate()
    {
        if (camara == null)
        {
            if (Camera.main != null)
                camara = Camera.main.transform;
            return;
        }

        // Posicionar frente a la cámara y mirar hacia ella
        transform.position = camara.position + camara.forward * distancia;
        transform.LookAt(camara);
        transform.Rotate(0f, 180f, 0f);
    }

    // ------------------------
    // MOSTRAR / OCULTAR MENSAJES
    // ------------------------

    /// <summary>
    /// Mostrar mensaje con duración por defecto (fade in -> espera duracionDefault -> fade out).
    /// </summary>
    public void MostrarMensaje(string mensaje)
    {
        MostrarMensaje(mensaje, -1f);
    }

    /// <summary>
    /// Mostrar mensaje con override de duración.
    /// - durationOverride < 0 : usar comportamiento por defecto (fade in, duracion por defecto, fade out)
    /// - durationOverride >= 999f : mostrar indefinidamente hasta OcultarAhora()
    /// - durationOverride >= 0 && < 999f : mostrar durante ese tiempo
    /// </summary>
    public void MostrarMensaje(string mensaje, float durationOverride)
    {
        if (textoMensaje == null) return;

        textoMensaje.text = mensaje;

        // Reiniciar rutina actual si existe
        if (rutinaActual != null)
            StopCoroutine(rutinaActual);

        rutinaActual = StartCoroutine(ShowRoutine(durationOverride));
    }

    /// <summary>
    /// Oculta el mensaje inmediatamente (fuerza fade out).
    /// </summary>
    public void OcultarAhora()
    {
        if (rutinaActual != null)
            StopCoroutine(rutinaActual);

        rutinaActual = StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator ShowRoutine(float durationOverride)
    {
        // Fade in
        yield return StartCoroutine(FadeInCoroutine());

        // Decide tiempo visible
        if (durationOverride >= 999f)
        {
            // Mantener visible hasta que alguien llame OcultarAhora()
            rutinaActual = null; // dejamos de controlar, OcultarAhora() realizará el FadeOut
            yield break;
        }

        float tiempoVisible;
        if (durationOverride >= 0f)
            tiempoVisible = durationOverride;
        else
            tiempoVisible = 5f; // valor por defecto (puedes ajustar)

        // Esperar visible
        yield return new WaitForSeconds(tiempoVisible);

        // Fade out
        yield return StartCoroutine(FadeOutCoroutine());

        rutinaActual = null;
    }

    IEnumerator FadeInCoroutine()
    {
        if (cg == null) cg = GetComponent<CanvasGroup>();
        while (cg.alpha < 1f)
        {
            cg.alpha += Time.deltaTime * fadeSpeed;
            UpdateTextAlpha(cg.alpha);
            yield return null;
        }
        cg.alpha = 1f;
        UpdateTextAlpha(1f);
    }

    IEnumerator FadeOutCoroutine()
    {
        if (cg == null) cg = GetComponent<CanvasGroup>();
        while (cg.alpha > 0f)
        {
            cg.alpha -= Time.deltaTime * fadeSpeed;
            UpdateTextAlpha(Mathf.Max(0f, cg.alpha));
            yield return null;
        }
        cg.alpha = 0f;
        UpdateTextAlpha(0f);
    }

    void UpdateTextAlpha(float a)
    {
        if (textoMensaje != null)
        {
            Color c = textoMensaje.color;
            c.a = a;
            textoMensaje.color = c;
        }
    }

    void SetAlpha(float a)
    {
        if (cg == null) cg = GetComponent<CanvasGroup>();
        if (cg != null)
            cg.alpha = a;

        UpdateTextAlpha(a);
    }

    // Ya NO se usan mensajes únicos en esta versión, se deja vacío por compatibilidad
    public void ResetMensajes() { }
}
