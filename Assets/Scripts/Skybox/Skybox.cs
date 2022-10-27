using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Skybox : MonoBehaviour
{
    public Light sunLight;
    string shaderName = "_SunDirection";

    private void Start()
    {
        Shader.SetGlobalVector(shaderName, sunLight.transform.position);
    }
}
