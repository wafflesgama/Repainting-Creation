using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureContainer : MonoBehaviour
{
    public Texture2D[] textures= new Texture2D[3];
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
