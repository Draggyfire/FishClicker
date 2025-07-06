using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cycle Temps")]
    public float cycleDuration = 30f; // Durée d'un cycle complet (jour+nuit)
    private float cycleTime = 0f;

    [Header("Soleil & Lune")]
    public Transform sun;   // Soleil
    public Transform moon;  // Lune
    public Transform pivot; // Point central de rotation
    public float orbitRadius = 5f; // Rayon de l’orbite
    public Light sunLight;
    public Light moonLight;

    [Header("Visuel du Ciel")]
    public Gradient dayNightGradient; // Dégradé de couleurs du ciel
    private Image image;
    private SpriteRenderer spriteRenderer;
    private Camera cam;

    void Start()
    {
        image = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // Progression du cycle (entre 0 et 1)
        cycleTime += Time.deltaTime / cycleDuration;
        if (cycleTime > 1f) cycleTime = 0f; // Boucle infinie

        // 🔹 Récupérer la couleur du ciel
        Color currentColor = dayNightGradient.Evaluate(cycleTime);

        // 🔹 Appliquer la couleur du ciel
        if (sunLight) sunLight.color = currentColor;
        if (image) image.color = currentColor;
        if (spriteRenderer) spriteRenderer.color = currentColor;
        if (cam) cam.backgroundColor = currentColor;

        // 🔹 Rotation autour du pivot (mouvement orbital)
        float angle = cycleTime * 360f; // Convertit le cycle en un angle de rotation
        float radians = angle * Mathf.Deg2Rad;

        Vector3 sunPosition = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians),500f) * orbitRadius + pivot.position;
        Vector3 moonPosition = new Vector3(Mathf.Cos(radians + Mathf.PI), Mathf.Sin(radians + Mathf.PI), 500f) * orbitRadius + pivot.position;

        sun.position = sunPosition;
        moon.position = moonPosition;

        // 🔹 Ajuster l'intensité de la lumière
        float sunIntensity = Mathf.Clamp01(Mathf.Sin(radians));
        float moonIntensity = Mathf.Clamp01(Mathf.Sin(radians + Mathf.PI));

        sunLight.intensity = sunIntensity * 1.5f;
        moonLight.intensity = moonIntensity * 0.7f;
    }
}
