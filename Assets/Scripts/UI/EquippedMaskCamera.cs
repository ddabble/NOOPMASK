using UnityEngine;

public class EquippedMaskCamera : MonoBehaviour
{
    public static EquippedMaskCamera Singleton;

    [SerializeField]
    private Bust bust;
    [SerializeField]
    private float bustRotateSpeed = 60.0f;

    private new Camera camera;

    private void Awake()
    {
        #region Singleton boilerplate

        if (Singleton != null)
        {
            if (Singleton != this)
            {
                Debug.LogWarning($"There's more than one {Singleton.GetType()} in the scene!", this);
                Destroy(gameObject);
            }

            return;
        }

        Singleton = this;

        #endregion Singleton boilerplate
    }

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        bust.transform.Rotate(Vector3.up, Time.deltaTime * bustRotateSpeed);
    }

    public void EquipMask(Mask mask)
    {
        bust.EquipMask(mask);
    }

    public void UnequipMask(Mask mask)
    {
        bust.UnequipMask(mask);
    }
}
