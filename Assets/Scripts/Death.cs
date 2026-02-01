using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        CheckCollisionObjAndReload(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckCollisionObjAndReload(other.gameObject);
    }

    private void CheckCollisionObjAndReload(GameObject obj)
    {
        if (obj.layer == Layer.DEFAULT)
            return;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
