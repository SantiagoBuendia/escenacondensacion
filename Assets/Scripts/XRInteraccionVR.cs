using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// MANO DERECHA = mover cámara (mirar) e interactuar.
/// MANO IZQUIERDA = solo interactuar.
/// Funciona con XR Origin (VR) SIN Acción-based.
/// Detecta hover con XRRayInteractor y gatillo real del dispositivo.
/// </summary>
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor))]
public class XRInteraccionVR : MonoBehaviour
{
    public enum HandRole { AutoDetectByName, Left, Right }
    public HandRole handRole = HandRole.AutoDetectByName;

    [Header("Rotación de cámara (solo mano derecha)")]
    public float rotationSpeed = 60f;
    public float maxPitch = 60f;
    public float minPitch = -60f;

    UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor interactor;
    ObjetoInteractivo objetoActual;
    InputDevice device;
    bool triggerPressedPrev = false;
    float triggerThreshold = 0.6f;
    bool isRightHand = false;
    float pitchRotation = 0f;
    Transform cameraOffset;

    void Start()
    {
        interactor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();

        // Detectar mano por nombre o setting
        string n = gameObject.name.ToLower();
        if (handRole == HandRole.Right || (handRole == HandRole.AutoDetectByName && n.Contains("right")))
            isRightHand = true;
        else
            isRightHand = false;

        // Buscar Camera Offset (XR Origin -> "Camera Offset")
        var xrOrigin = transform.parent;
        if (xrOrigin != null)
        {
            cameraOffset = xrOrigin.Find("Camera Offset");
            if (cameraOffset == null && Camera.main != null)
                cameraOffset = Camera.main.transform.parent;
        }

        TryAssignDevice();
    }

    void TryAssignDevice()
    {
        InputDeviceRole role = isRightHand ? InputDeviceRole.RightHanded : InputDeviceRole.LeftHanded;
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithRole(role, devices);
        device = devices.Count > 0 ? devices[0] : default;
    }

    void Update()
    {
        if (!device.isValid)
            TryAssignDevice();

        // Rotación (solo mano derecha)
        if (isRightHand && device.isValid && cameraOffset != null)
        {
            if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
            {
                // Yaw (rotar rig entero)
                float yaw = axis.x * rotationSpeed * Time.deltaTime;
                cameraOffset.Rotate(0f, yaw, 0f);

                // Pitch (mover cámara arriba/abajo)
                pitchRotation -= axis.y * rotationSpeed * Time.deltaTime;
                pitchRotation = Mathf.Clamp(pitchRotation, minPitch, maxPitch);

                var cam = Camera.main;
                if (cam != null)
                {
                    Vector3 euler = cam.transform.localEulerAngles;
                    // Evitar wrap 0-360 (convertir implícitamente al rango adecuado)
                    euler.x = pitchRotation;
                    cam.transform.localEulerAngles = euler;
                }
            }
        }

        // Hover detection (XRRayInteractor)
        if (interactor != null)
        {
            if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                ObjetoInteractivo nuevo = hit.collider.GetComponent<ObjetoInteractivo>();
                if (nuevo != objetoActual)
                {
                    objetoActual?.OnHoverExit();
                    objetoActual = nuevo;
                    objetoActual?.OnHoverEnter();
                }
            }
            else
            {
                if (objetoActual != null)
                {
                    objetoActual.OnHoverExit();
                    objetoActual = null;
                }
            }
        }

        // Trigger detection (InputDevice)
        bool triggered = false;
        if (device.isValid)
        {
            if (device.TryGetFeatureValue(CommonUsages.trigger, out float trigVal))
                triggered = trigVal > triggerThreshold;
            else if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primary))
                triggered = primary;
        }
        else
        {
            // Fallback para testing en editor
            if (Input.GetMouseButtonDown(0))
                triggered = true;
        }

        // Edge detection: solo on press
        if (triggered && !triggerPressedPrev && objetoActual != null)
        {
            objetoActual.Interactuar();
        }

        triggerPressedPrev = triggered;
    }
}
