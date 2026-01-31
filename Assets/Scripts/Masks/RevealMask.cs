using UnityEngine;
using UnityEngine.UI;

public class RevealMask : MonoBehaviour
{
    [SerializeField]
    private LayerMask affectedObjectLayer;

    void OnEnable()
    {
        foreach (var item in FindObjectsByType<GameObject>(sortMode: FindObjectsSortMode.None))
            if ((affectedObjectLayer.value & (1 << item.layer)) != 0)
                item.SetActive(false);
    }

    private void OnDisable()
    {
        foreach (var item in FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if ((affectedObjectLayer.value & (1 << item.layer)) != 0)
                item.SetActive(true);
    }
}
