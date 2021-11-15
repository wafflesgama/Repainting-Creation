using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneTransition : MonoBehaviour
{
    public OpenCvSharp.Demo.FaceTracker faceTracker;
    public string nextSceneName;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    public void NextScene()
    {
        faceTracker.Close();
        SceneManager.LoadSceneAsync(nextSceneName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
