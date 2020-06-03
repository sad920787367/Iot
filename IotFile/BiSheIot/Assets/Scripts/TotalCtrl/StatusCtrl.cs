using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class StatusCtrl : MonoBehaviour
{
    private static StatusCtrl instance;

    public static StatusCtrl Instance
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

    public int CameraIndex { get; set; }//摄像机位 0是原位 1是书本位 2是桌子位
    public int UIStatusIndex { get; set; }//当前场景UI所处状态：0-初进入，1-点击任意键，2-进入书本时,3-桌子处UI

    void Awake()
    {
        instance = this;
        UIConfig.defaultFont = "汉仪南宫体简";
        FontManager.RegisterFont(FontManager.GetFont("汉仪南宫体简"), "HYNanGongJ");
    }
}
