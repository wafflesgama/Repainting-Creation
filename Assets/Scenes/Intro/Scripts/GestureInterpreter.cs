using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureInterpreter : MonoBehaviour
{
    [Header("Left Eye Trackers")]
    [SerializeField] private Transform leftTopEyeTracker;
    [SerializeField] private Transform leftCentEyeTracker;
    [SerializeField] private Transform leftBotEyeTracker;

    [Header("Right Eye Trackers")]
    [SerializeField] private Transform rightTopEyeTracker;
    [SerializeField] private Transform rightCentEyeTracker;
    [SerializeField] private Transform rightBotEyeTracker;

    [Header("Nose Trackers")]
    public Transform noseBridgTopTracker;
    public Transform noseBridgBotTracker;
    public Transform noseTracker;

    //[Header("Center Trackers")]
    //[SerializeField] private Transform centerTracker;


    [Header("Default Params")]
    public float noseBridgDefHeight = 22;
    public float eyeLineDefWidth = 48;
    public float zPosFactor = .01f;
    public float xRotFactor = .01f;
    public float yRotFactor = .01f;
    public float negNoseFactor = 1.5f;
    public float eyelidDist = 2.3f;
    public float minEyelidDist = 3.2f;
    public float blinkDuration = 2f;
    public float blinkScaleFactor = .2f;

    [Header("Exposed Params")]
    public Vector3 facePosition;
    public Quaternion faceRotation;
    public bool hasStarted;
    public bool hasEyesClosed;
    public bool hasOpenedEyesAfterBlink;
    public DateTime blinkTime;

    public Action blinkAction;

    void Start()
    {
        hasStarted = false;
        hasEyesClosed = false;
        hasOpenedEyesAfterBlink = true;
    }

    void Update()
    {
        if (!hasStarted && noseTracker.position.y != 0)
            hasStarted = true;

        if (!hasStarted) return;

        var eyeLineDir = rightCentEyeTracker.position - leftCentEyeTracker.position;
        var noseBridgYDistance = noseBridgTopTracker.position.y - noseBridgBotTracker.position.y;
        var eyeLineXDistance = leftCentEyeTracker.position.x - rightCentEyeTracker.position.x;

        var eyeDistRef = eyeLineDefWidth - eyeLineXDistance;
        var noseDistRef = noseBridgDefHeight - noseBridgYDistance;
        noseDistRef = noseDistRef < 0 ? noseDistRef * negNoseFactor : noseDistRef;


        float scaleRef;
        if (eyeDistRef < 0 || noseDistRef < 0)
            scaleRef = eyeDistRef < noseDistRef ? eyeDistRef : noseDistRef; //In case zoom in face the biggest distance is considered  (ref is oposite)
        else
            scaleRef = eyeDistRef > noseDistRef ? eyeDistRef : noseDistRef; //In case zoom out face the shortest distance is considered   (ref is oposite)

        scaleRef *= -1;


        // Debug.Log($"noseBridgYDistance: {noseBridgYDistance} | eyeLineXDistance: {eyeLineXDistance} | scaleRef: {scaleRef}");


        facePosition = Centerof3(rightCentEyeTracker.position, leftCentEyeTracker.position, noseTracker.position);
        facePosition.z = scaleRef * zPosFactor;

        faceRotation = Quaternion.Euler(
                                        noseDistRef * xRotFactor * scaleRef,
                                        -eyeDistRef * yRotFactor * scaleRef,
                                        Vector3.Angle(Vector3.up, eyeLineDir));

        var rEyelidDist = Vector2.Distance(rightTopEyeTracker.position, rightBotEyeTracker.position);
        var lEyelidDist = Vector2.Distance(leftTopEyeTracker.position, leftBotEyeTracker.position);
        //Debug.Log($"Eyelid distance R: {rEyelidDist}  ! L: {lEyelidDist}");

        var scaledMinEyelidDist = minEyelidDist +(scaleRef * blinkScaleFactor);
        var scaledEyelidDist = eyelidDist + (scaleRef * blinkScaleFactor);

        //Debug.Log($"scaledMinEyelidDist {scaledMinEyelidDist}, scaledEyelidDist {scaledEyelidDist}");
        //Debug.Log($"rEyelidDist {rEyelidDist}, lEyelidDist {lEyelidDist}");

        if (rEyelidDist < scaledMinEyelidDist && lEyelidDist < scaledEyelidDist || rEyelidDist < scaledEyelidDist && lEyelidDist < scaledMinEyelidDist)
        {
            //Debug.Log("Distance is considered closed");
            //Debug.Log("Distance is considered closed");


            if (hasOpenedEyesAfterBlink)
            {
                if (!hasEyesClosed)
                {
                    hasEyesClosed = true;
                    blinkTime = DateTime.Now;
                }
                else
                {
                    var blinkDif = DateTime.Now - blinkTime;
                    if (blinkDif.TotalSeconds > blinkDuration)
                    {
                        Debug.LogWarning("Blink action");
                        hasEyesClosed = false;
                        hasOpenedEyesAfterBlink = false;
                        blinkAction?.Invoke();
                    }
                }
            }
        }
        else
        {
            hasOpenedEyesAfterBlink = true;
           // Debug.Log("Distance is considered not closed");
        }

    }
    Vector3 Centerof3(Vector3 p1, Vector3 p2, Vector3 p3) => new Vector3((p1.x + p2.x + p3.x) / 3, (p1.y + p2.y + p3.y) / 3, (p1.z + p2.z + p3.z) / 3);
}
