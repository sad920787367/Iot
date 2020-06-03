using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using FairyGUI;
using UnityEngine;

public class StageWindow : Window
{
    private GList list;
    private GComponent deskCom;

    private GameObject ctrlObj;
    private GameObject airText;

    private AudioSource audioSource;
    private AudioClip audio1;
    private AudioClip audio2;
    private AudioClip audio3;

    private GameObject[] ropeGroup;//0是USB线，1是黑红绿线
    private GameObject[] changeGroup;//0是LED，1是继电器，2是温湿度监测
    private GameObject[] otherGroup;//0是ESP32，1是扩展板，2是连接板部件
    private GameObject[] posGroup;//预览时位置，0是LED，1是继电器，2是温湿度监测，3是ESP32，4是扩展板
    private GameObject[] pos1Group;//非预览时的位置,0是LED，1是继电器，2是温湿度监测，3是ESP32，4是扩展板

    public StageWindow(GComponent deskCom,GameObject[] ropeGroup,GameObject[] changeGroup,GameObject[] otherGroup, GameObject[] posGroup, GameObject[] pos1Group)
    {
        this.deskCom = deskCom;
        this.ropeGroup = ropeGroup;
        this.changeGroup = changeGroup;
        this.otherGroup = otherGroup;
        this.posGroup = posGroup;
        this.pos1Group = pos1Group;
        airText = ClientAliyunIot.Instance.airText;
    }

    protected override void OnInit()
    {
        this.contentPane = UIPackage.CreateObject("Main", "SelectWIndows").asCom;
        list = contentPane.GetChild("n1").asList;
        contentPane.GetChild("n3").text = "";
        contentPane.SetSize(800,600);
        contentPane.SetXY(80,10);
        selectWindowLoad(list);
        contentPane.GetChild("n5").onClick.Add(() =>
        {
            if (list.selectedIndex == -1)
            {
                return;
            }

            if (ClientAliyunIot.Instance.conStatus)
            {
                contentPane.GetChild("n3").text = "目前处于连接状态，\n请断开后再切换实验场景";
                return;
            }

            GRoot.inst.CloseAllWindows();
            DOTween.To(() => deskCom.GetChild("n7").asGraph.alpha, x => deskCom.GetChild("n7").asGraph.alpha = x, 1,
                    1.5f)
                .SetEase(Ease.Linear).OnComplete(ChangeObject);
        });

        GGraph holder = deskCom.GetChild("n10").asGraph;
        RenderTexture renderTexture = Resources.Load<RenderTexture>("FGUI2/AirView");
        Material material = Resources.Load<Material>("FGUI2/AirViewMat");
        Image img = new Image();
        img.texture = new NTexture(renderTexture);
        img.material = material;
        img.size = holder.size;
        holder.SetNativeObject(img);

        audioSource = ClientAliyunIot.Instance.air.GetComponent<AudioSource>();
        audio1 = Resources.Load<AudioClip>("FGUI2/XunHuanAirSound");
        audio2 = Resources.Load<AudioClip>("FGUI2/Warning");
    }

    private void selectWindowLoad(GList list)
    {
        list.SetVirtual();
        list.itemRenderer = RenderListItem;
        list.numItems = 4;
    }

    private void RenderListItem(int index, GObject obj)
    {
        GButton button = obj.asButton;
        button.icon = UIPackage.GetItemURL("Main", "yq-0" + (index + 1));
        switch (index)
        {
            case 0:
                button.title = "智能灯光";
                break;
            case 1:
                button.title = "智能插座";
                break;
            case 2:
                button.title = "温湿度监测";
                break;
            case 3:
                button.title = "预览场景";
                break;
        }
        //button.onChanged.Add(() => { Debug.Log("切换");});
        button.onClick.Add(() =>
        {
            switch (index)
            {
                case 0:
                    contentPane.GetChild("n3").text =
                        "场景内容:\n红色食人鱼 LED 发光模块 x1\nFireBettle Board-ESP32 x1\nFireBettle Gravity 扩展板 x1";
                    break;
                case 1:
                    contentPane.GetChild("n3").text =
                        "场景内容:\n继电器模块 x1\nFireBettle Board-ESP32 x1\nFireBettle Gravity 扩展板 x1";
                    break;
                case 2:
                    contentPane.GetChild("n3").text =
                        "场景内容:\nDHT11 温湿度传感器 x1\nFireBettle Board-ESP32 x1\nFireBettle Gravity 扩展板 x1";
                    break;
                case 3:
                    contentPane.GetChild("n3").text =
                        "场景内容:\n所有部件展示";
                    contentPane.GetChild("n3").asTextField.textFormat.size = 27;
                    break;
            }
        });
    }

    private void ChangeObject()
    {
        if (changeGroup[2].activeSelf && deskCom.GetChild("n18").asButton.title == "关闭")
        {
            AudioChange("开启");
            airText.SetActive(false);
            deskCom.GetChild("n18").asButton.title = "开启";
        }

        MyDelay(1,()=>{
            DOTween.To(() => deskCom.GetChild("n7").asGraph.alpha, x => deskCom.GetChild("n7").asGraph.alpha = x, 0,
                2f).SetEase(Ease.Linear);
        });
        YiQiMove();

        foreach (GameObject obj in ropeGroup)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in changeGroup)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in otherGroup)
        {
            obj.SetActive(false);
        }

        GButton turnButton = deskCom.GetChild("n8").asButton;

        deskCom.GetChild("n6").visible = false;
        deskCom.GetChild("n8").visible = false;

        switch (list.selectedIndex)
        {
            case 0:
                ropeGroup[0].SetActive(true);
                ropeGroup[1].SetActive(true);
                changeGroup[0].SetActive(true);
                changeGroup[0].transform.GetChild(0).gameObject.SetActive(true);
                otherGroup[0].SetActive(true);
                otherGroup[1].SetActive(true);
                otherGroup[2].SetActive(true);
                ctrlObj = changeGroup[0].transform.GetChild(1).gameObject;
                turnButton.title = "开灯";
                turnButton.onClick.Clear();
                turnButton.onClick.Add(() =>
                {
                    JiDianQiAndDeng(turnButton);
                });
                break;
            case 1:
                ropeGroup[0].SetActive(true);
                ropeGroup[1].SetActive(true);
                changeGroup[1].SetActive(true);
                changeGroup[1].transform.GetChild(0).gameObject.SetActive(true);
                otherGroup[0].SetActive(true);
                otherGroup[1].SetActive(true);
                otherGroup[2].SetActive(true);
                ctrlObj = changeGroup[1].transform.GetChild(1).gameObject;
                turnButton.title = "开灯";
                turnButton.onClick.Clear();
                turnButton.onClick.Add(() =>
                {
                    JiDianQiAndDeng(turnButton);
                });
                break;
            case 2:
                ropeGroup[0].SetActive(true);
                ropeGroup[1].SetActive(true);
                changeGroup[2].SetActive(true);
                changeGroup[2].transform.GetChild(0).gameObject.SetActive(true);
                otherGroup[0].SetActive(true);
                otherGroup[1].SetActive(true);
                otherGroup[2].SetActive(true);
                ctrlObj = changeGroup[2].transform.GetChild(1).gameObject;
                turnButton.title = "调整室温";
                turnButton.onClick.Clear();
                turnButton.onClick.Add(() =>
                {
                    AirPanel();
                });
                break;
            default:
                changeGroup[0].SetActive(true);
                changeGroup[1].SetActive(true);
                changeGroup[2].SetActive(true);
                otherGroup[0].SetActive(true);
                otherGroup[1].SetActive(true);
                changeGroup[0].transform.GetChild(0).gameObject.SetActive(false);
                changeGroup[1].transform.GetChild(0).gameObject.SetActive(false);
                changeGroup[2].transform.GetChild(0).gameObject.SetActive(false);
                if (ctrlObj.activeSelf && ctrlObj.transform.parent.transform.name != "DHT11")
                {
                    ctrlObj.SetActive(false);
                }
                break;
        }

        if (list.selectedIndex <= 2)
        {
            deskCom.GetChild("n8").visible = true;
            deskCom.GetChild("n6").visible = true;
        }

        if (ctrlObj != null && list.selectedIndex <=2)
        {
            if (list.selectedIndex <=1)
            {
                ctrlObj.SetActive(false);
            }
            ClientAliyunIot.Instance.MainCObj = ctrlObj;
        }
    }

    private void YiQiMove()
    {
        if (list.selectedIndex == list.numItems-1)
        {
            for (int i = 0; i < pos1Group.Length; i++)
            {
                if (i < 3)
                {
                    changeGroup[i].transform.position = posGroup[i].transform.position;
                    changeGroup[i].transform.rotation = posGroup[i].transform.rotation;
                }

                if (i >= 3 && i < 5)
                {
                    otherGroup[i - 3].transform.position = posGroup[i].transform.position;
                    otherGroup[i - 3].transform.rotation = posGroup[i].transform.rotation;
                }
            }
        }
        else
        {
            for (int i = 0; i < pos1Group.Length; i++)
            {
                if (i<3)
                {
                    changeGroup[i].transform.position = pos1Group[i].transform.position;
                    changeGroup[i].transform.rotation = pos1Group[i].transform.rotation;
                }

                if (i>=3 && i<5)
                {
                    otherGroup[i - 3].transform.position = pos1Group[i].transform.position;
                    otherGroup[i - 3].transform.rotation = pos1Group[i].transform.rotation;
                }
            }
        }
    }

    private void JiDianQiAndDeng(GButton button)
    {
        ClientAliyunIot clientAliyunIot = ClientAliyunIot.Instance;
        if (ctrlObj.activeSelf)
        {
            ctrlObj.SetActive(false);
            clientAliyunIot.statusStr = '0';
            button.title = "开灯";
        }
        else
        {
            ctrlObj.SetActive(true);
            clientAliyunIot.statusStr = '1';
            button.title = "关灯";
        }

        if (clientAliyunIot.conStatus)
        {
            clientAliyunIot.SendLightStatus();
        }
    }

    private void AirPanel()
    {
        Controller c1 = deskCom.GetController("c1");
        c1.selectedIndex = 2;
        deskCom.GetChild("n19").asGraph.onClick.Add(()=>
        {
            c1.selectedIndex = 0;
            deskCom.GetChild("n19").asGraph.onClick.Clear();
            deskCom.GetChild("n18").onClick.Clear();
            deskCom.GetChild("n20").onClick.Clear();
            deskCom.GetChild("n21").onClick.Clear();
        });//退出按钮


        deskCom.GetChild("n18").onClick.Add(() =>
        {
            AirCtrl();
        });//开启按钮

        deskCom.GetChild("n20").onClick.Add(() =>
        {
            string wenDu = deskCom.GetChild("n13").text;
            int index = Convert.ToInt32(wenDu.Substring(0, 2));
            if (index <= 18)
            {
                return;
            }

            index -= 1;
            deskCom.GetChild("n13").text = index + "℃";
            airText.GetComponent<TextMesh>().text = deskCom.GetChild("n13").text;
            if (deskCom.GetChild("n18").asButton.title == "关闭" && ClientAliyunIot.Instance.conStatus)
            {
                ClientAliyunIot.Instance.WenDuSend();
            }
        });//左按钮
        deskCom.GetChild("n21").onClick.Add(() =>
        {
            string wenDu = deskCom.GetChild("n13").text;
            int index = Convert.ToInt32(wenDu.Substring(0, 2));
            if (index >= 36)
            {
                return;
            }

            index += 1;
            deskCom.GetChild("n13").text = index + "℃";
            airText.GetComponent<TextMesh>().text = deskCom.GetChild("n13").text;
            if (deskCom.GetChild("n18").asButton.title == "关闭" && ClientAliyunIot.Instance.conStatus)
            {
                ClientAliyunIot.Instance.WenDuSend();
            }
        });//右按钮

    }

    private void AirCtrl()
    {
        if (DOTween.IsTweening("audioUp") || DOTween.IsTweening("audioDown"))
        {
            return;
        }
        string wenDu = deskCom.GetChild("n13").text;
        //Tween tween1;
        //Tween tween2;
        if (deskCom.GetChild("n18").asButton.title == "开启")
        {
            airText.SetActive(true);
            airText.GetComponent<TextMesh>().text = wenDu;
            audioSource.clip = audio2;
            //Color color = airText.GetComponent<MeshRenderer>().material.color;
            //airText.GetComponent<MeshRenderer>().material.color = new Color(color.r,color.g,color.b,0);
            //tween1 = airText.GetComponent<MeshRenderer>().material.DOFade(255, 0.5f);
            audioSource.Play();
            deskCom.GetChild("n18").asButton.title = "关闭";
            if (ClientAliyunIot.Instance.conStatus)
            {
                ClientAliyunIot.Instance.WenDuSend();
            }
        }
        else
        {
            airText.SetActive(false);
            ClientAliyunIot.Instance.MyCancelAllInvoke();
            deskCom.GetChild("n18").asButton.title = "开启";
        }
        MyDelay(0.5f,()=>AudioChange(deskCom.GetChild("n18").asButton.title));
    }

    private void AudioChange(string str)
    {
        if (str == "关闭")
        {
            audioSource.clip = audio1;
            audioSource.loop = true;
            audioSource.Play();
            DOTween.To(() => audioSource.volume=0, x => audioSource.volume = x, 1, 8).SetEase(Ease.Linear).SetId("audioUp");
        }
        else
        {
            DOTween.To(() => audioSource.volume=1, x => audioSource.volume = x, 0, 3).SetEase(Ease.Linear).SetId("audioDown").OnComplete(()=>
            {
                audioSource.loop = false;
                audioSource.Stop();
            });
        }
    }

    private void MyDelay(float delay,TweenCallback fallBack)
    {
        float timeCount = 0;
        DOTween.To(() => timeCount, x => timeCount = x, delay, delay).SetEase(Ease.Linear).OnComplete(fallBack);
    }


}
