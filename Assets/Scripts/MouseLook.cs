using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;   // sensibilidad del mouse
    public Transform playerBody;            // referencia al cuerpo del jugador

    float xRotation = 0f;   // rotación en el eje X (vertical)

    void Start()
    {
        // Ocultar y bloquear el cursor al centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Leer movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotación vertical de la cámara (arriba/abajo)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // limitar ángulo vertical

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal del jugador (izquierda/derecha)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}

