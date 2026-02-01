using UnityEngine;

public interface Mask
{
    public GameObject GameObject => ((MonoBehaviour)this).gameObject;

    public Collider GetCollider()
    {
        return GameObject.GetComponent<Collider>();
    }

    public string DisplayedMaskName { get; }

    public void OnEquipped();
    public void OnUnequipped();
}
