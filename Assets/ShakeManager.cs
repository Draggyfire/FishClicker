using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShakeManager : MonoBehaviour
{
    private float shakeThreshold = 2f;  // Seuil de détection
    private float sqrShakeThreshold;
    private Vector3 lastAcceleration;
    private bool isShaking = false;

    void Start()
    {
        sqrShakeThreshold = shakeThreshold * shakeThreshold;
        InputSystem.EnableDevice(Accelerometer.current);
        lastAcceleration = Accelerometer.current.acceleration.ReadValue(); // Utilisation du New Input System

    }

    void Update()
    {
        DetectShake();
    }

    private void DetectShake()
    {
        if (Accelerometer.current == null)
        {
            Debug.LogWarning("⚠️ L'accéléromètre n'est pas disponible sur cet appareil !");
            return;
        }

        Vector3 acceleration = Accelerometer.current.acceleration.ReadValue();
        Vector3 deltaAcceleration = acceleration - lastAcceleration;
        if (deltaAcceleration.sqrMagnitude >= sqrShakeThreshold)
        {
            Debug.Log("📳 Téléphone secoué !");
            isShaking = true;
            OnShakeDetected();
        }
        else
        {
            isShaking = false;
        }

        lastAcceleration = acceleration;
    }

    private void OnShakeDetected()
    {
        Debug.Log("🎉 Action après secousse !");
    }

    public bool IsShaking()
    {
        return isShaking;
    }
}
