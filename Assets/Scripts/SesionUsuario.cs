using UnityEngine;

public class SesionUsuario : MonoBehaviour
{
    public static string IdUsuario = "0";

    void Awake()
    {
        Debug.Log("═══════════════════════════════════════");
        Debug.Log("INICIANDO SESIÓN DE USUARIO");
        Debug.Log("═══════════════════════════════════════");

        if (PlayerPrefs.HasKey("usuario_id"))
        {
            IdUsuario = PlayerPrefs.GetString("usuario_id");
            Debug.Log("✅ ID cargado desde PlayerPrefs: " + IdUsuario);
        }
        else
        {
            IdUsuario = "0";
            Debug.LogWarning("⚠️ No hay sesión activa");

        }

        Debug.Log("═══════════════════════════════════════");
        Debug.Log("ID USUARIO FINAL: " + IdUsuario);
        Debug.Log("═══════════════════════════════════════");
    }

    // 🔑 Método centralizado
    public static void GuardarIdUsuario(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("⚠️ Intento de guardar ID vacío");
            return;
        }

        PlayerPrefs.SetString("usuario_id", id);
        PlayerPrefs.Save();
        IdUsuario = id;

        Debug.Log("💾 ID USUARIO GUARDADO: " + id);
    }

    public static bool SesionActiva()
    {
        return IdUsuario != "0";
    }

    public static void CerrarSesion()
    {
        PlayerPrefs.DeleteKey("usuario_id");
        PlayerPrefs.DeleteKey("usuario");
        PlayerPrefs.DeleteKey("rol");
        IdUsuario = "0";

        Debug.Log("🔒 Sesión cerrada");
    }
}
