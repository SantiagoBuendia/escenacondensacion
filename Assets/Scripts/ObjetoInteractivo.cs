using UnityEngine;

public class ObjetoInteractivo : MonoBehaviour
{
    [Header("Explicación educativa")]
    [TextArea] public string mensajeExplicacion;

    [Header("Tipo de objeto")]
    public bool esNevera;
    public bool esOlla;
    public bool esBotonEstufa;

    [Header("Mensajes")]
    [TextArea] public string mensajeAccionNevera = "Has tomado un cubo de hielo.";
    [TextArea] public string mensajeAccionOlla = "Has colocado el hielo en la olla.";
    [TextArea] public string mensajeAccionEstufaEncendida = "La estufa está encendida.";
    [TextArea] public string mensajeAccionEstufaApagada = "La estufa se ha apagado.";
    [TextArea] public string mensajeSinAccion = "No hay acción para este objeto.";

    [Header("Referencias")]
    public GameObject prefabHielo;
    public ControlEvaporizacion olla;

    MensajeVRPro mensajeVR;
    InventarioJugador jugador;
    UIExplicacionLaboratorio uiExplicacion;

    void Awake()
    {
        mensajeVR = FindObjectOfType<MensajeVRPro>();
        jugador = FindObjectOfType<InventarioJugador>();
        uiExplicacion = FindObjectOfType<UIExplicacionLaboratorio>();
    }

    public void OnHoverEnter()
    {
        if (!string.IsNullOrEmpty(mensajeExplicacion))
        {
            mensajeVR?.MostrarMensaje(mensajeExplicacion, 999f);
            uiExplicacion?.MostrarExplicacion(mensajeExplicacion);
        }
    }

    public void OnHoverExit()
    {
        mensajeVR?.OcultarAhora();
    }

    public void Interactuar()
    {
        if (mensajeVR == null) mensajeVR = FindObjectOfType<MensajeVRPro>();
        if (jugador == null) jugador = FindObjectOfType<InventarioJugador>();

        if (esNevera && prefabHielo != null)
        {
            jugador.TomarHielo(prefabHielo);
            mensajeVR?.MostrarMensaje(mensajeAccionNevera);
            return;
        }

        if (esOlla && olla != null)
        {
            jugador.ColocarHieloEnOlla(olla);
            mensajeVR?.MostrarMensaje(mensajeAccionOlla);
            return;
        }

        if (esBotonEstufa)
        {
            if (!jugador.hieloEnOlla)
            {
                mensajeVR?.MostrarMensaje("Primero coloca el hielo en la olla.");
                return;
            }

            olla.ToggleEstufa();

            if (olla.EstufaEncendida)
                mensajeVR?.MostrarMensaje(mensajeAccionEstufaEncendida);
            else
                mensajeVR?.MostrarMensaje(mensajeAccionEstufaApagada);

            return;
        }

        mensajeVR?.MostrarMensaje(mensajeSinAccion);
    }
}
