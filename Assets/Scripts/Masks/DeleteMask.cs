using UnityEngine;

public class DeleteMask : MonoBehaviour, Mask
{
    [SerializeField]
    private LayerMask affectedObjectLayer;
    public LayerMask AffectedObjectLayer => affectedObjectLayer;

    public string DisplayedMaskName => "delete this 死ぬ";

    public void OnEquipped()
    {
        Application.Quit();
    }

    public void OnUnequipped()
    {
    }
}
