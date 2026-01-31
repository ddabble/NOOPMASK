using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerMovement;

public class MaskHolder : MonoBehaviour
{
    private InputAction pickUpOrDropAction;
    private InputAction equipAction;

    [SerializeField]
    private CinemachineBrain cinemachineBrain;
    [SerializeField]
    private TMP_Text hudText;

    [SerializeField]
    private float pickUpMaskMaxDistance = 3.0f;
    [SerializeField]
    private float dropMaskDistance = 1.5f;
    private Mask lookingAtMask;
    private Mask heldMask;
    private Mask equippedMask;
    private Vector3 heldMaskOriginalScale;
    [SerializeField]
    private Vector3 heldMaskScreenCornerOffset;
    [SerializeField]
    private Vector3 equippedMaskScreenTopOffset;
    [SerializeField]
    private float heldMaskScaleMultiplier = 0.2f;

    void Start()
    {
        pickUpOrDropAction = InputSystem.actions.FindAction("Attack");
        equipAction = InputSystem.actions.FindAction("Equip");

        hudText.text = "";

        pickUpOrDropAction.started += OnPickUpOrDropActionStarted;
        equipAction.started += OnEquipActionStarted;
    }

    void OnDisable()
    {
        equipAction.started -= OnEquipActionStarted;
        pickUpOrDropAction.started -= OnPickUpOrDropActionStarted;
    }

    void FixedUpdate()
    {
        CheckLookingAtMask();
    }

    private void CheckLookingAtMask(bool debug = false)
    {
        var playerHead = Player.Head.transform;

        if (debug)
        {
            Debug.DrawRay(
                playerHead.position,
                playerHead.TransformDirection(Vector3.forward) * 1000,
                Color.red
            );
        }

        if (Physics.Raycast(
                playerHead.position,
                playerHead.TransformDirection(Vector3.forward),
                out var hit,
                pickUpMaskMaxDistance,
                Layer.ALL_MASKS_MASK
            ))
        {
            var hitObj = hit.collider.gameObject;
            lookingAtMask = hitObj.GetComponent<Mask>();
            hudText.text = hitObj.name;
        }
        else
        {
            lookingAtMask = null;
            hudText.text = "";
        }
    }

    private void OnPickUpOrDropActionStarted(InputAction.CallbackContext ctx)
    {
        if (heldMask != null)
            SetHeldMask(null);
        if (lookingAtMask != null)
            SetHeldMask(lookingAtMask);
    }

    private void OnEquipActionStarted(InputAction.CallbackContext ctx)
    {
        equippedMask?.OnUnequipped();

        // Swap held and equipped masks
        (heldMask, equippedMask) = (equippedMask, heldMask);

        if (equippedMask != null)
        {
            MoveMaskToEquippedPos(equippedMask);
            equippedMask.OnEquipped();
        }
        if (heldMask != null)
            MoveMaskToHeldPos(heldMask);
    }

    private void SetHeldMask(Mask mask)
    {
        if (mask == null)
        {
            if (heldMask == null)
                return;

            heldMask.GetCollider().enabled = true;
            var maskRb = heldMask.GameObject.GetComponent<Rigidbody>();
            if (maskRb)
                maskRb.isKinematic = false;
            MoveMaskWhenDropping(heldMask);
        }
        else
        {
            mask.GetCollider().enabled = false;
            var maskRb = mask.GameObject.GetComponent<Rigidbody>();
            if (maskRb)
                maskRb.isKinematic = true;
            MoveMaskWhenPickingUp(mask);
        }

        heldMask = mask;
    }

    private void MoveMaskWhenPickingUp(Mask mask)
    {
        var maskTransform = mask.GameObject.transform;
        var headTransform = Player.Head.transform;
        maskTransform.SetParent(headTransform);

        // Turn the mask toward the camera
        maskTransform.LookAt(maskTransform.position - headTransform.forward);
        // Shrink the mask
        heldMaskOriginalScale = maskTransform.localScale;
        maskTransform.localScale = heldMaskScaleMultiplier * heldMaskOriginalScale;

        MoveMaskToHeldPos(mask);
    }

    private void MoveMaskToHeldPos(Mask mask)
    {
        MoveMaskToScreenPos(mask, heldMaskScreenCornerOffset);
    }

    private void MoveMaskToEquippedPos(Mask mask)
    {
        MoveMaskToScreenPos(
            mask,
            new Vector3(Screen.width / 2f, Screen.height, 0)
            + equippedMaskScreenTopOffset
        );
    }

    private void MoveMaskToScreenPos(Mask mask, Vector3 pos)
    {
        var cam = cinemachineBrain.OutputCamera;
        var maskTransform = mask.GameObject.transform;
        var screenPos = new Vector3(pos.x, pos.y, cam.nearClipPlane + pos.z);
        maskTransform.position = cam.ScreenToWorldPoint(screenPos);
    }

    private void MoveMaskWhenDropping(Mask mask)
    {
        var maskTransform = mask.GameObject.transform;
        maskTransform.SetParent(null);

        maskTransform.localScale = heldMaskOriginalScale;
        maskTransform.position = Player.Head.transform.position
            + dropMaskDistance * Player.Head.transform.forward;
    }
}
