using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLogo : MonoBehaviour
{
    private GComponent mainUI;

    void Awake()
    {
        UIConfig.defaultFont = "汉仪南宫体简";
        FontManager.RegisterFont(FontManager.GetFont("汉仪南宫体简"), "HYNanGongJ");
    }

    void Start()
    {
        mainUI = GetComponent<UIPanel>().ui;
        Screen.SetResolution(1600, 900, false);
        AsyncOperation async = SceneManager.LoadSceneAsync(1);
        async.allowSceneActivation = false;
        mainUI.GetTransition("t0").Play(() => async.allowSceneActivation = true);
    }

}
