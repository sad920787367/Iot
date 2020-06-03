using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightStatus : MonoBehaviour
{
    private static LightStatus instance;

    public bool lightStatus = true;
    private bool oldlightStatus = true;
    public GameObject RedLED;

    public static LightStatus Instance
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

    void Start()
    {
        if (RedLED.activeSelf)
        {
            lightStatus = true;
        }
        else
        {
            lightStatus = false;
        }
    }

    void Update()
    {
        if (oldlightStatus != lightStatus)
        {
            statusChange();
            oldlightStatus = lightStatus;
        }
    }

    private void statusChange()
    {
        if (lightStatus)
        {
            RedLED.SetActive(true);
            ClientAliyunIot.Instance.SendLightStatus();
        }
        else
        {
            RedLED.SetActive(false);
            ClientAliyunIot.Instance.SendLightStatus();
        }
    }



}
