using UnityEngine;
using QFSW.QC;

public class Skybox : MonoBehaviour
{
    [Header("Time")]
    [Range(0, 24)]
    [Command("Time.SetTime")]
    [CommandDescription("Sets the current time value. 0 - 24")]
    public float timeOfDay;
    public float lenghtOfDay = 1f;

    [Header("Sun & Moon")]
    public Light directionalLight;
    public Transform sunTransform;
    public Transform moonTransform;

    [Header("Colors")]
    public Gradient colorLight;
    public Gradient colorSky;
    public Gradient colorHorizon;
    public Material skyboxMaterial;

    private void Start()
    {
        sunTransform.position = -directionalLight.transform.forward * 1800f;
        moonTransform.position = directionalLight.transform.forward * 1800f;
    }

    private void LateUpdate()
    {
        timeOfDay += 6.0f / lenghtOfDay * 4f * Time.deltaTime;
        timeOfDay %= 24;
        UpdateSkybox(timeOfDay / 24f);
    }

    private void UpdateSkybox(float timePercent)
    {
        // Rotate lighing
        directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170, 0));

        // Apply values of light
        directionalLight.color = colorLight.Evaluate(timePercent);
        skyboxMaterial.SetColor("_SkyColor", colorSky.Evaluate(timePercent));
        skyboxMaterial.SetColor("_HorizonColor", colorHorizon.Evaluate(timePercent));
        skyboxMaterial.SetFloat("_WindSpeed", lenghtOfDay / 2f);

        // Active stars if is night
        if (directionalLight.transform.localEulerAngles.x > 180f)
        {
            skyboxMaterial.SetFloat("_StarsOpacity", Mathf.Lerp(skyboxMaterial.GetFloat("_StarsOpacity"), 1f, Time.deltaTime * 2f));
        }
        else
        {
            skyboxMaterial.SetFloat("_StarsOpacity", Mathf.Lerp(skyboxMaterial.GetFloat("_StarsOpacity"), 0f, Time.deltaTime * 2f));
        }
    }
}
