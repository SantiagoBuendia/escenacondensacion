using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class PanelIngresoLogin : MonoBehaviour
{
    public TMP_InputField inputUsuario;
    public TMP_InputField inputContrasena;
    public TextMeshProUGUI textoMensaje;

    private string url = ApiConfig.BASE_URL;

    public void Ingresar()
    {
        if (inputUsuario.text == "" || inputContrasena.text == "")
        {
            textoMensaje.text = "⚠️ Complete todos los campos";
            return;
        }

        StartCoroutine(LoginCoroutine());
    }

    IEnumerator LoginCoroutine()
    {
        WWWForm form = new WWWForm();

        form.AddField("accion", "verificarVR");
        form.AddField("usuario", inputUsuario.text);
        form.AddField("contrasena", inputContrasena.text);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            textoMensaje.text = "❌ Error de conexión";
        }
        else
        {
            RespuestaLogin r = JsonUtility.FromJson<RespuestaLogin>(www.downloadHandler.text);

            if (r.ok)
            {
                textoMensaje.text = "✅ Bienvenido " + r.usuario;

                SesionUsuario.GuardarIdUsuario(r.id);
                PlayerPrefs.SetString("usuario", r.usuario);
                PlayerPrefs.SetString("rol", r.rol);

                StartCoroutine(OcultarPanelConDelay());
            }
            else
            {
                textoMensaje.text = "❌ " + r.mensaje;
            }
        }
    }

    IEnumerator OcultarPanelConDelay()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }
}

[System.Serializable]
public class RespuestaLogin
{
    public bool ok;
    public string id;
    public string usuario;
    public string rol;
    public string mensaje;
}
