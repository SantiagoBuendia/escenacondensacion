using UnityEngine;

public class Condensador : MonoBehaviour
{
    public ControlEvaporizacion control;
    public GameObject gotaPrefab;
    public Transform puntoCaida;
    public float tiempoEntreGotas = 0.4f;

    bool condensando = false;

    void Update()
    {
        if (control == null || gotaPrefab == null || puntoCaida == null)
            return;

        if (control.VaporActivo && !condensando)
        {
            condensando = true;
            InvokeRepeating(nameof(CrearGota), 0f, tiempoEntreGotas);
        }
        else if (!control.VaporActivo && condensando)
        {
            condensando = false;
            CancelInvoke(nameof(CrearGota));
        }
    }

    void CrearGota()
    {
        Instantiate(gotaPrefab, puntoCaida.position, Quaternion.identity);
    }
}
