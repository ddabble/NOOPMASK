using UnityEngine;

public class SunSetter : MonoBehaviour
{
    void Start()
    {
        RenderSettings.skybox = Instantiate(RenderSettings.skybox);

    }

    void Update()
    {
        RenderSettings.skybox.SetVector("_PlayerPosition", transform.position);
    }
}
