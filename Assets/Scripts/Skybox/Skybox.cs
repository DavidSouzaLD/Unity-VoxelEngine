using UnityEngine;

public class Skybox : MonoBehaviour
{
    [Header("Settings")]
    public bool useRotationSun;
    [SerializeField] private Light sunLight;
    [SerializeField] private Transform sun;
    [SerializeField] private float timeSpeed;

    [Header("Colors")]
    [SerializeField] private Gradient colorSky;
    [SerializeField] private Gradient colorHorizon;
    [SerializeField] private Material skyboxMaterial;

    string shaderName = "_SunDirection";

    private void Start()
    {
        sun.position = -sunLight.transform.forward * 1500f;
    }

    private void LateUpdate()
    {
        if (useRotationSun)
        {
            // Rotate lighing sun
            sunLight.transform.Rotate(new Vector3(1f, 0f, 0f) * Time.deltaTime * timeSpeed);
        }

        // Apply values of light
        skyboxMaterial.SetColor("_SkyColor", colorSky.Evaluate(sunLight.transform.localEulerAngles.x / 180f));
        skyboxMaterial.SetColor("_HorizonColor", colorHorizon.Evaluate(sunLight.transform.localEulerAngles.x / 180f));
        skyboxMaterial.SetFloat("_WindSpeed", timeSpeed / 2f);

        // Active stars if is night
        if (sunLight.transform.localEulerAngles.x > 180f)
        {
            skyboxMaterial.SetFloat("_StarsOpacity", Mathf.Lerp(skyboxMaterial.GetFloat("_StarsOpacity"), 1f, Time.deltaTime * 2f));
        }
        else
        {
            skyboxMaterial.SetFloat("_StarsOpacity", Mathf.Lerp(skyboxMaterial.GetFloat("_StarsOpacity"), 0f, Time.deltaTime * 2f));
        }
    }
}
