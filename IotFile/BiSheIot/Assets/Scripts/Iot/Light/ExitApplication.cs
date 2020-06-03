using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitApplication : MonoBehaviour
{
    private static ExitApplication instance;

    public static ExitApplication Instance
    {
        get
        {
            return instance;
        }

        set
        {
            instance = value;
        }
    }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {

    }

    public void Exit()
    {
        //#if UNITY_EDITOR
        //EditorApplication.isPlaying = false;
        //Debug.Log("编辑状态游戏退出");
        //#else
        //    Debug.Log ("游戏退出"):
        //    Application.Quit();
        //#endif
        Application.Quit();
    }


}
