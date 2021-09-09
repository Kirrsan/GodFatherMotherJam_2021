using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad = "";


    public void Play()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    public void Quit()
    {
        Debug.Log("Bisou");
        Application.Quit();
    }
}