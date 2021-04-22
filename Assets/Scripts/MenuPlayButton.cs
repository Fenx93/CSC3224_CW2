using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayButton : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;

    public void LoadSceneOnButtonClick(Object scene)
    {
        SceneManager.LoadScene(scene.name);
    }        

    public void OpenMenu()
    {

    }

    public void CloseMenu()
    {

    }
}
