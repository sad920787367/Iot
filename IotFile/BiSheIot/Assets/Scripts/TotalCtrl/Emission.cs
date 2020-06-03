using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Emission : MonoBehaviour
{
    private static Emission instance;

    public Tweener flashTween;

    private Material material;
    private bool isPlay = false;

    public static Emission Instance
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
        instance = this;
    }

    void Update()
    {
        //if (Input.GetKey(KeyCode.Space))
        //{
        //    KillTween();
        //}
        if (StatusCtrl.Instance.CameraIndex != 0 && isPlay)
        {
            KillTween();
        }

        if (StatusCtrl.Instance.CameraIndex == 0 && !isPlay && StatusCtrl.Instance.UIStatusIndex == 1)
        {
            TipsFlash();
        }
    }

    void Start()
    {
        material = GetComponent<MeshRenderer>().materials[0];
    }


    public void TipsFlash()
    {
        flashTween = material.DOColor(new Color(0.197f, 0.251f, 0.515f), "_EmissionColor", 2).SetLoops(-1, LoopType.Yoyo);
        isPlay = true;
        //Debug.Log("开始闪烁");
    }

    public void KillTween()
    {
        flashTween.Kill();
        material.DOColor(Color.black, "_EmissionColor", 0.1f);
        isPlay = false;
        //Debug.Log("销毁动画");
    }
}
