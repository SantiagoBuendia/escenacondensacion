using UnityEngine;
using TMPro;

public class ControlEvaporizacion : MonoBehaviour
{
    [Header("Referencias de escena")]
    public GameObject hielo;
    public GameObject agua;
    public Light luzEstufa;
    public TextMeshProUGUI textoUI;

    [Header("Ajustes de proceso")]
    public float velocidadAumentoTemp = 10f;
    public float velocidadDerretir = 0.005f;
    public float velocidadEvaporar = 0.05f;
    public float tiempoVaporVisible = 3f;

    bool estufaEncendida = false;
    float temperatura = 0f;
    bool transicionHieloAguaCompleta = false;
    bool aguaEvaporandose = false;
    bool vaporMostrado = false;

    Vector3 escalaInicialAgua;
    Vector3 posicionInicialAgua;

    ParticleSystem vaporPS;
    MensajeVRPro mensajeVR;

    void Start()
    {
        if (agua != null)
        {
            escalaInicialAgua = agua.transform.localScale;
            posicionInicialAgua = agua.transform.position;
            agua.SetActive(false);
        }

        if (hielo != null)
        {
            hielo.SetActive(true);
            hielo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }

        if (luzEstufa != null)
            luzEstufa.enabled = estufaEncendida;

        CrearVapor();
        mensajeVR = FindObjectOfType<MensajeVRPro>();
    }

    void Update()
    {
        if (!estufaEncendida) return;

        temperatura += Time.deltaTime * velocidadAumentoTemp;
        if (textoUI != null)
            textoUI.text = "Temperatura: " + (int)temperatura + " °C";

        if (!transicionHieloAguaCompleta && hielo != null && hielo.activeSelf)
        {
            hielo.transform.localScale -= Vector3.one * (velocidadDerretir * Time.deltaTime);

            if (hielo.transform.localScale.x <= 0.05f)
            {
                hielo.SetActive(false);

                if (agua != null)
                {
                    agua.SetActive(true);
                    agua.transform.localScale = new Vector3(escalaInicialAgua.x, 0.01f, escalaInicialAgua.z);
                    agua.transform.position = posicionInicialAgua;
                }

                transicionHieloAguaCompleta = true;
            }
        }
        else if (transicionHieloAguaCompleta && !aguaEvaporandose && agua != null && agua.activeSelf)
        {
            Vector3 esc = agua.transform.localScale;
            if (esc.y < escalaInicialAgua.y)
            {
                esc.y += Time.deltaTime * 0.05f;
                agua.transform.localScale = esc;

                Vector3 pos = agua.transform.position;
                pos.y += Time.deltaTime * 0.025f;
                agua.transform.position = pos;
            }
            else
            {
                aguaEvaporandose = true;
            }
        }
        else if (aguaEvaporandose && agua != null && agua.activeSelf)
        {
            agua.transform.localScale -= new Vector3(0f, velocidadEvaporar * Time.deltaTime, 0f);

            if (agua.transform.localScale.y <= 0.01f)
            {
                agua.SetActive(false);
                if (!vaporMostrado)
                    MostrarVapor();
            }
        }
    }

    public void ToggleEstufa()
    {
        estufaEncendida = !estufaEncendida;
        if (luzEstufa != null)
            luzEstufa.enabled = estufaEncendida;
    }

    public bool EstufaEncendida => estufaEncendida;

    // 🔹 EXPOSICIÓN SOLO LECTURA PARA EL CONDENSADOR
    public bool VaporActivo => vaporMostrado;

    void CrearVapor()
    {
        GameObject vaporGO = new GameObject("Vapor");
        vaporGO.transform.SetParent(this.transform);
        vaporGO.transform.localPosition = Vector3.zero;

        vaporPS = vaporGO.AddComponent<ParticleSystem>();
        var main = vaporPS.main;
        main.loop = false;
        main.startLifetime = 1.5f;
        main.startSpeed = 0.5f;
        main.startSize = 0.3f;
        main.startColor = new Color(1f, 1f, 1f, 0.6f);

        var emission = vaporPS.emission;
        emission.rateOverTime = 30f;

        var shape = vaporPS.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 25f;
        shape.radius = 0.2f;

        vaporPS.Stop();
    }

    void MostrarVapor()
    {
        vaporMostrado = true;
        vaporPS.Play();
        Invoke(nameof(OcultarVapor), tiempoVaporVisible);
    }

    void OcultarVapor()
    {
        vaporPS.Stop();
    }

    public void ResetProceso()
    {
        temperatura = 0f;
        transicionHieloAguaCompleta = false;
        aguaEvaporandose = false;
        vaporMostrado = false;

        if (textoUI != null)
            textoUI.text = "Temperatura: 0 °C";

        if (agua != null)
        {
            agua.SetActive(false);
            agua.transform.localScale = escalaInicialAgua;
            agua.transform.position = posicionInicialAgua;
        }

        if (hielo != null)
        {
            hielo.SetActive(true);
            hielo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }

        vaporPS.Stop();

        if (mensajeVR != null)
            mensajeVR.ResetMensajes();
    }

    public void ReiniciarCiclo()
    {
        ResetProceso();
    }
}
