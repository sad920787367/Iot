using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScenesTouch : MonoBehaviour
{
    private string TouchObjectName;
    void Start()
    {
        
    }
    void OnMouseDown()
    {
        Debug.Log("点击有效");
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit))
            {
                hit.collider.gameObject.SetActive(false);
                TouchObjectName = hit.collider.gameObject.name;
            }

            if (TouchObjectName == "BookTouch")
            {
                Debug.Log("书点击");
                Move.Instance.BookTypeMove();
            }
            if(TouchObjectName == "DeskTouch")
            {
                Debug.Log("桌子点击");
                Move.Instance.DeskTypeMove();
            }
        }
    }
}
