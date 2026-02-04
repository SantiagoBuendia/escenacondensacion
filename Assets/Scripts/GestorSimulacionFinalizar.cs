using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public static class GestorSimulacionFinalizar
{
    public static void FinalizarSimulacion(int idSimulacion, int duracion)
    {
        // 1. Validación de ID (Punto 3)
        if (idSimulacion <= 0)
        {
            Debug.LogWarning("⚠️ No se puede finalizar la simulación: ID inválido.");
            return;
        }

        MonoBehaviourHelper.Instance.StartCoroutine(
            EnviarFinal(idSimulacion, duracion)
        );
    }

    static IEnumerator EnviarFinal(int idSimulacion, int duracion)
    {
        WWWForm form = new WWWForm();
        form.AddField("accion", "finalizarSimulacion");

        form.AddField("simulacion_id", idSimulacion);

        form.AddField("duracion", duracion);

        UnityWebRequest www =
            UnityWebRequest.Post(ApiConfig.BASE_URL, form);

        yield return www.SendWebRequest();

        // 3. Feedback para saber si funcionó
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Simulación finalizada correctamente en la base de datos.");
        }
        else
        {
            Debug.LogError("❌ Error al finalizar simulación: " + www.error);
        }
    }
}