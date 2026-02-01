using UnityEngine;

public class NoopMask : MonoBehaviour, Mask
{
    public string DisplayedMaskName => "Noopmask";

    public void OnEquipped()
    {}

    public void OnUnequipped()
    {}
}
