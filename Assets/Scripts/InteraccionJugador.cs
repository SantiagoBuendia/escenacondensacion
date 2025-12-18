using UnityEngine;

public class InteraccionJugador : MonoBehaviour
{
    public Camera camaraJugador;
    public float distanciaInteraccion = 3f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // clic izquierdo
        {
            Ray ray = camaraJugador.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteraccion))
            {
                ObjetoInteractivo objeto = hit.collider.GetComponent<ObjetoInteractivo>();
                if (objeto != null)
                {
                    objeto.Interactuar();
                }
            }
        }
    }
}