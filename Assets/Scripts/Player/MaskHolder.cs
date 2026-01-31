using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerMovement;

public class MaskHolder : MonoBehaviour
{
    private InputAction pickUpAction;
    private InputAction equipAction;
    private InputAction dropAction;

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
    private bool isMaskEquipped = false;
    private Vector3 heldMaskOriginalScale;
    [SerializeField]
    private Vector3 heldMaskScreenCornerOffset;
    [SerializeField]
    private float heldMaskScaleMultiplier = 0.2f;

    void Start()
    {
        pickUpAction = InputSystem.actions.FindAction("Attack");
        equipAction = InputSystem.actions.FindAction("Interact");
        dropAction = InputSystem.actions.FindAction("Drop");

        hudText.text = "";

        pickUpAction.started += OnPickUpActionStarted;
        equipAction.started += OnEquipActionStarted;
        dropAction.started += OnDropActionStarted;
    }

    void OnDisable()
    {
        dropAction.started -= OnDropActionStarted;
        equipAction.started -= OnEquipActionStarted;
        pickUpAction.started -= OnPickUpActionStarted;
    }

    void FixedUpdate()
    {
        CheckLookingAtMask();
    }

    private void CheckLookingAtMask(bool debug = true)
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
                Layer.MASKS.mask
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

    private void OnPickUpActionStarted(InputAction.CallbackContext ctx)
    {
        if (lookingAtMask == null || heldMask != null)
            return;

        SetHeldMask(lookingAtMask);
    }

    private void OnEquipActionStarted(InputAction.CallbackContext ctx)
    {
        if (heldMask == null)
            return;

        if (isMaskEquipped)
            heldMask.OnUnequipped();
        else
            heldMask.OnEquipped();
        isMaskEquipped = !isMaskEquipped;
    }

    private void OnDropActionStarted(InputAction.CallbackContext ctx)
    {
        if (heldMask == null)
            return;

        var oldMask = heldMask;
        // Restore various components before calling `OnUnequipped()`
        SetHeldMask(null);
        if (isMaskEquipped)
        {
            oldMask.OnUnequipped();
            isMaskEquipped = false;
        }
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

        // Place the mask in the bottom left screen corner
        var cam = cinemachineBrain.OutputCamera;
        var bottomLeftScreenPos = new Vector3(
            heldMaskScreenCornerOffset.x,
            heldMaskScreenCornerOffset.y,
            cam.nearClipPlane + heldMaskScreenCornerOffset.z
        );
        maskTransform.position = cam.ScreenToWorldPoint(bottomLeftScreenPos);
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
