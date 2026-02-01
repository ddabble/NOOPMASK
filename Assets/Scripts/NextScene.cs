using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(StudioEventEmitter))]
public class NextScene : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName;

    private StudioEventEmitter emitter;

    private void Awake()
    {
        emitter = GetComponent<StudioEventEmitter>();
    }

    private void OnTriggerEnter()
    {
        emitter.Play();
        SceneManager.LoadScene(nextSceneName);
    }
}
