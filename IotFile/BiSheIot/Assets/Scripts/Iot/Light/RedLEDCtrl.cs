using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class RedLEDCtrl : MonoBehaviour
{

    private GComponent mainUI;
    private GButton changerButton;
    private GButton menuCtrl;
    private GTextField changerBtnTxt;

    private GTextInput Input1;
    private GTextInput Input2;
    private GTextInput Input3;
    private GTextInput Input4;
    private GTextInput Input5;
    void Start ()
    {
        mainUI = GetComponent<UIPanel>().ui;
        changerButton = mainUI.GetChild("LightOn-off").asButton;
        menuCtrl = mainUI.GetChild("Button").asButton;
        changerBtnTxt = mainUI.GetChild("On-offText").asTextField;

        Input1 = mainUI.GetChild("Input1").asTextInput;
        Input2 = mainUI.GetChild("Input2").asTextInput;
        Input3 = mainUI.GetChild("Input3").asTextInput;
        Input4 = mainUI.GetChild("Input4").asTextInput;
        Input5 = mainUI.GetChild("Input5").asTextInput;

        changerButton.onClick.Add(LEDChanger);
        menuCtrl.onClick.Add(MenuCtrl);
        //清空输入框按钮
        mainUI.GetChild("Clear").asButton.onClick.Add(()=>
        {
            Input1.text = "";
            Input2.text = "";
            Input3.text = "";
            Input4.text = "";
            Input5.text = "";
        });
        //连接按钮
        mainUI.GetChild("Con").asButton.onClick.Add(() =>
        {
            ClientAliyunIot.Instance.ProductKey = Input1.text;
            ClientAliyunIot.Instance.DeviceName = Input2.text;
            ClientAliyunIot.Instance.DeviceSecret = Input3.text;
            ClientAliyunIot.Instance.RegionId = Input4.text;
            ClientAliyunIot.Instance.Identifier = Input5.text;
            ClientAliyunIot.Instance.ConAndDisconBtnDown();
        });

        GetDefaultUser();
    }

    void Update()
    {
        GetLEDStatus();
        GetConStatus();
    }

    private void LEDChanger()
    {
        if (LightStatus.Instance.lightStatus)
        {
            LightStatus.Instance.lightStatus = false;

        }
        else
        {
            LightStatus.Instance.lightStatus = true;
        }
    }

    private void MenuCtrl()
    {
        Controller c1 = mainUI.GetController("c1");
        if (c1.selectedIndex == 0)
        {
            c1.selectedIndex = 1;
        }
        else
        {
            c1.selectedIndex = 0;
        }
    }

    private void GetLEDStatus()
    {
        if (LightStatus.Instance.lightStatus)
        {
            changerBtnTxt.text = "关灯";
        }
        else
        {
            changerBtnTxt.text = "开灯";
        }
    }

    private void GetDefaultUser()
    {
        Input1.text = ClientAliyunIot.Instance.ProductKey;
        Input2.text = ClientAliyunIot.Instance.DeviceName;
        Input3.text = ClientAliyunIot.Instance.DeviceSecret;
        Input4.text = ClientAliyunIot.Instance.RegionId;
        Input5.text = ClientAliyunIot.Instance.Identifier;
    }

    private void GetConStatus()
    {
        if (ClientAliyunIot.Instance.conStatus)
        {
            mainUI.GetChild("ConText").asTextField.text = "断开";
            mainUI.GetChild("Tip").asTextField.text = "当前状态：已连接";
            mainUI.GetChild("Tip").asTextField.color = Color.green;
        }
        else
        {
            mainUI.GetChild("ConText").asTextField.text = "连接";
            mainUI.GetChild("Tip").asTextField.text = "当前状态：未连接";
            mainUI.GetChild("Tip").asTextField.color = Color.red;
        }
    }
}
