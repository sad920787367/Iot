using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class LightCtl : MonoBehaviour
{
    private GComponent mainUI;
    private GObject onButton;
    private GObject offButton;
    private GObject conAndDisconButton;
    private GObject conBtnText;
    private Controller ctrlmainUI;

    //private bool isQuit = false;
    //private bool isMenuEnter = false;

    void Start()
    {
        //控制器c1的0页面相关UI处理
        mainUI = GetComponent<UIPanel>().ui;
        onButton = mainUI.GetChild("TurnOnBtn");
        offButton = mainUI.GetChild("TurnOffBtn");
        conAndDisconButton = mainUI.GetChild("ConBtn");
        conBtnText = mainUI.GetChild("ConText");
        ctrlmainUI = mainUI.GetController("c1");

        //c1 0页点击事件绑定
        onButton.onClick.Add(LightOnBtn);
        offButton.onClick.Add(LightOffBtn);
        conAndDisconButton.onClick.Add(AboutConBtnThing);

        //c1 1页点击事件绑定
        mainUI.GetChild("QuitYes").onClick.Add(QuitYes);
        mainUI.GetChild("QuitNo").onClick.Add(QuitNo);

        //退出时处理的事情

    }

    void OnDestroy()
    {
        if (ClientAliyunIot.Instance.conStatus)
        {
            ClientAliyunIot.Instance.ConAndDisconBtnDown();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsQuit();
        }
    }
    
    //开关灯事件
    private void LightOnBtn()
    {
        LightStatus.Instance.lightStatus = true;
    }
    private void LightOffBtn()
    {
        LightStatus.Instance.lightStatus = false;
    }

    //连接按钮事件
    private void AboutConBtnThing()
    {
        ClientAliyunIot.Instance.ConAndDisconBtnDown();
        bool conStatus = ClientAliyunIot.Instance.conStatus;
        if (conStatus)
        {
            conBtnText.text = "断开";
        }
        else
        {
            conBtnText.text = "连接";
        }
    }

    //用户按ESC退出时的处理
    private void IsQuit()
    {
        Debug.Log("执行成功");
        Transition QuitMenuDX = mainUI.GetTransition("QuitMenuEnter");
        ctrlmainUI.selectedIndex = 1;
        QuitMenuDX.Play();
    }

    private void QuitYes()
    {
        //isQuit = true;
        ExitApplication.Instance.Exit();
    }

    private void QuitNo()
    {
        Transition QuitMenuDX = mainUI.GetTransition("QuitMenuCancel");
        QuitMenuDX.Play(() => { ctrlmainUI.selectedIndex = 0;});
    }
}
