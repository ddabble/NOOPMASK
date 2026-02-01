using Unity.VisualScripting;
using UnityEngine;

public class Dispenser : MonoBehaviour
{
    [SerializeField]
    private GameObject itemToDispense;
    public static Dispenser Singleton;
    void Awake()
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

    public void Dispense()
    {
        var cube = Instantiate(itemToDispense, transform);
        cube.transform.position = transform.position;
    }
}
