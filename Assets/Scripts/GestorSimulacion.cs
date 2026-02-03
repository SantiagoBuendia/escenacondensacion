using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public static class GestorSimulacion
{
    public static int idSimulacionActual = -1;

    public static void IniciarSimulacion(
        string usuarioId,
        string nombre,
        string descripcion,
        string dispositivo
    )
    {
        MonoBehaviourHelper.Instance.StartCoroutine(
            EnviarInicio(usuarioId, nombre, descripcion, dispositivo)
        );
    }

    private static IEnumerator EnviarInicio(
        string usuarioId,
        string nombre,
        string descripcion,
        string dispositivo
    )
    {
        // 1. Validaciones de seguridad para evitar el error de "String Null"
        if (string.IsNullOrEmpty(usuarioId)) usuarioId = "0";
        if (string.IsNullOrEmpty(nombre)) nombre = "Simulacion Condensacion";
        if (string.IsNullOrEmpty(descripcion)) descripcion = "Sin Descripcion";
        if (string.IsNullOrEmpty(dispositivo)) dispositivo = "PC/VR";

        WWWForm form = new WWWForm();
        form.AddField("accion", "iniciarSimulacion");
        form.AddField("usuario_id", usuarioId);
        form.AddField("nombre", nombre);
        form.AddField("descripcion", descripcion);
        form.AddField("dispositivo", dispositivo);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/cgi-bin/PaginaWebLaboratorio.exe", form);

        yield return www.SendWebRequest();

        // --- BLOQUE DE DEPURACIÓN ---
        string respuestaServidor = www.downloadHandler.text;
        Debug.Log(">>> RESPUESTA CRUDA DEL SERVIDOR: [" + respuestaServidor + "]");

        if (www.result == UnityWebRequest.Result.Success)
        {
            
            if (int.TryParse(respuestaServidor.Trim(), out int resultadoID))
            {
                idSimulacionActual = resultadoID;

                if (idSimulacionActual > 0)
                {
                    Debug.Log("✅ Simulación registrada con éxito. ID: " + idSimulacionActual);
                }
                else
                {
                    Debug.LogError("❌ El servidor devolvió un ID inválido (menor o igual a 0). Revisa el .exe");
                }
            }
            else
            {
                idSimulacionActual = -1;
                Debug.LogError("❌ El servidor no devolvió un número. Posible error de programación en el .exe. Respuesta recibida: " + respuestaServidor);
            }
        }
        else
        {
            idSimulacionActual = -1;
            Debug.LogError("❌ Error de red o Apache: " + www.error);
        }
    }
}