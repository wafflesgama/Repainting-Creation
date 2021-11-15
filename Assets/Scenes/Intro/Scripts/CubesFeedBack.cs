using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubesFeedBack : MonoBehaviour
{
    public bool isMainObject;
    public GestureInterpreter gInterpreter;

    float scaleFactor = .5f;
    float minScale = 150;
    float yPosOffset=90;
    float scaleLerpSpeed = .02f;
    public Material objectMaterial;
    Rigidbody rb;
    float objectScale;
    float rotLerpSpeed = .01f;
    float rotSpeed = .9f;
    float hueColor = 0;
    // Start is called before the first frame update
    void Start()
    {
        //if (isMainObject)
        //    objectMaterial.EnableKeyword("_EMISSION");

        objectScale = transform.localScale.x;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.down * rb.mass * 19);
        rb.AddForce(Vector3.left * rb.mass * 40);
        //rb.AddTorque(new Vector3(Random.Range(10, 160), Random.Range(10, 180), Random.Range(8, 190)));
    }

    private void FixedUpdate()
    {
        //if (gInterpreter.facePosition.y != 0)
        //{
        //    var newScale = (objectScale * (gInterpreter.facePosition.y+ yPosOffset) * scaleFactor) + minScale;
        //    //Debug.Log($"facePosition.y: {gInterpreter.facePosition.y}| newScale: {newScale}");
        //    objectScale =Mathf.Clamp(Mathf.Lerp(objectScale, newScale, scaleLerpSpeed),2,120);
        //}

        //if (gInterpreter.facePosition.y != 0)
        //    rb.AddRelativeTorque(gInterpreter.facePosition.x * 0.001f, gInterpreter.facePosition.x * 0.005f, 0);

        //if (isMainObject)
        //{
        //    hueColor= Mathf.Lerp(hueColor,gInterpreter.faceRotation.z, rotLerpSpeed);
        //    Color color = Color.HSVToRGB(Mathf.Clamp(hueColor, 0,359), .5f, .85f);
        //    objectMaterial.color = color;
        //    //objectMaterial.SetColor("_EmissionColor", color);
        //}

        //transform.localScale = new Vector3(objectScale, objectScale, objectScale);
    }
}
