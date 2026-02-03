using UnityEngine;
using System;
using System.IO;

public class DebugArgumentos : MonoBehaviour
{
    void Start()
    {
        string[] args = Environment.GetCommandLineArgs();
        string logPath = Path.Combine(Application.dataPath, "argumentos_log.txt");

        using (StreamWriter writer = new StreamWriter(logPath, true))
        {
            writer.WriteLine("=== NUEVO INICIO: " + DateTime.Now + " ===");
            writer.WriteLine("Total argumentos: " + args.Length);
            for (int i = 0; i < args.Length; i++)
            {
                writer.WriteLine($"args[{i}] = '{args[i]}'");
            }
            writer.WriteLine("ID Usuario detectado: " + SesionUsuario.IdUsuario);
            writer.WriteLine("");
        }
    }
}