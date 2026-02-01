using UnityEngine;

public class Bust : MonoBehaviour
{
    [SerializeField]
    private Transform maskMountPoint;
    public Transform MaskMountPoint => maskMountPoint;

    [SerializeField]
    private GameObject startWithMaskEquipped;

    void Start()
    {
        if (startWithMaskEquipped)
            EquipMask(startWithMaskEquipped.GetComponent<Mask>());
    }

    public void EquipMask(Mask mask)
    {
        var maskTransform = mask.GameObject.transform;
        maskTransform.SetParent(transform);

        maskTransform.position = MaskMountPoint.position;
        // Make the mask face away from the bust
        maskTransform.LookAt(
            maskTransform.position + transform.forward, transform.up
        );

        var maskRb = maskTransform.GetComponent<Rigidbody>();
        if (maskRb)
            maskRb.isKinematic = true;
    }

    public void UnequipMask(Mask mask)
    {
        var maskTransform = mask.GameObject.transform;
        maskTransform.SetParent(null);

        var maskRb = maskTransform.GetComponent<Rigidbody>();
        if (maskRb)
            maskRb.isKinematic = false;
    }
}
