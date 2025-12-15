using UnityEngine;

public class InventarioJugador : MonoBehaviour
{
    public bool tengoHielo = false;
    public GameObject hieloEnMano;

    public bool hieloEnOlla = false;

    public void TomarHielo(GameObject prefab)
    {
        if (tengoHielo)
        {
            Debug.Log("Ya tienes un hielo en la mano.");
            return;
        }

        hieloEnMano = Instantiate(prefab);
        hieloEnMano.transform.SetParent(transform);
        hieloEnMano.transform.localPosition = new Vector3(0.2f, -0.1f, 0.4f);

        tengoHielo = true;
        hieloEnOlla = false;

        Debug.Log("[InventarioJugador] Tomaste un cubo de hielo.");
    }

    public void ColocarHieloEnOlla(ControlEvaporizacion olla)
    {
        if (!tengoHielo)
        {
            Debug.Log("No tienes hielo.");
            return;
        }

        if (olla == null)
        {
            Debug.LogError("No hay ControlEvaporizacion.");
            return;
        }

        // ACTIVAR HIELO AL SER COLOCADO
        olla.hielo.SetActive(true);
        olla.hielo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        Destroy(hieloEnMano);
        tengoHielo = false;
        hieloEnOlla = true;

        // INICIA UN NUEVO CICLO EN LA OLLA
        olla.ReiniciarCiclo();

        Debug.Log("[InventarioJugador] Hielo colocado en la olla.");
    }
}
