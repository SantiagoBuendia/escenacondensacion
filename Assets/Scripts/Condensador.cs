using UnityEngine;

public class Condensador : MonoBehaviour
{
    public ControlEvaporizacion olla;
    public GameObject prefabGota;
    public Transform puntoSalida;

    public float intervaloGoteo = 0.4f;
    float tiempo;

    void Update()
    {
        if (olla == null || prefabGota == null || puntoSalida == null)
            return;

        if (!olla.VaporActivo)
            return;

        tiempo += Time.deltaTime;

        if (tiempo >= intervaloGoteo)
        {
            Instantiate(prefabGota, puntoSalida.position, Quaternion.identity);
            tiempo = 0f;
        }
    }
}
