using System.Collections;
using UnityEngine;

public class CollisionForwarder : MonoBehaviour
{
    private Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();

        HideIfHideMaskIsEquipped();
    }

    private void HideIfHideMaskIsEquipped()
    {
        if (MaskHolder.Singleton.EquippedMask is not HideMask hideMask)
            return;

        if ((hideMask.AffectedObjectLayer.value & (1 << gameObject.layer)) != 0)
            gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision c)
    {
        if (Pendulum.Singleton.transform.GetChild(0).gameObject == c.gameObject)
        {
            StartCoroutine(WaitAndPush(Pendulum.Singleton.DispenseCount));
            Pendulum.Singleton.OnCollision(c);
        }
    }

    private IEnumerator WaitAndPush(int times)
    {
        yield return null;
        var force = times == 1 && Pendulum.Singleton.Sign < 0 ? -7f : -15f;
        body.AddForce(new Vector3(0f, 0f, force * Pendulum.Singleton.Sign * Mathf.Min(times, 2f)), ForceMode.Impulse);
    }
}
