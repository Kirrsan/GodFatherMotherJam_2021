using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Greg");
    }
    public void Quit()
    {
        Debug.Log("Bisou");
        Application.Quit();
    }
}