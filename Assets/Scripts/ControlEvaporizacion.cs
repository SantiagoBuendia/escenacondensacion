using UnityEngine;
using TMPro;
using System.Collections;

public class ControlEvaporizacion : MonoBehaviour
{
    [Header("Referencias de escena")]
    public GameObject hielo;
    public GameObject agua;
    public GameObject aguaCondensada;
    public Light luzEstufa;
    public TextMeshProUGUI textoUI;

    [Header("Ajustes de proceso")]
    public float velocidadAumentoTemp = 10f;
    public float velocidadDerretir = 0.005f;
    public float velocidadEvaporar = 0.05f;
    public float tiempoVaporVisible = 3f;

    [Header("Condensación")]
    public float tiempoEsperaCondensacion = 7f;
    public float velocidadSubidaCondensada = 0.02f;

    // ===============================
    // VARIABLES PARA BASE DE DATOS
    // ===============================
    private float tiempoInicioSimulacion;
    private bool simulacionIniciadaBD = false;
    private bool simulacionFinalizadaBD = false;

    // ===============================
    // ESTADOS ORIGINALES (NO TOCADOS)
    // ===============================
    bool estufaEncendida = false;
    float temperatura = 0f;
    bool transicionHieloAguaCompleta = false;
    bool aguaEvaporandose = false;
    bool vaporMostrado = false;

    // ===============================
    // ESTADOS NUEVOS
    // ===============================
    bool esperandoCondensacion = false;
    bool llenandoCondensada = false;
    float temporizadorCondensacion = 0f;

    // ⬇️ ADICIÓN: límite máximo de temperatura
    private const float TEMPERATURA_MAXIMA = 150f;

    Vector3 escalaInicialAgua;
    Vector3 posicionInicialAgua;
    Vector3 escalaInicialCondensada;

    ParticleSystem vaporPS;

    // ===============================
    // PROPIEDADES USADAS POR OTROS
    // ===============================
    public bool EstufaEncendida => estufaEncendida;
    public bool VaporActivo => vaporMostrado;

    void Start()
    {
        if (agua != null)
        {
            escalaInicialAgua = agua.transform.localScale;
            posicionInicialAgua = agua.transform.position;
            agua.SetActive(false);
        }

        if (aguaCondensada != null)
        {
            escalaInicialCondensada = aguaCondensada.transform.localScale;
            aguaCondensada.SetActive(false);
        }

        if (hielo != null)
        {
            hielo.SetActive(true);
            hielo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }

        if (luzEstufa != null)
            luzEstufa.enabled = estufaEncendida;

        CrearVapor();
        ActualizarUI();
    }

    void Update()
    {

        // --- INICIO DE SIMULACIÓN EN BD ---
        if (hielo.activeSelf && !simulacionIniciadaBD)
        {
            IniciarSimulacionEnBD();
        }

        if (!estufaEncendida) return;

        temperatura += Time.deltaTime * velocidadAumentoTemp;
        temperatura = Mathf.Min(temperatura, TEMPERATURA_MAXIMA); // ⬅️ LIMITADOR
        ActualizarUI();

        // 🔹 HIELO → AGUA
        if (!transicionHieloAguaCompleta && hielo.activeSelf)
        {
            hielo.transform.localScale -= Vector3.one * velocidadDerretir * Time.deltaTime;

            if (hielo.transform.localScale.x <= 0.05f)
            {
                hielo.SetActive(false);
                agua.SetActive(true);
                agua.transform.localScale =
                    new Vector3(escalaInicialAgua.x, 0.01f, escalaInicialAgua.z);
                agua.transform.position = posicionInicialAgua;
                transicionHieloAguaCompleta = true;
            }
        }
        // 🔹 AGUA SUBE
        else if (transicionHieloAguaCompleta && !aguaEvaporandose)
        {
            Vector3 esc = agua.transform.localScale;

            if (esc.y < escalaInicialAgua.y)
            {
                esc.y += Time.deltaTime * 0.05f;
                agua.transform.localScale = esc;
            }
            else
            {
                aguaEvaporandose = true;
            }
        }
        // 🔹 EVAPORACIÓN
        else if (aguaEvaporandose)
        {
            agua.transform.localScale -=
                new Vector3(0f, velocidadEvaporar * Time.deltaTime, 0f);

            if (agua.transform.localScale.y <= 0.01f)
            {
                agua.SetActive(false);

                if (!vaporMostrado)
                {
                    MostrarVapor();
                    IniciarEsperaCondensacion();
                }
            }
        }

        // 🔹 ESPERA DE CONDENSACIÓN
        if (esperandoCondensacion)
        {
            temporizadorCondensacion += Time.deltaTime;

            if (temporizadorCondensacion >= tiempoEsperaCondensacion)
            {
                esperandoCondensacion = false;
                ActivarAguaCondensada();
            }
        }

        // 🔹 SUBIDA DEL AGUA CONDENSADA
        if (llenandoCondensada && aguaCondensada.activeSelf)
        {
            Vector3 esc = aguaCondensada.transform.localScale;
            esc.y += Time.deltaTime * velocidadSubidaCondensada;
            if (esc.y >= escalaInicialCondensada.y)
            {
                esc.y = escalaInicialCondensada.y;
                llenandoCondensada = false;
                FinalizarSimulacionBD(); // <--- GUARDAR EN BD AL LLENARSE
            }
            aguaCondensada.transform.localScale = esc;
        }
    }

    // ===============================
    // ESTUFA
    // ===============================
    public void ToggleEstufa()
    {
        estufaEncendida = !estufaEncendida;

        if (luzEstufa != null)
            luzEstufa.enabled = estufaEncendida;

        if (simulacionIniciadaBD && !simulacionFinalizadaBD && GestorSimulacion.idSimulacionActual > 0)
        {
            GestorSimulacionEvento.RegistrarEvento(
                GestorSimulacion.idSimulacionActual,
                estufaEncendida ? "Estufa encendida" : "Estufa apagada",
                "Usuario cambio el estado del calor",
                (int)Time.time
            );
        }
    }

    // ===============================
    // VAPOR
    // ===============================
    void CrearVapor()
    {
        GameObject vaporGO = new GameObject("Vapor");
        vaporGO.transform.SetParent(transform);
        vaporGO.transform.localPosition = Vector3.zero;

        vaporPS = vaporGO.AddComponent<ParticleSystem>();
        vaporPS.Stop();
    }

    void MostrarVapor()
    {
        vaporMostrado = true;
        vaporPS.Play();
    }

    // ===============================
    // CONDENSACIÓN
    // ===============================
    void IniciarEsperaCondensacion()
    {
        esperandoCondensacion = true;
        temporizadorCondensacion = 0f;
    }

    void ActivarAguaCondensada()
    {
        aguaCondensada.SetActive(true);
        aguaCondensada.transform.localScale =
            new Vector3(escalaInicialCondensada.x, 0.01f, escalaInicialCondensada.z);
        llenandoCondensada = true;
    }

    // ===============================
    // MÉTODOS COMPATIBILIDAD
    // ===============================
    public void AgregarAguaPorCondensacion(float cantidad)
    {
        // Se mantiene SOLO para no romper GotaAgua
    }

    public void ResetProceso()
    {
        temperatura = 0f;
        transicionHieloAguaCompleta = false;
        aguaEvaporandose = false;
        vaporMostrado = false;
        esperandoCondensacion = false;
        llenandoCondensada = false;

        agua.SetActive(false);
        aguaCondensada.SetActive(false);

        hielo.SetActive(true);
        hielo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        vaporPS.Stop();
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (textoUI != null)
            textoUI.text = "Temperatura: " + (int)temperatura + " °C";
    }
    // ===============================
    // LÓGICA DE BASE DE DATOS
    // ===============================
    void IniciarSimulacionEnBD()
    {
        simulacionIniciadaBD = true;
        tiempoInicioSimulacion = Time.time;

        GestorSimulacion.IniciarSimulacion(
            SesionUsuario.IdUsuario,
            "Condensacion de la materia",
            "Cambio de estado gaseoso a liquido",
            "VR"
        );
        StartCoroutine(RegistrarEventoInicial());
    }

    IEnumerator RegistrarEventoInicial()
    {
        yield return new WaitUntil(() => GestorSimulacion.idSimulacionActual > 0);
        GestorSimulacionEvento.RegistrarEvento(
            GestorSimulacion.idSimulacionActual,
            "Inicio Proceso",
            "El hielo esta en posicion y listo para el calor",
            (int)Time.time
        );
    }

    void FinalizarSimulacionBD()
    {
        if (simulacionFinalizadaBD) return;
        simulacionFinalizadaBD = true;

        GestorSimulacionResultado.RegistrarResultado(
            GestorSimulacion.idSimulacionActual,
            "Temperatura final condensacion",
            temperatura.ToString("F1"),
            "°C"
        );

        int duracion = (int)(Time.time - tiempoInicioSimulacion);
        GestorSimulacionFinalizar.FinalizarSimulacion(GestorSimulacion.idSimulacionActual, duracion);

        if (textoUI != null)
        {
            textoUI.text = "¡CONDENSACIÓN COMPLETADA!";
            textoUI.color = Color.green;
        }

        Invoke("CerrarAplicacion", 5f);
    }

    void CerrarAplicacion() { Application.Quit(); }
}
