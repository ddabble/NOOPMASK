using FMODUnity;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))]
public class Pendulum : MonoBehaviour
{

    public static Pendulum Singleton;
    public int DispenseCount = 1;
    public int Sign = 1;

    private StudioEventEmitter emitter;

    void Awake()
    {
        emitter = GetComponent<StudioEventEmitter>();
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
    private void Update()
    {
        var newSign = transform.GetChild(0).localRotation.eulerAngles.x > 100 && transform.GetChild(0).localRotation.eulerAngles.x < 330 ? 1 : -1;

        if (newSign != Sign)
        {
            DispenseCount = 1;
        }
        Sign = newSign;
    }

    public void OnCollision(Collision collision)
    {
        DispenseCount++;
        transform.GetChild(0).GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, Sign * 15f), ForceMode.Impulse);
        emitter.Play();
        StartCoroutine(WaitAndDispenseNew());
    }

    private IEnumerator WaitAndDispenseNew()
    {
        yield return new WaitForSeconds(1f);
        emitter.Play();
        Dispenser.Singleton.Dispense();
    }
}
