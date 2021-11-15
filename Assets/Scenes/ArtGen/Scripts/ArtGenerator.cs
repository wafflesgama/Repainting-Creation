using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;

public class ArtGenerator : MonoBehaviour
{
    public VisualEffect visualEffect;
    public GestureInterpreter gestureInterpreter;

    public int numPaintings;

    public string AngleParamID;
    public string IntensityParamID;
    public string AlphaThresParamID;
    public string GravityParamID;

    public float AngleFactor = 2;
    public Vector3 AngleOffset;
    public float IntensityFactor = 1;
    public float AlphaFactor = 1;
    public float AlphaOffset = 1;
    public float GravityFactor = 2;

    //public string 

    int currentPainting=0;
    
    
    void Start()
    {
       
    }

    void Update()
    {
        visualEffect.SetVector3(AngleParamID, AngleOffset +(gestureInterpreter.facePosition* AngleFactor));
        visualEffect.SetFloat(IntensityParamID, gestureInterpreter.facePosition.z * IntensityFactor);
        visualEffect.SetFloat(GravityParamID, gestureInterpreter.facePosition.x * GravityFactor);
        visualEffect.SetFloat(AlphaThresParamID, gestureInterpreter.faceRotation.eulerAngles.z * AlphaFactor + AlphaOffset);
    }
}
