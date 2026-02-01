using System.Collections;
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
    private Camera Camera => cinemachineBrain.OutputCamera;
    [SerializeField]
    private TMP_Text hudText;
    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private float pickUpMaskMaxDistance = 3.0f;
    [SerializeField]
    private float dropMaskDistance = 1.5f;
    private Mask lookingAtMask;
    private Mask heldMask;
    private Mask equippedMask;
    [SerializeField]
    private Vector3 heldMaskScreenBottomOffset;
    [SerializeField]
    private float heldMaskScaleMultiplier = 0.2f;

    void Start()
    {
        pickUpOrDropAction = InputSystem.actions.FindAction("Attack");
        equipAction = InputSystem.actions.FindAction("Equip");

        pickUpOrDropAction.started += OnPickUpOrDropActionStarted;
        equipAction.started += OnEquipActionStarted;

        StartCoroutine(WaitAndHideTitleCard());
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

    private IEnumerator WaitAndHideTitleCard()
    {
        yield return new WaitForSeconds(3);
        titleText.text = "";
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
                Layer.MASKS.mask
            ))
        {
            var hitObj = hit.collider.gameObject;
            lookingAtMask = hitObj.GetComponent<Mask>();
            hudText.text = lookingAtMask.DisplayedMaskName;
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
        {
            DropMask(heldMask);
            SetHeldMask(null);
        }
        if (lookingAtMask != null)
            SetHeldMask(lookingAtMask);
    }

    private void OnEquipActionStarted(InputAction.CallbackContext ctx)
    {
        var oldEquippedMask = equippedMask;
        SetEquippedMask(heldMask);
        SetHeldMask(oldEquippedMask);
    }

    private void SetHeldMask(Mask mask)
    {
        if (mask != null)
        {
            mask.GetCollider().enabled = false;
            var maskRb = mask.GameObject.GetComponent<Rigidbody>();
            if (maskRb)
                maskRb.isKinematic = true;
            MoveMaskToHeldPos(mask);
        }

        heldMask = mask;
    }

    private void SetEquippedMask(Mask mask)
    {
        if (equippedMask != null)
        {
            EquippedMaskCamera.Singleton.UnequipMask(equippedMask);
            equippedMask.OnUnequipped();
        }
        if (mask != null)
        {
            EquippedMaskCamera.Singleton.EquipMask(mask);
            mask.OnEquipped();
        }

        equippedMask = mask;
    }

    private void MoveMaskToHeldPos(Mask mask)
    {
        var maskTransform = mask.GameObject.transform;
        var headTransform = Player.Head.transform;
        maskTransform.SetParent(headTransform);

        MoveMaskToScreenPos(
            mask, new Vector3(Screen.width / 2f, 0, 0) + heldMaskScreenBottomOffset
        );

        // Turn the mask toward the camera
        maskTransform.LookAt(Camera.transform);
    }

    private void MoveMaskToScreenPos(Mask mask, Vector3 pos)
    {
        var maskTransform = mask.GameObject.transform;
        var screenPos = new Vector3(pos.x, pos.y, Camera.nearClipPlane + pos.z);
        maskTransform.position = Camera.ScreenToWorldPoint(screenPos);
    }

    private void DropMask(Mask mask)
    {
        var maskTransform = mask.GameObject.transform;
        maskTransform.SetParent(null);

        mask.GetCollider().enabled = true;
        var maskRb = maskTransform.GetComponent<Rigidbody>();
        if (maskRb)
            maskRb.isKinematic = false;

        maskTransform.position = Player.Head.transform.position
            + dropMaskDistance * Player.Head.transform.forward;
    }
}
