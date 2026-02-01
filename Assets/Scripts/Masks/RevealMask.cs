using UnityEngine;

public class RevealMask : MonoBehaviour, Mask
{
    [SerializeField]
    private LayerMask affectedObjectLayer;

    public string DisplayedMaskName => "I see now 開ける";

    public void OnEquipped()
    {
        foreach (var item in FindObjectsByType<GameObject>(FindObjectsInactive.Include, sortMode: FindObjectsSortMode.None))
            if ((affectedObjectLayer.value & (1 << item.layer)) != 0)
                item.SetActive(true);
    }

    public void OnUnequipped()
    {
        foreach (var item in FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if ((affectedObjectLayer.value & (1 << item.layer)) != 0)
                item.SetActive(false);
    }
}
