using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesChanger : MonoBehaviour
{
    public void ChangeScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
    
    public void Exid()
    {
        Application.Quit();
    }
}
