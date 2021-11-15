using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class FaceDetector : MonoBehaviour
{
    public MeshRenderer rend;
    WebCamTexture webCamTexture;
    CascadeClassifier cascade;
    Mat matFrame;
    OpenCvSharp.Rect myFace;

    void Start()
    {
        WebCamDevice[] devices= WebCamTexture.devices;
        webCamTexture=new WebCamTexture(devices[0].name);
        webCamTexture.Play();
        cascade = new CascadeClassifier(Application.dataPath + @"/haarcascade_frontalface_default.xml");
    }

    void Update()
    {
        if (!webCamTexture.isPlaying) return;

        matFrame = OpenCvSharp.Unity.TextureToMat(webCamTexture);
        FindNewFace(matFrame);
        DisplayRect(matFrame);

    }

    void FindNewFace(Mat frame)
    {
        if (cascade == null) return;
        var faces = cascade.DetectMultiScale(frame, 1.1, 2, HaarDetectionType.FindBiggestObject);
        if(faces.Length > 0)
        {
            myFace = faces[0];
        }
    }

    void DisplayRect(Mat frame)
    {
        

        if (myFace != null)
        {
            frame.Rectangle(myFace,new Scalar(250,0,0),2);
            Texture exportTexture = OpenCvSharp.Unity.MatToTexture(matFrame);
            rend.material.mainTexture = exportTexture;
        }
    }
}
