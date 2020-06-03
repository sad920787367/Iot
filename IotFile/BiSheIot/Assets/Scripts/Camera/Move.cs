using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Move : MonoBehaviour
{
    private static Move instance;

    private BookCtrl bookCtrl;

    /// <summary>
    /// Pos代表摄像机，XXXTouchM为点击检测物体
    /// </summary>
    public GameObject pos0;
    public GameObject pos1;
    public GameObject pos2;
    public GameObject bookTouchM;
    public GameObject deskTouchM;

    public static Move Instance
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

    void Start()
    {
        bookCtrl = BookCtrl.Instance;
        StatusCtrl.Instance.CameraIndex = 0;
        bookTouchM.SetActive(false);
        deskTouchM.SetActive(false);
    }


    public void BookTypeMove()
    {
        transform.DOMove(pos1.transform.position, 1.2f);
        transform.DORotate(pos1.transform.eulerAngles, 1.5f).OnComplete(() =>
        {
            bookCtrl.OpenBook();
            Debug.Log("移动完毕");
            bookTouchM.SetActive(false);
        });
        StatusCtrl.Instance.CameraIndex = 1;
    }

    public void DeskTypeMove()
    {
        transform.DOMove(pos2.transform.position, 1.2f);
        transform.DORotate(pos2.transform.eulerAngles, 1.5f).OnComplete(() =>
        {
            Debug.Log("移动完毕");
            deskTouchM.SetActive(false);
            StatusCtrl.Instance.UIStatusIndex = 3;
        });
        StatusCtrl.Instance.CameraIndex = 2;
    }

    public void ToBasePoint()
    {
        transform.DOMove(pos0.transform.position, 1.2f);
        transform.DORotate(pos0.transform.eulerAngles, 1.5f).OnComplete(() =>
        {
            StatusCtrl.Instance.UIStatusIndex = 1;
            StatusCtrl.Instance.CameraIndex = 0;
            bookTouchM.SetActive(true);
            deskTouchM.SetActive(true);
        });
    }
}
