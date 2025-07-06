using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class JaugeManager : MonoBehaviour
{
    
    public Image Jauge;
    public Image Moulinet;

    private bool bossFightActive = false;
    private float rotationAngle = 0f;
    private bool isRotating = false;

    private float RotationSpeed = 8;

    private void Awake()
    {
        if (!Jauge || !Moulinet)
        {
            Debug.LogError("Jauge or Moulinet is missing!");
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int StartBossFight()
    {
        DisplayJauge(true);
        bossFightActive = true;
        isRotating = true;
        StartCoroutine(RotateMoulinet());
        return 2;
    }
    
    private IEnumerator RotateMoulinet()
    {
        while (isRotating)
        {
            // Augmente l'angle
            rotationAngle += RotationSpeed;
            if (rotationAngle >= 360f) rotationAngle -= 360f;

            // Applique la rotation autour de Jauge
            Moulinet.transform.RotateAround(Jauge.transform.position, Vector3.forward, RotationSpeed);

            yield return new WaitForSeconds(0.02f);
        }
    }
    
    void SetRotation(float angle)
    {
        // Position du pivot en monde
        Vector3 pivotPosition = Moulinet.sprite.pivot; 

        // Calculer la nouvelle position du moulinet autour du pivot
        Vector3 direction = Moulinet.transform.position - pivotPosition;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector3 newPosition = pivotPosition + rotation * direction;

        // Appliquer la nouvelle position et rotation
        Moulinet.transform.position = newPosition;
        Moulinet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void DisplayJauge(bool display = true)
    {
        gameObject.SetActive(display);
    }
    
    public bool IsBossFightActive()
    {
        return bossFightActive;
    }
    
    public bool StopRotationAndCheck()
    {
        isRotating = false;
        
        // Vérifie si l'angle de la manivelle est dans une plage "gagnante"
        if (rotationAngle == 0 || (rotationAngle > 335f && rotationAngle < 360f))
        {
            Debug.Log("Pêche réussie !");
            DisplayJauge(false);
            bossFightActive = false;
            return true;
        }
    
        Handheld.Vibrate(); // Fait vibrer le téléphone si le boss est perdu
        Debug.Log("Pêche ratée...");
        DisplayJauge(false);
        bossFightActive = false;
        return false;
    }
}