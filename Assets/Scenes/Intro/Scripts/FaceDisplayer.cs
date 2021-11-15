using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenCvSharp.Demo;
using UnityEngine;

public class FaceDisplayer : MonoBehaviour
{
    public FaceTracker faceTracker;
    public LineRenderer lineRenderer;
    public float lerpSpeed=25;
    public Vector3 faceOffset;
    Vector3[] lerpPos = new Vector3[70];

    void Start()
    {
        DelayedStart();
    }

    public async void DelayedStart()
    {
        await Task.Delay(500);
        if (faceTracker.placeholders.Count < 2)
            await Task.Delay(500);

        var posArray = faceTracker.placeholders.Select(x => x.position).ToArray();
        lineRenderer.positionCount = posArray.Length-1;
        lineRenderer.SetPositions(posArray);
    }

    void Update()
    {
        for (int i = 0; i < faceTracker.placeholders.Count; i++)
            lerpPos[i] = Vector3.Lerp(lerpPos[i],faceTracker.placeholders[i].position + faceOffset, Time.deltaTime*lerpSpeed);

        lineRenderer.SetPositions(lerpPos);
    }
}
