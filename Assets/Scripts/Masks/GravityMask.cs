using System.Collections.Generic;
using UnityEngine;

public class GravityMask : MonoBehaviour, Mask
{
    [SerializeField]
    private LayerMask affectedObjectLayer;

    [SerializeField]
    private float upwardGravityStrength = 9.81f;

    private Dictionary<Rigidbody, RigidbodyState> affectedBodies =
        new Dictionary<Rigidbody, RigidbodyState>();

    public string DisplayedMaskName => "逆重力 Anti-Gravity";

    private struct RigidbodyState
    {
        public bool useGravity;
        public Vector3 linearVelocity;
        public Vector3 angularVelocity;
    }

    public void OnEquipped()
    {
        foreach (var item in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if ((affectedObjectLayer.value & (1 << item.layer)) == 0)
                continue;

            if (!item.TryGetComponent<Rigidbody>(out var body))
                continue;

            if (affectedBodies.ContainsKey(body))
                continue;

            affectedBodies.Add(body, new RigidbodyState
            {
                useGravity = body.useGravity,
                linearVelocity = body.linearVelocity,
                angularVelocity = body.angularVelocity
            });

            body.useGravity = false;
            body.excludeLayers = Layer.PLAYER.mask;
        }
    }

    private void FixedUpdate()
    {
        foreach (var kvp in affectedBodies)
        {
            if (kvp.Key == null)
                continue;

            kvp.Key.AddForce(Vector3.up * upwardGravityStrength, ForceMode.Acceleration);
        }
    }

    public void OnUnequipped()
    {
        foreach (var item in affectedBodies)
        {
            if (item.Key == null)
                continue;

            item.Key.useGravity = item.Value.useGravity;
            item.Key.linearVelocity = item.Value.linearVelocity;
            item.Key.angularVelocity = item.Value.angularVelocity;
            item.Key.excludeLayers = 0;
        }

        affectedBodies.Clear();
    }
}
