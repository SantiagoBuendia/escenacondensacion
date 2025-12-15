using UnityEngine;

/// <summary>
/// Inventario simple para el jugador: controla si tiene un cubo de hielo en mano.
/// - Instancia el prefab del hielo en un punto "hieloEnMano" (si está asignado).
/// - Permite colocar el hielo en la olla llamando a ControlEvaporizacion.ResetProceso().
/// </summary>
public class InventarioJugador : MonoBehaviour
{
    [Header("Estado")]
    public bool tieneHielo = false;

    [Header("Visual (opcional)")]
    public Transform hieloEnMano; // empty transform donde aparece el hielo en la mano (opcional)
    GameObject hieloInstanciado;

    /// <summary>
    /// Tomar un cubo de hielo (instancia prefab y marca que el jugador tiene hielo).
    /// </summary>
    public void TomarHielo(GameObject prefabHielo)
    {
        if (tieneHielo) return;

        tieneHielo = true;

        if (prefabHielo != null && hieloEnMano != null)
        {
            hieloInstanciado = Instantiate(prefabHielo, hieloEnMano.position, Quaternion.identity);
            hieloInstanciado.transform.SetParent(hieloEnMano, true);
        }

        Debug.Log("[InventarioJugador] Tomó un cubo de hielo.");
    }

    /// <summary>
    /// Colocar el hielo en la olla: destruye el objeto visual en la mano y activa el hielo en la olla.
    /// Llama a ResetProceso() del ControlEvaporizacion para inicializar el ciclo.
    /// </summary>
    public void ColocarHieloEnOlla(ControlEvaporizacion controlEvaporizacion)
    {
        if (!tieneHielo || controlEvaporizacion == null) return;

        tieneHielo = false;

        // Destruir el hielo visual en mano si existe
        if (hieloInstanciado != null)
        {
            Destroy(hieloInstanciado);
            hieloInstanciado = null;
        }

        // Activar el hielo en la olla y reiniciar proceso
        if (controlEvaporizacion.hielo != null)
        {
            controlEvaporizacion.hielo.SetActive(true);
            controlEvaporizacion.ResetProceso();
        }

        Debug.Log("[InventarioJugador] Colocó el hielo en la olla.");
    }
}
