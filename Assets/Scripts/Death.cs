using SteamAudio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
