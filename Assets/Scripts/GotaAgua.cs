using UnityEngine;

public class GotaAgua : MonoBehaviour
{
    public float cantidadAgua = 0.003f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Olla")) return;

        ControlEvaporizacion control =
            other.GetComponent<ControlEvaporizacion>() ??
            other.GetComponentInParent<ControlEvaporizacion>();

        if (control != null)
            control.AgregarAguaPorCondensacion(cantidadAgua);

        Destroy(gameObject);
    }
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.linearDamping = 2f; // ?? aumenta para que caiga m�s lento
        }
    }

}
