using UnityEngine;

/// <summary>
/// Objeto interactivo: hover muestra explicación larga, interact ejecuta acción.
/// Adaptado para trabajar con ControlEvaporizacion.
/// </summary>
public class ObjetoInteractivo : MonoBehaviour
{
    [Header("Explicación educativa (hover)")]
    [TextArea]
    public string mensajeExplicacion;

    [Header("Tipo de objeto (marca uno)")]
    public bool esNevera;
    public bool esOlla;
    public bool esBotonEstufa;

    [Header("Mensajes cortos (al interactuar)")]
    [TextArea] public string mensajeAccionNevera = "Has tomado un cubo de hielo.";
    [TextArea] public string mensajeAccionOlla = "Has colocado el hielo en la olla.";
    [TextArea] public string mensajeAccionEstufaEncendida = "La estufa está encendida.";
    [TextArea] public string mensajeAccionEstufaApagada = "La estufa se ha apagado.";
    [TextArea] public string mensajeSinAccion = "No hay acción para este objeto.";

    [Header("Referencias")]
    public GameObject prefabHielo;           // Prefab del cubo de hielo (para la nevera)
    public ControlEvaporizacion olla;        // Referencia a la olla (ControlEvaporizacion) si se necesita

    // caches
    MensajeVRPro mensajeVR;
    InventarioJugador jugador;
    UIExplicacionLaboratorio uiExplicacion;

    void Awake()
    {
        mensajeVR = FindObjectOfType<MensajeVRPro>();
        jugador = FindObjectOfType<InventarioJugador>();
        uiExplicacion = FindObjectOfType<UIExplicacionLaboratorio>();
    }

    // Hover enter: mostrar explicación larga
    public void OnHoverEnter()
    {
        if (!string.IsNullOrEmpty(mensajeExplicacion))
        {
            // Mostrar en HUD VR con duración muy larga (hasta OnHoverExit)
            mensajeVR?.MostrarMensaje(mensajeExplicacion, 999f);

            // También actualizar panel de UI si existe
            if (uiExplicacion != null)
                uiExplicacion.MostrarExplicacion(mensajeExplicacion);
        }
    }

    public void OnHoverExit()
    {
        mensajeVR?.OcultarAhora();
    }

    // Interactuar (gatillo)
    public void Interactuar()
    {
        Debug.Log("[ObjetoInteractivo] Interactuar con " + gameObject.name);

        if (mensajeVR == null) mensajeVR = FindObjectOfType<MensajeVRPro>();
        if (jugador == null) jugador = FindObjectOfType<InventarioJugador>();

        // Nevera: tomar hielo
        if (esNevera && prefabHielo != null && jugador != null)
        {
            jugador.TomarHielo(prefabHielo);
            mensajeVR?.MostrarMensaje(mensajeAccionNevera);
            return;
        }

        // Olla: colocar hielo
        if (esOlla && olla != null && jugador != null)
        {
            jugador.ColocarHieloEnOlla(olla);
            mensajeVR?.MostrarMensaje(mensajeAccionOlla);
            return;
        }

        // Botón estufa: toggle
        if (esBotonEstufa)
        {
            var control = FindObjectOfType<ControlEvaporizacion>();
            if (control != null)
            {
                control.ToggleEstufa();
                if (control.EstufaEncendida)
                    mensajeVR?.MostrarMensaje(mensajeAccionEstufaEncendida);
                else
                    mensajeVR?.MostrarMensaje(mensajeAccionEstufaApagada);
            }
            return;
        }

        // Default
        mensajeVR?.MostrarMensaje(mensajeSinAccion);
    }
}
