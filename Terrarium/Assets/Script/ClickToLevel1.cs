using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToLevel1 : MonoBehaviour
{
    public string sceneToLoad;

    void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
