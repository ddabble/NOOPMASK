using UnityEngine;

public class HideMask : MonoBehaviour, Mask
{
    [SerializeField]
    private LayerMask affectedObjectLayer;

    public void OnEquipped()
    {
        foreach (var item in FindObjectsByType<GameObject>(FindObjectsInactive.Include, sortMode: FindObjectsSortMode.None))
            if ((affectedObjectLayer.value & (1 << item.layer)) != 0)
                item.SetActive(false);
    }

    public void OnUnequipped()
    {
        foreach (var item in FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if ((affectedObjectLayer.value & (1 << item.layer)) != 0)
                item.SetActive(true);
    }
}
