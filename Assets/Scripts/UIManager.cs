using UnityEngine;
using TMPro; // ?? Importar TextMesh Pro

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI estadoTexto; // ?? Cambiado a TMP

    public void CambiarEstado(string nuevoEstado)
    {
        if (estadoTexto != null)
            estadoTexto.text = "Estado actual: " + nuevoEstado;
    }
}
