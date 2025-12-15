using UnityEngine;


/// <summary>
/// Script que se pone en el GameObject que contiene el XRRayInteractor (LeftHand / RightHand).
/// Detecta el "gatillo" via Input.GetButtonDown("Fire1") (o la entrada legacy) y, si el rayo
/// apunta a un ObjetoInteractivo, llama a Interactuar().
/// </summary>
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor))]
public class XRInteraccion : MonoBehaviour
{
    UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor interactor;

    void Start()
    {
        interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
        if (interactor == null) Debug.LogError("XRInteraccion requiere XRRayInteractor en el mismo GameObject.");
    }

    void Update()
    {
        if (interactor == null) return;

        // Disparador: intenta con Input legacy "Fire1" (teclado/ratón) y también con botón "Submit"
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Submit"))
        {
            TryInteract();
        }

        // Nota: si usas el nuevo Input System o acciones del XR Interaction Toolkit, deberías
        // atarlo a la acción correspondiente. Aquí queda una solución simple que funcionó antes.
    }

    void TryInteract()
    {
        // Usar el raycast actual del interactor
        if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            var obj = hit.collider.GetComponent<ObjetoInteractivo>();
            if (obj != null)
            {
                obj.Interactuar();
                Debug.Log("Interacción VR con: " + hit.collider.name);
            }
            else
            {
                Debug.Log("El objeto apuntado no tiene ObjetoInteractivo: " + hit.collider.name);
            }
        }
        else
        {
            Debug.Log("XRInteraccion: no hay hit detectado por el XRRayInteractor.");
        }
    }
}
