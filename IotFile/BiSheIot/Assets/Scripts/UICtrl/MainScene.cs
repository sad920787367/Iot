using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using FairyGUI;
//using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MainScene : MonoBehaviour
{
    public GComponent mainUI;
    public GComponent selectCom;
    public GComponent bookCom;
    public GComponent deskCom;
    private GComponent YuLanCom;
    public GGroup fengmianUI;

    private StageWindow stageWindow;
    private PeiZhiWindow configWindow;

    private Transition awakeOpen;
    private Transition anyKeyDown;

    private bool isBookUI = false;
    private bool isDeskUI = false;

    //private GameObject ctrlObj;
    public Camera mainCamera;
    public Camera yiqiCamera;

    public GameObject[] YiQiTouchObjs;//所有仪器的点击监测物体
    public GameObject[] yuLanYiQiObjs;//0是ESP32,1是扩展板，2是LED，3是继电器，4是温湿度监测
    private GameObject yulanRotateObj;//被预览的物体

    public GameObject[] ropeGroup;//0是USB线，1是黑红绿线
    public GameObject[] changeGroup;//0是LED，1是继电器，2是温湿度监测
    public GameObject[] otherGroup;//0是ESP32，1是扩展板，2是连接板部件
    public GameObject[] posGroup;//预览时位置，0是LED，1是继电器，2是温湿度监测，3是ESP32，4是扩展板
    public GameObject[] pos1Group;//非预览时的位置,0是LED，1是继电器，2是温湿度监测，3是ESP32，4是扩展板

    private List<string> fangAn;


    void Start()
    {
        fangAn = new List<string>() { "选择方案" };
        LoadFangAn();
        mainUI = GetComponent<UIPanel>().ui;
        configWindow = new PeiZhiWindow(fangAn);
        selectCom = UIPackage.CreateObject("Main", "Select").asCom;
        bookCom = UIPackage.CreateObject("Main", "BookPage").asCom;
        deskCom = UIPackage.CreateObject("Main", "DeskPage").asCom;
        YuLanCom = deskCom.GetChild("n31").asCom;
        stageWindow = new StageWindow(deskCom,ropeGroup,changeGroup,otherGroup,posGroup,pos1Group);
        fengmianUI = mainUI.GetChild("n7").asGroup;
        awakeOpen = mainUI.GetTransition("t1");
        awakeOpen.Play();
        StatusCtrl.Instance.UIStatusIndex = 0;
        UIGetResolution();
        //AssetDatabase.SaveAssets();
        HelpCtrl helpCtrl = new HelpCtrl(bookCom);
        helpCtrl.UIInit();
    }

    void Update()
    {
        if (Input.anyKeyDown && StatusCtrl.Instance.UIStatusIndex == 0)
        {
            AnyKeyDown(selectCom);
        }
        else if (StatusCtrl.Instance.CameraIndex != 0 && selectCom.GetTransition("t0").playing)
        {
            selectCom.GetTransition("t0").Stop();
            DeletUI(selectCom);
        }
        else if (StatusCtrl.Instance.UIStatusIndex == 2 && !isBookUI)
        {
            isBookUI = true;
            mainCamera.fieldOfView = 60;
            ShowUI(bookCom);
            BookUICtrl();
        }
        else if (StatusCtrl.Instance.CameraIndex == 0 && !selectCom.GetTransition("t0").playing && StatusCtrl.Instance.UIStatusIndex == 1)
        {
            isBookUI = false;
            isDeskUI = false;
            ShowUI(selectCom);
            DeletUI(bookCom);
            DeletUI(deskCom);
            selectCom.GetTransition("t0").Play();
        }
        else if (StatusCtrl.Instance.UIStatusIndex == 3 && StatusCtrl.Instance.CameraIndex == 2 && !isDeskUI)
        {
            isDeskUI = true;
            ShowUI(deskCom);
            DeskUICtrl();
        }
        else if(deskCom.GetController("c1").selectedIndex == 1 && !WindowIsShow())
        {
            deskCom.GetController("c1").selectedIndex = 0;
        }

        if (ClientAliyunIot.Instance.MainCObj != null)
        {
            if (ClientAliyunIot.Instance.MainCObj.transform.parent.name != "DHT11")//在不是温湿度场景时动态改变按钮文字
            {
                GButton button = deskCom.GetChild("n8").asButton;
                if (ClientAliyunIot.Instance.statusStr.Equals('0') && button.title != "开灯")
                {
                    button.title = "开灯";
                }
                else if (ClientAliyunIot.Instance.statusStr.Equals('1') && button.title != "关灯")
                {
                    button.title = "关灯";
                }
            }
        }


        if (StatusCtrl.Instance.CameraIndex == 2 && StatusCtrl.Instance.UIStatusIndex == 3 && isDeskUI)
        {
            if (!configWindow.isShowing && !stageWindow.isShowing)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    string touchName;
                    if (Physics.Raycast(ray, out hit))
                    {
                        touchName = hit.transform.name;
                        if (touchName == "TouchObj")
                        {
                            if (deskCom.GetController("c2").selectedIndex == 1)
                            {
                                return;
                            }
                            Debug.Log(hit.transform.parent.name);
                            YuLanShow(hit.transform.parent.name);

                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            selectCom.SetSize(GRoot.inst.width, GRoot.inst.height);
        }

    }
    public void ArrowHide()
    {
        selectCom.GetTransition("t0").Stop();
    }

    private void AnyKeyDown(GComponent targetCom)
    {
        awakeOpen.Stop();
        anyKeyDown = mainUI.GetTransition("t2");
        anyKeyDown.Play(() =>
        {
            fengmianUI.visible = false;
            ShowUI(targetCom);
            targetCom.GetTransition("t0").Play();
            Move.Instance.bookTouchM.SetActive(true);
            Move.Instance.deskTouchM.SetActive(true);
            StatusCtrl.Instance.UIStatusIndex = 1;
        });
    }
    private void BookUICtrl()
    {
        bookCom.GetChild("n19").onClick.Clear();
        bookCom.GetChild("n18").onClick.Clear();

        GComboBox combo = bookCom.GetChild("n25").asComboBox;
        bookCom.GetTransition("t0").Play();
        bookCom.GetChild("n19").onClick.Add(() =>
        {
            bookCom.GetChild("n19").onClick.Clear();
            bookCom.GetChild("n18").onClick.Clear();
            bookCom.GetTransition("t1").Play(() =>
            {
                BookCtrl.Instance.CloseBook();
                if (combo.selectedIndex == 0)
                {
                    mainCamera.fieldOfView = 60;
                }
                else if (combo.selectedIndex == 1)
                {
                    mainCamera.fieldOfView = 62.5f;
                }
                else
                {
                    mainCamera.fieldOfView = 71;
                }
            });

        });//设置页退出按钮
        bookCom.GetChild("n18").onClick.Add(() =>
        {
            string res = combo.value;
            int middle = res.IndexOf("x");
            //Debug.Log(middle);
            int resX = Convert.ToInt32(res.Substring(0, middle));
            int resY = Convert.ToInt32(res.Substring(middle+1));
            //Debug.Log(resX+" "+resY+" "+res);
            Screen.SetResolution(resX,resY,false);
            
            //Debug.Log(bookCom.size);
            //mainCamera.aspect = resX / resY;
            //yiqiCamera.aspect = resX / resY;
            if (combo.selectedIndex == 2)
            {
                bookCom.SetSize(resX, resY);
            }
            else
            {
                float myS = 0;
                DOTween.To(() => myS, x => myS = x, 0.5f, 0.5f).OnComplete(() =>
                {
                    bookCom.SetSize(GRoot.inst.width, GRoot.inst.height);
                    Debug.Log("切换成功");
                });
            }
        });//确认按钮

        bookCom.GetChild("n26").onClick.Add(() =>
        {
            string path = "./Iot_Data/StreamingAssets/2 Starter Kit for Aliyun IoT.pdf";
            path = path.Replace("/", "\\");
            Process.Start(path);
        });//打开实验资料
    }

    private void DeskUICtrl()
    {
        Controller c1 = deskCom.GetController("c1");
        deskCom.GetChild("n0").onClick.Add(() =>
        {
            DeletUI(deskCom);
            Move.Instance.ToBasePoint();
        });//返回按钮事件监听

        deskCom.GetChild("n3").onClick.Add(() =>
        {
            c1.selectedIndex = 1;
            stageWindow.Show();
        });//选择实验按钮事件监听

        deskCom.GetChild("n6").onClick.Add(() =>
        {
            c1.selectedIndex = 1;
            configWindow.Show();
        });//配置信息按钮事件
    }

    private void UIGetResolution()
    {
        GComboBox combo = bookCom.GetChild("n25").asComboBox;
        if (Screen.resolutions.ToString().Equals(new string[] { "1920", "1080" }))
        {
            combo.title = "1920x1080";
        }
        else if (Screen.resolutions.ToString().Equals(new string[] { "1600", "900" }))
        {
            combo.title = "1600x900";
        }
        else if (Screen.resolutions.ToString().Equals(new string[] { "800", "600" }))
        {
            combo.title = "800x600";
        }
        else
        {
            combo.title = "1600x900";
            Screen.SetResolution(1600, 900, false);
        }
    }

    private bool WindowIsShow()
    {
        if (stageWindow.isShowing || configWindow.isShowing)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void LoadFangAn()
    {
        int comboCount = PlayerPrefs.GetInt("SaveComboCount", -1);
        if (comboCount == -1)
        {
            return;
        }

        for (int i = 1; i < comboCount; i++)
        {
            fangAn.Add(PlayerPrefs.GetString("Save"+i,"数据错误"));
        }
    }

    //器材预览相关
    private void YuLanShow(string yiqiName)
    {
        if (WindowIsShow() || deskCom.GetController("c1").selectedIndex == 2)
        {
            return;
        }
        deskCom.GetController("c2").selectedIndex = 1;
        foreach (GameObject obj in YiQiTouchObjs)
        {
            obj.SetActive(false);
        }

        GGraph graph = YuLanCom.GetChild("n1").asGraph;
        RenderTexture renderTexture = Resources.Load<RenderTexture>("FGUI2/YiQiView");
        Material material = Resources.Load<Material>("FGUI2/YiQiViewMat");
        Image img = new Image();
        img.texture = new NTexture(renderTexture);
        img.material = material;
        img.size = graph.size;
        graph.SetNativeObject(img);
        int count = 0;
        for (int i = 0; i < yuLanYiQiObjs.Length; i++)
        {
            yuLanYiQiObjs[i].SetActive(false);
            if (yuLanYiQiObjs[i].transform.name == yiqiName)
            {
                count = i;
            }
        }
        Debug.Log(count);
        yuLanYiQiObjs[count].SetActive(true);
        yulanRotateObj = yuLanYiQiObjs[count];
        YuLanCom.onGearStop.Add(() =>
        {
            if (deskCom.GetController("c2").selectedIndex == 1)
            {
                YuLanCom.GetChild("n2").onTouchBegin.Add(OnTouchBegin);//想让预览旋转时的鼠标点击
                YuLanCom.GetChild("n2").onTouchMove.Add(OnTouchMove);//想让预览旋转时的鼠标按住左键移动
                YuLanCom.GetChild("n0").onClick.Add(() =>
                {
                    deskCom.GetController("c2").selectedIndex = 0;
                    graph = null;
                    yulanRotateObj.transform.localEulerAngles = new Vector3(0, 0, -90);
                    YuLanCom.GetChild("n2").onTouchBegin.Clear();
                    YuLanCom.GetChild("n2").onTouchMove.Clear();
                    YuLanCom.GetChild("n0").onClick.Clear();
                });//预览窗口关闭
            }
            else
            {
                foreach (GameObject obj in YiQiTouchObjs)
                {
                    obj.SetActive(true);
                }
                YuLanCom.onGearStop.Clear();
            }
        });//此处为防误触,防止一点开就关掉
    }

    private void OnTouchBegin(EventContext context)
    {
        context.CaptureTouch();
    }
    private void OnTouchMove(EventContext context)
    {
        //Debug.Log("开始旋转");
        if (yulanRotateObj!=null)
        {
            Vector3 offect = new Vector3(0, -Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"));
            yulanRotateObj.transform.localEulerAngles += offect * 10;
        }

    }


    private void ShowUI(GComponent targetCom)
    {
        targetCom.SetSize(GRoot.inst.width, GRoot.inst.height);
        GRoot.inst.AddChild(targetCom);
    }

    private void DeletUI(GComponent targetCom)
    {
        GRoot.inst.RemoveChild(targetCom);
    }
}
