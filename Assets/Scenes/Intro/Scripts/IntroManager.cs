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
    public double spaceKeyDuration = 2.5f;
    public DateTime? spacePressTime;
    public Image circle;
    public Image fade;
    public Text spaceHintText;

    bool textIsOut;
    double spaceKeyTimeDif;

    private void Awake()
    {
        spaceHintText.color = new Color(255, 255, 255, 0);
        circle.fillAmount = 0;

    }
    void Start()
    {
        StartCoroutine(ShowSpaceHint());
    }

    IEnumerator ShowSpaceHint()
    {
        yield return new WaitForSeconds(10);
        spaceHintText.DOFade(1, 2f).SetEase(Ease.InBack);
    }

    // Update is called once per frame
    void Update()
    {
        SpacebarHandle();
    }

    private void SpacebarHandle()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spacePressTime = DateTime.Now;
            if (!textIsOut)
            {
                textIsOut = true;
                spaceHintText.DOFade(0, 1f).SetEase(Ease.InBack);
            }

        }

        if (Input.GetKey(KeyCode.Space) && spacePressTime.HasValue)
        {
            spaceKeyTimeDif = (DateTime.Now - spacePressTime).Value.TotalSeconds;
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
            circle.fillAmount = 0;
        }
    }

    private void SpacebarLongPress()
    {
        Debug.Log($"Long Press Space");
        fade.DOFade(1, .5f).SetEase(Ease.InBack).onComplete += () => sceneTransition.NextScene();
    }

}
