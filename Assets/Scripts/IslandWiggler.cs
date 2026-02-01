using UnityEngine;

public class IslandWiggler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Vector3 init_pos;
    Quaternion init_rot;
    public float bob_size, bob_speed, rotation_speed;

    private float time_offset;
    private void Start()
    {
        bob_size *= 0.5f + Random.value;
        bob_speed *= 0.5f + Random.value;
        rotation_speed *= 0.5f + Random.value;
        this.init_pos = transform.position;
        this.init_rot = transform.rotation;
        this.time_offset = Random.value * 15f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = init_pos + Vector3.up * Mathf.Sin((Time.time + this.time_offset)*bob_speed)*bob_size;
        transform.rotation = Quaternion.Euler(0f, rotation_speed * (Time.time + this.time_offset), 0f) * init_rot;
    }
}
