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
    public GameObject prefabHielo;            // Prefab del cubo de hielo (nevera)
    public ControlEvaporizacion olla;         // REFERENCIA DIRECTA A LA OLLA

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
            mensajeVR?.MostrarMensaje(mensajeExplicacion, 999f);

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

        // ================================
        // NEVERA
        // ================================
        if (esNevera && prefabHielo != null && jugador != null)
        {
            jugador.TomarHielo(prefabHielo);
            mensajeVR?.MostrarMensaje(mensajeAccionNevera);

            GestorSimulacionEvento.RegistrarEvento(
                GestorSimulacion.idSimulacionActual,
                "Hielo en olla",
                "El usuario coloco el hielo sobre la estufa/olla",
                (int)Time.time
            );

            return;
        }

        // ================================
        // OLLA
        // ================================
        if (esOlla && olla != null && jugador != null)
        {
            jugador.ColocarHieloEnOlla(olla);
            mensajeVR?.MostrarMensaje(mensajeAccionOlla);

            GestorSimulacionEvento.RegistrarEvento(
                GestorSimulacion.idSimulacionActual,
                "Hielo en olla",
                "El usuario coloco el hielo sobre la estufa/olla",
                (int)Time.time
            );

            return;
        }

        // ================================
        // BOTÓN ESTUFA (FIX DEFINITIVO)
        // ================================
        if (esBotonEstufa && olla != null)
        {
            olla.ToggleEstufa();

            if (olla.EstufaEncendida)
            {
                GestorSimulacionEvento.RegistrarEvento(GestorSimulacion.idSimulacionActual, "Encendido estufa", "El usuario pulso el boton de encendido", (int)Time.time);

                mensajeVR?.MostrarMensaje(mensajeAccionEstufaEncendida);
            }
            else
            {
                GestorSimulacionEvento.RegistrarEvento(GestorSimulacion.idSimulacionActual, "Apagado estufa", "El usuario apago la estufa", (int)Time.time);

                mensajeVR?.MostrarMensaje(mensajeAccionEstufaApagada);
            }


            return;
        }

        // ================================
        // DEFAULT
        // ================================
        mensajeVR?.MostrarMensaje(mensajeSinAccion);
    }
}
