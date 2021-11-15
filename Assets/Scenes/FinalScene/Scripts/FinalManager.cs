using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class FinalManager : MonoBehaviour
{
    public Material paintingMat1;
    public Material paintingMat2;
    public Material paintingMat3;

    public GameObject[] people;

    public Image fade;


    TextureContainer textureContainer;

    private void Awake()
    {
        fade.color = Color.black;

    }
    void Start()
    {
        fade.DOFade(0,.5f).SetEase(Ease.InBack);
        var rndIndex = Random.Range(0, people.Length);

        for (int i = 0; i < people.Length; i++)
            people[i].SetActive(i == rndIndex);

        textureContainer = FindObjectOfType<TextureContainer>();

        paintingMat1.mainTexture = textureContainer.textures[0];
        paintingMat2.mainTexture = textureContainer.textures[1];
        paintingMat3.mainTexture = textureContainer.textures[2];
    }

    public void ClickQuit()
    {
        fade.DOFade(1, .5f).SetEase(Ease.InBack).onComplete += () => Application.Quit();
    }

    public void ClickRetry()
    {
        fade.DOFade(1, .5f).SetEase(Ease.InBack).onComplete += () => SceneManager.LoadSceneAsync("Intro");
    }

}
