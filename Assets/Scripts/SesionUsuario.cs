using UnityEngine;
using System;
using System.IO;

public class SesionUsuario : MonoBehaviour
{
    public static string IdUsuario = "0";

    void Awake()
    {
        Debug.Log("═══════════════════════════════════════");
        Debug.Log("INICIANDO SESIÓN DE USUARIO");
        Debug.Log("═══════════════════════════════════════");

        // Ruta fija (misma que C++)
        string tempFilePath = @"C:\Temp\unity_userid.txt";
        Debug.Log("Buscando archivo: " + tempFilePath);

        // Esperar un poco
        System.Threading.Thread.Sleep(200);

        // Intentar leer (con reintentos)
        for (int intento = 0; intento < 10; intento++)
        {
            if (File.Exists(tempFilePath))
            {
                try
                {
                    string contenido = File.ReadAllText(tempFilePath).Trim();
                    Debug.Log("✅ Archivo encontrado. Contenido: [" + contenido + "]");

                    if (!string.IsNullOrEmpty(contenido) && int.TryParse(contenido, out _))
                    {
                        IdUsuario = contenido;
                        Debug.Log("✅ ID EXTRAÍDO: " + IdUsuario);

                        // Eliminar archivo
                        try
                        {
                            File.Delete(tempFilePath);
                            Debug.Log("✅ Archivo eliminado");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning("No se pudo eliminar: " + ex.Message);
                        }

                        break; // Salir del loop
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Intento {intento + 1} falló: {ex.Message}");
                    System.Threading.Thread.Sleep(100);
                }
            }
            else
            {
                Debug.Log($"⏳ Intento {intento + 1}/10 - Archivo no encontrado");
                System.Threading.Thread.Sleep(100);
            }
        }

        if (IdUsuario == "0")
        {
            Debug.LogWarning("⚠️ No se encontró archivo. Usando default: 3");
        }

        Debug.Log("═══════════════════════════════════════");
        Debug.Log("ID USUARIO FINAL: " + IdUsuario);
        Debug.Log("═══════════════════════════════════════");
    }
}