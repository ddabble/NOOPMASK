using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerMovement;

public class MaskHolder : MonoBehaviour
{
    public static MaskHolder Singleton;

    private static readonly int IsHoldingMask = Animator.StringToHash("IsHoldingMask");
    private static readonly int EquipMask = Animator.StringToHash("EquipMask");

    private InputAction pickUpOrDropAction;
    private InputAction equipAction;

    [SerializeField]
    private CinemachineBrain cinemachineBrain;
    private Camera Camera => cinemachineBrain.OutputCamera;
    [SerializeField]
    private TMP_Text maskPickupText;
    [SerializeField]
    private TMP_Text equippedMaskText;
    private Coroutine equippedMaskTextAnimationCoroutine;
    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private float pickUpMaskMaxDistance = 3.0f;
    [SerializeField]
    private float dropMaskDistance = 1.5f;
    private Mask lookingAtMask;
    private Mask heldMask;
    private Mask equippedMask;
    public Mask EquippedMask => equippedMask;
    [SerializeField]
    private Vector3 heldMaskScreenBottomOffset;
    [SerializeField]
    private float heldMaskScaleMultiplier = 0.2f;
    [SerializeField]
    private Animator animator;

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
        pickUpOrDropAction = InputSystem.actions.FindAction("Attack");
        equipAction = InputSystem.actions.FindAction("Equip");

        maskPickupText.text = "";
        equippedMaskText.text = "";

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
            maskPickupText.text = lookingAtMask.DisplayedMaskName;
        }
        else
        {
            lookingAtMask = null;
            maskPickupText.text = "";
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
        {
            SetHeldMask(lookingAtMask);
        }
    }

    private void OnEquipActionStarted(InputAction.CallbackContext ctx)
    {
        var oldEquippedMask = equippedMask;
        SetEquippedMask(heldMask);
        SetHeldMask(oldEquippedMask);
    }

    private void SetHeldMask(Mask mask)
    {
        if (mask == null)
        {
            animator.SetBool(IsHoldingMask, false);
        }
        else
        {
            mask.GetCollider().enabled = false;
            var maskRb = mask.GameObject.GetComponent<Rigidbody>();
            if (maskRb)
                maskRb.isKinematic = true;
            MoveMaskToHeldPos(mask);

            animator.SetBool(IsHoldingMask, true);
        }

        heldMask = mask;
    }

    private void SetEquippedMask(Mask mask)
    {
        if (equippedMask != null)
        {
            EquippedMaskCamera.Singleton.UnequipMask(equippedMask);
            equippedMask.OnUnequipped();

            StopCoroutine(equippedMaskTextAnimationCoroutine);
            equippedMaskText.text = "";
        }
        if (mask != null)
        {
            EquippedMaskCamera.Singleton.EquipMask(mask);
            mask.OnEquipped();
            animator.SetTrigger(EquipMask);

            equippedMaskText.text = mask.DisplayedMaskName;
            equippedMaskTextAnimationCoroutine = StartCoroutine(AnimateEquippedMaskText());
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

        maskTransform.position = FindClosestFreeSpaceAbove(
            Player.Head.transform.position
            + dropMaskDistance * Player.Head.transform.forward
        );
    }

    private Vector3 FindClosestFreeSpaceAbove(Vector3 pos)
    {
        if (Physics.Raycast(
                pos + 10f * Vector3.up,
                Vector3.down,
                out var hit,
                1000,
                Layer.DEFAULT.mask
            ))
        {
            var newPos = hit.point + 0.5f * Vector3.up;
            if (newPos.y > pos.y)
                return newPos;
        }
        return pos;
    }

    private IEnumerator AnimateEquippedMaskText()
    {
        var textMesh = equippedMaskText;
        while (true)
        {
            textMesh.ForceMeshUpdate();
            var textInfo = textMesh.textInfo;
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;

                var charVertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                for (int j = 0; j < 4; j++)
                {
                    var position = charVertices[charInfo.vertexIndex + j];
                    charVertices[charInfo.vertexIndex + j] = position + new Vector3(0f, Mathf.Sin(Time.time * 16f + position.x * 0.1f) * 1f, 0f);
                    Color32 gradient = Color.white;
                    textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[charInfo.vertexIndex + j] = gradient;
                }
            }
            textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                textMesh.UpdateGeometry(meshInfo.mesh, i);
            }
            yield return null;
        }
    }
}
