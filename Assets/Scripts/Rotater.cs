using UnityEngine;

public class Rotater : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        var move = transform.rotation.eulerAngles.y;
        transform.Rotate(new Vector3(0f, Time.deltaTime*5f, 0f), Space.Self);
    }
}
