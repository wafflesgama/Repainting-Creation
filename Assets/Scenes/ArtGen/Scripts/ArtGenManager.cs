using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ArtGenManager : MonoBehaviour
{
    public GameObject[] paintings;
    public SceneTransition sceneTransition;
    public TextureContainer textureContainer;
    public double spaceKeyDuration = 2.5f;
    public DateTime? spacePressTime;
    public Image circle;
    public Image fade;
    double spaceKeyTimeDif;
    int currentPainting = 0;
    // Start is called before the first frame update
    void Start()
    {

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
            spaceKeyTimeDif = (DateTime.Now - spacePressTime).Value.TotalSeconds;
            float donePerc = (float)(spaceKeyTimeDif / spaceKeyDuration);
            Debug.Log($"donePerc {donePerc}");
            circle.fillAmount = donePerc;

            if (spaceKeyTimeDif > spaceKeyDuration)
            {
                spacePressTime = null;
                NextPainting();
            }

        }
        else
        {
            circle.fillAmount = 0;
        }
    }

    void NextPainting()
    {
        textureContainer.textures[currentPainting] = ScreenCapture.CaptureScreenshotAsTexture();

        currentPainting++;
        if (currentPainting >= 3)
            Finish();
        else
        {
            fade.DOFade(1, .5f).SetEase(Ease.InBack).onComplete += LoadNewPainting;
        }

    }


    void LoadNewPainting()
    {
        paintings[currentPainting - 1].SetActive(false);
        paintings[currentPainting].SetActive(true);
        fade.DOFade(0, .5f).SetEase(Ease.InBack);
    }


    private void Finish()
    {
        fade.DOFade(1, .5f).SetEase(Ease.InBack).onComplete += () => sceneTransition.NextScene();
    }
}
