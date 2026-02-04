using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public static class GestorSimulacionResultado
{
    public static void RegistrarResultado(
        int idSimulacion,
        string variable,
        string valor,
        string unidad
    )
    {
        if (idSimulacion <= 0)
        {
            Debug.LogWarning($"⚠️ No se pudo registrar el resultado '{variable}'. ID de simulación inválido.");
            return;
        }

        MonoBehaviourHelper.Instance.StartCoroutine(
            EnviarResultado(idSimulacion, variable, valor, unidad)
        );
    }

    static IEnumerator EnviarResultado(
        int idSimulacion,
        string variable,
        string valor,
        string unidad
    )
    {
        WWWForm form = new WWWForm();
        form.AddField("accion", "registrarResultado");


        form.AddField("simulacion_id", idSimulacion);

        form.AddField("variable", variable);
        form.AddField("valor", valor);
        form.AddField("unidad", unidad);

        UnityWebRequest www =
            UnityWebRequest.Post(ApiConfig.BASE_URL, form);

        yield return www.SendWebRequest();

        // 3. Feedback de depuración
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"✅ Resultado guardado: {variable} = {valor} {unidad}");
        }
        else
        {
            Debug.LogError("❌ Error al registrar resultado: " + www.error);
        }
    }
}