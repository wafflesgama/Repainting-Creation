using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParallax : MonoBehaviour
{
    [Header("Camera Parameters")]
    public GestureInterpreter gestureInterpreter;

    [Header("Camera Parameters")]
    public Transform moveableCamera;
    public Vector3 moveOffset;
    public float moveSpeed;
    public float moveLerpSpeed;

    public Vector3 rotOffset;
    public float rotSpeed;
    public float rotLerpSpeed;


   
    void Start()
    {
        
    }

    void Update()
    {
        if (!gestureInterpreter.hasStarted) return;

        moveableCamera.position = Vector3.Lerp(moveableCamera.position, gestureInterpreter.facePosition * moveSpeed + moveOffset, Time.deltaTime * moveLerpSpeed);
        moveableCamera.rotation = Quaternion.Lerp(moveableCamera.rotation, Quaternion.Euler(gestureInterpreter.faceRotation.eulerAngles + rotOffset) , Time.deltaTime * rotLerpSpeed);
    }
}
