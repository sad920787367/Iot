using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using FairyGUI;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PeiZhiWindow : Window
{
    private List<string> fangAn;
    private GameObject mainObj;
    public PeiZhiWindow(List<string> list)
    {
        fangAn = list;
    }

    protected override void OnInit()
    {
        this.contentPane = UIPackage.CreateObject("Main", "ConfigWindow").asCom;
        contentPane.SetSize(800, 600);
        contentPane.SetXY(80, 10);
        WindowLoad();
    }

    protected override void OnShown()
    {
        mainObj = ClientAliyunIot.Instance.MainCObj.transform.parent.gameObject;
        if (mainObj.transform.name == "DHT11")
        {
            contentPane.GetController("c1").selectedIndex = 1;
            contentPane.GetChild("title6").text = "TempIdentifier";
        }
        else
        {

            contentPane.GetController("c1").selectedIndex = 0;
            contentPane.GetChild("title6").text = "Identifier";
        }
    }


    private void WindowLoad()
    {
        GComboBox combo = contentPane.GetChild("n1").asComboBox;
        combo.items = fangAn.ToArray();

        contentPane.GetChild("n34").onClick.Add((() =>
        {
            for (int i = 1; i <= 6; i++)
            {
                if (i==5)
                {
                    contentPane.GetChild("shuru5").asComboBox.selectedIndex = 0;
                    continue;
                }
                GTextInput textInput = contentPane.GetChild("shuru" + i).asTextInput;
                textInput.text = "";
            }
        }));//清空按钮
        contentPane.GetChild("n2").onClick.Add(() =>
        {
            SaveData(combo);
            SaveShuRu(combo);
        });//保存按钮
        contentPane.GetChild("n36").onClick.Add(() =>
        {
            if (combo.items.Length>1)
            {
                DeleteShuRu(combo);
                fangAn.Remove(combo.title);
                PlayerPrefs.DeleteKey("Save"+combo.selectedIndex);
                combo.items = fangAn.ToArray();
                PlayerPrefs.SetInt("SaveComboCount", combo.items.Length);
            }
        });//删除按钮
        contentPane.GetChild("n37").onClick.Add(()=>
        {
            EditData(combo);
            SaveShuRu(combo);
        });//修改按钮
        contentPane.GetChild("n3").onClick.Add(() =>
        {
            LoadShuRu(combo);
        });//读取按钮

        contentPane.GetChild("n33").onClick.Add(() =>
        {
            ConnectAliyun();
        });//连接按钮
    }

    /// <summary>
    /// 此处存档
    /// 方案名：Save+下拉框序号 下拉框选项数：SaveComboCount
    /// </summary>
    /// <param name="combo"></param>
    private void SaveData(GComboBox combo)
    {
        if (contentPane.GetChild("shuru1").text == "")
        {
            fangAn.Add("方案" + combo.items.Length);
        }
        else
        {
            fangAn.Add(contentPane.GetChild("shuru1").text);
        }
        combo.items = fangAn.ToArray();
        combo.selectedIndex = combo.items.Length - 1;
        PlayerPrefs.SetString("Save"+combo.selectedIndex,combo.title);
        Debug.Log(combo.selectedIndex);
        PlayerPrefs.SetInt("SaveComboCount",combo.items.Length);
    }

    private void EditData(GComboBox combo)
    {
        if (combo.selectedIndex == 0)
        {
            return;
        }
        combo.title = contentPane.GetChild("shuru1").text;
        fangAn[combo.selectedIndex] = contentPane.GetChild("shuru1").text;
        combo.items = fangAn.ToArray();
        PlayerPrefs.SetString("Save" + combo.selectedIndex, combo.title);
    }

    /// <summary>
    /// 存档内容
    /// 各个输入框的数据存档名为方案编号+输入框编号
    /// </summary>
    /// <param name="combo"></param>
    private void SaveShuRu(GComboBox combo)
    {
        int ShuRuCount = 6;
        if (mainObj.transform.name == "DHT11")
        {
            ShuRuCount = 7;
        }
        for (int i = 1; i <= ShuRuCount; i++)
        {
            if (i==5)
            {
                PlayerPrefs.SetInt(combo.selectedIndex + "-" + i, contentPane.GetChild("shuru5").asComboBox.selectedIndex);
                continue;
            }

            PlayerPrefs.SetString(combo.selectedIndex + "-" + i, contentPane.GetChild("shuru" + i).text);
        }
    }

    private void DeleteShuRu(GComboBox combo)
    {
        int ShuRuCount = 6;
        if (mainObj.transform.name == "DHT11")
        {
            ShuRuCount = 7;
        }
        for (int i = 1; i <= ShuRuCount; i++)
        {
            PlayerPrefs.DeleteKey(combo.selectedIndex + "-" + i);
            Debug.Log(combo.selectedIndex + "-" + i);
        }
    }

    private void LoadShuRu(GComboBox combo)
    {
        int ShuRuCount = 6;
        if (mainObj.transform.name == "DHT11")
        {
            ShuRuCount = 7;
        }
        for (int i = 1; i <= ShuRuCount; i++)
        {
            if (i == 5)
            {
                int index = PlayerPrefs.GetInt(combo.selectedIndex + "-" + i, 0);
                contentPane.GetChild("shuru5").asComboBox.selectedIndex = index;
                continue;
            }

            contentPane.GetChild("shuru"+i).text = PlayerPrefs.GetString(combo.selectedIndex + "-" + i, "数据存档错误");
        }
    }

    private void ConnectAliyun()
    {
        bool isNull = false;
        ClientAliyunIot clientAliyunIot = ClientAliyunIot.Instance;
        GButton button = contentPane.GetChild("n33").asButton;
        int ShuRuCount = 6;
        if (mainObj.transform.name == "DHT11")
        {
            ShuRuCount = 7;
        }
        if (!clientAliyunIot.conStatus)
        {
            clientAliyunIot.ProductKey = contentPane.GetChild("shuru2").text;
            clientAliyunIot.DeviceName = contentPane.GetChild("shuru3").text;
            clientAliyunIot.DeviceSecret = contentPane.GetChild("shuru4").text;
            clientAliyunIot.RegionId = contentPane.GetChild("shuru5").asComboBox.value;
            clientAliyunIot.Identifier = contentPane.GetChild("shuru6").text;
            clientAliyunIot.HumiIdentifier = contentPane.GetChild("shuru7").text;
            for (int i = 2; i <= ShuRuCount; i++)
            {
                if (i==5)
                {
                    contentPane.GetChild("shuru" + i).asComboBox.titleColor = Color.red;
                    contentPane.GetChild("shuru" + i).asComboBox.onChanged.Clear();
                    contentPane.GetChild("shuru" + i).asComboBox.onChanged.Add(()=>{
                        if (contentPane.GetChild("shuru5").asComboBox.selectedIndex!=0)
                        {
                            contentPane.GetChild("shuru5").asComboBox.titleColor = Color.white;
                        }
                    });
                    continue;
                }
                if (contentPane.GetChild("shuru" + i).text == "")
                {
                    contentPane.GetChild("shuru" + i).asTextInput.promptText = "[color=#FF0000]此输入框不能为空[/color]";
                    isNull = true;
                }
            }

            if (isNull)
            {
                return;
            }

            if (clientAliyunIot.ConAndDisconBtnDown())
            {
                button.title = "断开";
                contentPane.GetChild("Tips").text = "连接状态：已连接";
                contentPane.GetChild("Tips").asTextField.color = Color.green;
            }
            else
            {
                contentPane.GetChild("Tips").text = "连接状态：连接失败";
                contentPane.GetChild("Tips").asTextField.color = Color.red;
            }
        }
        else
        {
            if (!clientAliyunIot.ConAndDisconBtnDown())
            {
                button.title = "连接";
                contentPane.GetChild("Tips").text = "连接状态：未连接";
                contentPane.GetChild("Tips").asTextField.color = Color.yellow;
            }
        }

    }
}
