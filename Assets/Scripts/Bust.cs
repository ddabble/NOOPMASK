using UnityEngine;

public class Bust : MonoBehaviour
{
    [SerializeField]
    private Transform maskMountPoint;
    public Transform MaskMountPoint => maskMountPoint;

    public void EquipMask(Mask mask)
    {
        var maskTransform = mask.GameObject.transform;
        maskTransform.SetParent(transform);

        maskTransform.position = MaskMountPoint.position;
        // Make the mask face away from the bust
        maskTransform.LookAt(
            maskTransform.position + transform.forward, transform.up
        );
    }
}
