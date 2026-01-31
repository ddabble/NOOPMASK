using UnityEngine;

public interface Mask
{
    public GameObject GameObject => ((MonoBehaviour)this).gameObject;

    public Collider GetCollider()
    {
        return GameObject.GetComponent<Collider>();
    }

    public void OnEquipped();
    public void OnUnequipped();
}
