using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Assets.Scripts;
using DG.Tweening;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class ClientAliyunIot : MonoBehaviour
{


    //public string ProductKey = "a1kR2bPfA4g";
    //public string DeviceName = "Light";
    //public string DeviceSecret = "lh2Zp8dmSwcOmt2pDP8pgdy6bC8suwJp";
    //public string RegionId = "cn-shanghai";
    //public string Identifier = "LightStatus";

    public string ProductKey { get; set; }
    public string DeviceName { get; set; }
    public string DeviceSecret { get; set; }
    public string RegionId { get; set; }
    public string Identifier { get; set; }
    public string HumiIdentifier { get; set; }

    public bool conStatus = false;
    public GameObject MainCObj;
    public GameObject airText;
    public GameObject air;
    public char statusStr;

    private MqttClient client;
    private float wenDuData;
    private float wenDuDataOld;
    

    //单例模式
    private static ClientAliyunIot instance;

    public static ClientAliyunIot Instance
    {
        get { return instance; }
        set { instance = value; }
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        /// <summary>
        /// 用来通信的Topic
        /// </summary>
        //static string PubTopic = "/" + ProductKey + "/" + DeviceName + "/user/update";
        //static string SubTopic = "/" + ProductKey + "/" + DeviceName + "/user/get";
        //string PubTopic = "/sys/" + ProductKey + "/" + DeviceName + "/thing/event/property/post";//属性上报 发送
        //string SubTopic = "/sys/" + ProductKey + "/" + DeviceName + "/thing/service/property/set";//属性设置 订阅
        //ProductKey = "a1kR2bPfA4g";
        //DeviceName = "Light";
        //DeviceSecret = "lh2Zp8dmSwcOmt2pDP8pgdy6bC8suwJp";
        //RegionId = "cn-shanghai";
        //Identifier = "LightStatus";
        wenDuDataOld = (float)Math.Round(Convert.ToSingle(airText.GetComponent<TextMesh>().text.Substring(0, 2)),1);
        statusStr = '0';
    }

    //应用被结束时断开连接
    void OnApplicationQuit()
    {
        if (conStatus)
        {
            ConAndDisconBtnDown();
        }

        Application.Quit();
    }

    void Update()
    {
        if (conStatus)
        {
            if (MainCObj.transform.parent.name == "jidianqi" || MainCObj.transform.parent.name == "RedLED")
            {
                if (statusStr.Equals('0') && MainCObj.activeSelf)
                {
                    MainCObj.SetActive(false);
                    SendLightStatus();
                }

                if (statusStr.Equals('1') && !MainCObj.activeSelf)
                {
                    MainCObj.SetActive(true);
                    SendLightStatus();
                }
            }
        }
    }

    //连接
    public bool ConAndDisconBtnDown()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        string clientId = host.AddressList.FirstOrDefault(
            ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
        string t = Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());
        string signmethod = "hmacmd5";

        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("productKey", ProductKey);
        dict.Add("deviceName", DeviceName);
        dict.Add("clientId", clientId);
        dict.Add("timestamp", t);

        string mqttUserName = DeviceName + "&" + ProductKey;
        string mqttPassword = IotSignUtils.sign(dict, DeviceSecret, signmethod);
        string mqttClientId = clientId + "|securemode=3,signmethod=" + signmethod + ",timestamp=" + t + "|";
        string targetServer = ProductKey + ".iot-as-mqtt." + RegionId + ".aliyuncs.com";
        MqttClient newClient = new MqttClient(targetServer);
        if (!conStatus)//让client对象在断开后才更新
        {
            client = newClient;
        }
        client.ProtocolVersion = MqttProtocolVersion.Version_3_1_1;

        if (!conStatus)
        {
            
            client.Connect(mqttClientId, mqttUserName, mqttPassword, false, 60);
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            if (client.IsConnected)
            {
                conStatus = true;
                if (MainCObj.transform.parent.name == "jidianqi" || MainCObj.transform.parent.name == "RedLED")
                {
                    SendLightStatus();
                }
                Debug.Log("连接至阿里云Iot");
                return true;
            }
            else
            {
                Debug.Log("连接失败");
                return false;
            }
        }
        else
        {
            client.MqttMsgPublishReceived -= Client_MqttMsgPublishReceived;
            client.Disconnect();
            conStatus = false;
            Debug.Log("断开连接");
            if (MainCObj.transform.parent.name == "DHT11")
            {
                WenDuSend();
            }
            return false;
        }
    }

    public void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string message = Encoding.ASCII.GetString(e.Message);
        ChangeLightStatus(message);
        Debug.Log(message);
    }
    //分析接收数据，改变设备状态
    private void ChangeLightStatus(string message)
    {
        int dataIndex = message.IndexOf("\"" + Identifier + "\":") + ("\"" + Identifier + "\":").Length;
        statusStr = message[dataIndex];
        Debug.Log(statusStr);
        Debug.Log(statusStr.Equals('0'));
    }


    //灯光状态上报
    public void SendLightStatus()
    {
        if (conStatus == false)
        {
            return;
        }
        string PubTopic = "/sys/" + ProductKey + "/" + DeviceName + "/thing/event/property/post";//属性上报 发送Topic
        client.Publish(PubTopic, Encoding.ASCII.GetBytes(ToSendStr()));
        Debug.Log("发送");

    }

    private string ToSendStr()
    {
        string id = DateTime.Now.ToString("yyyyMMddHHmmssffff");
        //string id = MainCObj.transform.name;
        string data = Todata();
        string sendStr = "{\"id\":\"" + id + "\",\"version\":\"1.0.0\",\"params\":{\"" + Identifier + "\":" + data +
                         "}}";
        return sendStr;
    }

    //温湿度上报
    public void WenDuSend()
    {
        if (conStatus == false)
        {
            if (IsInvoking("WenDuTuoZhan"))
            {
                CancelInvoke("WenDuTuoZhan");
            }
            return;
        }
        if (DOTween.IsTweening("WenDuChange"))
        {
            DOTween.Kill("WenDuChange");
        }
        wenDuData = (float)Math.Round(Convert.ToSingle(airText.GetComponent<TextMesh>().text.Substring(0, 2)), 1);
        if (wenDuDataOld != wenDuData)
        {
            DOTween.To(() => wenDuDataOld, x => wenDuDataOld = x, wenDuData, 120).SetEase(Ease.InCirc).SetId("WenDuChange");
        }
        if (IsInvoking("WenDuTuoZhan"))
        {
            CancelInvoke("WenDuTuoZhan");
        }
        InvokeRepeating("WenDuTuoZhan",20,20);


    }

    //取消各种方法委托循环
    public void MyCancelAllInvoke()
    {
        if (IsInvoking("WenDuTuoZhan"))
        {
            CancelInvoke("WenDuTuoZhan");
        }
    }

    private void WenDuTuoZhan()
    {
        if (wenDuDataOld == wenDuData)
        {
            return;
        }
        string PubTopic = "/sys/" + ProductKey + "/" + DeviceName + "/thing/event/property/post";//属性上报 发送Topic
        string id = MainCObj.transform.name;
        float data = (float)Math.Round(wenDuDataOld,1);
        int shiDu = 90;
        shiDu -= (int)(data * 1.67);
        string sendStr = "{\"id\":\"" + id + "\",\"version\":\"1.0.0\",\"params\":{\"" + Identifier + "\":" + data +
                         "}}";
        string shiDuStr = "{\"id\":\"" + id + "\",\"version\":\"1.0.0\",\"params\":{\"" + HumiIdentifier + "\":" + shiDu +
                          "}}";
        client.Publish(PubTopic, Encoding.ASCII.GetBytes(sendStr));
        client.Publish(PubTopic, Encoding.ASCII.GetBytes(shiDuStr));
        Debug.Log("温湿度上报成功");
        Debug.Log(sendStr);
        Debug.Log(shiDuStr);
    }

    private string Todata()
    {
        if (MainCObj.activeSelf)
        {
            return "1";
        }
        else
        {
            return "0";
        }
    }

}
