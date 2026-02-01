using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName;
    private void OnTriggerEnter()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
