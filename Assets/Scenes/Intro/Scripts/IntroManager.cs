using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public SceneTransition sceneTransition;
    public double spaceKeyDuration=2.5f;
    public DateTime? spacePressTime;
    public Image circle;
    public Image fade;

    double spaceKeyTimeDif;
    void Start()
    {
        circle.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        SpacebarHandle();
    }

    private void SpacebarHandle()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            spacePressTime = DateTime.Now;

        if (Input.GetKey(KeyCode.Space) && spacePressTime.HasValue)
        {
            spaceKeyTimeDif = (DateTime.Now- spacePressTime).Value.TotalSeconds;
            float donePerc = (float)(spaceKeyTimeDif / spaceKeyDuration);
            Debug.Log($"donePerc {donePerc}");
            circle.fillAmount = donePerc;

            if (spaceKeyTimeDif > spaceKeyDuration)
            {
                spacePressTime = null;
                SpacebarLongPress();
            }

        }
        else
        {
            circle.fillAmount =0;
        }
    }

    private void SpacebarLongPress()
    {
        Debug.Log($"Long Press Space");
        fade.DOFade(1, .5f).SetEase(Ease.InBack).onComplete += ()=> sceneTransition.NextScene();
    }

}
