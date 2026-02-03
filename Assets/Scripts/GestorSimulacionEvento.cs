using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public static class GestorSimulacionEvento
{
    public static void RegistrarEvento(
        int simulacionId,
        string evento,
        string detalle,
        int tiempo
    )
    {

        if (simulacionId <= 0)
        {
            Debug.LogWarning($"⚠️ No se pudo registrar el evento '{evento}'. El ID de simulación es inválido ({simulacionId}). Asegúrate de que IniciarSimulacion haya terminado.");
            return; 
        }

        MonoBehaviourHelper.Instance.StartCoroutine(
            EnviarEvento(simulacionId, evento, detalle, tiempo)
        );
    }

    static IEnumerator EnviarEvento(
        int simulacionId,
        string evento,
        string detalle,
        int tiempo
    )
    {
        WWWForm form = new WWWForm();
        form.AddField("accion", "registrarEvento");
        form.AddField("simulacion_id", simulacionId); // Coincide con el CGI
        form.AddField("evento", evento);
        form.AddField("detalle", detalle);
        form.AddField("tiempo", tiempo); // Coincide con el CGI (tiempo_segundos)

        UnityWebRequest www =
            UnityWebRequest.Post("http://localhost/cgi-bin/PaginaWebLaboratorio.exe", form);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al registrar evento: " + www.error);
        }
    }
}