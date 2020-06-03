using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BookCtrl : MonoBehaviour
{
    private static BookCtrl instance;

    public static BookCtrl Instance
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

    public void OpenBook()
    {
        transform.DOLocalRotate(new Vector3(185, 0, 0), 1,RotateMode.LocalAxisAdd).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            StatusCtrl.Instance.UIStatusIndex = 2;
        });
    }

    public void CloseBook()
    {
        transform.DOLocalRotate(new Vector3(-185, 0, 0), 1,RotateMode.LocalAxisAdd).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            Move.Instance.ToBasePoint();
        });
    }
}
