using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class HelpCtrl
{
    private GComponent bookPage;

    private GComponent mainHelp;//显示帮助图片的组件
    private GComponent helpCtrlPanel;//帮助界面的控制区域
    private GLoader loader;//图片装载器

    private GGraph touchAreaLeft;//上一页区域
    private GGraph touchAreaMiddle;//菜单区域
    private GGraph touchAreaRight;//下一页区域

    private int itemIndex = -1;//当前帮助项的序号 1为帮助1——登录物联网平台 2为帮助2 。。。。。。
    private int pageIndex = -1;//帮助图的序号
    private int pageCount = -1;//帮助页面总数

    private int PageIndex//帮助图的序号的属性方法
    {
        get { return pageIndex; }
        set
        {
            if (value != pageIndex)
            {
                pageIndex = value;
                mainHelp.GetChild("n12").asTextField.text = String.Format("{0}/{1}", PageIndex, this.pageCount);
            }
            pageIndex = value;
        }
    }

    public HelpCtrl(GComponent bookPage)
    {
        this.bookPage = bookPage;
    }

    public void UIInit()
    {
        mainHelp = bookPage.GetChild("n35").asCom;
        helpCtrlPanel = mainHelp.GetChild("n6").asCom;
        loader = mainHelp.GetChild("n0").asLoader;
        touchAreaLeft = mainHelp.GetChild("n1").asGraph;
        touchAreaMiddle = mainHelp.GetChild("n2").asGraph;
        touchAreaRight = mainHelp.GetChild("n3").asGraph;

        bookPage.GetChild("n29").onClick.Add(() => bookPage.GetController("c1").selectedIndex = 1);//帮助按钮
        bookPage.GetChild("n31").onClick.Add(() => bookPage.GetController("c1").selectedIndex = 0);//帮助界面返回按钮
        bookPage.GetChild("n32").onClick.Add(() =>
        {
            bookPage.GetController("c2").selectedIndex = 1;
            loader.icon = UIPackage.GetItemURL("Main", "hp1-1");
            itemIndex = 1;
            pageCount = 5;
            PageIndex = 1;
        });//第一个帮助按钮
        bookPage.GetChild("n36").onClick.Add(() =>
        {
            bookPage.GetController("c2").selectedIndex = 1;
            loader.icon = UIPackage.GetItemURL("Main", "hp2-1");
            itemIndex = 2;
            pageCount = 6;
            PageIndex = 1;
        });//第二个帮助按钮
        bookPage.GetChild("n48").onClick.Add(() =>
        {
            bookPage.GetController("c2").selectedIndex = 1;
            loader.icon = UIPackage.GetItemURL("Main", "hp3-1");
            itemIndex = 3;
            pageCount = 2;
            PageIndex = 1;
        });//第三个帮助按钮
        bookPage.GetChild("n51").onClick.Add(() =>
        {
            bookPage.GetController("c2").selectedIndex = 1;
            loader.icon = UIPackage.GetItemURL("Main", "hp4-1");
            itemIndex = 4;
            pageCount = 11;
            PageIndex = 1;
        });//第四个帮助按钮
        bookPage.GetChild("n54").onClick.Add(() =>
        {
            bookPage.GetController("c2").selectedIndex = 1;
            loader.icon = UIPackage.GetItemURL("Main", "hp5-1");
            itemIndex = 5;
            pageCount = 8;
            PageIndex = 1;
        });//第五个帮助按钮

        mainHelp.GetChild("n4").onClick.Add(() => mainHelp.GetController("c1").selectedIndex = 1);//点击后提示消失

        helpCtrlPanel.GetChild("n4").onClick.Add(() =>
        {
            bookPage.GetController("c2").selectedIndex = 0;
            mainHelp.GetController("c1").selectedIndex = 0;
            mainHelp.GetController("c2").selectedIndex = 0;

            itemIndex = -1;
            PageIndex = -1;
            pageCount = -1;
        });//帮助页面控制窗中的退出按钮
        helpCtrlPanel.GetChild("n1").onClick.Add(() => { PreviousPage(itemIndex, pageCount); });//帮助页面控制窗中的左按钮
        helpCtrlPanel.GetChild("n3").onClick.Add(() => { NextPage(itemIndex, pageCount); });//帮助页面控制窗中的右按钮

        touchAreaMiddle.onClick.Add(() =>
        {
            if (mainHelp.GetController("c2").selectedIndex == 0)
            {
                mainHelp.GetController("c2").selectedIndex = 1;
            }
            else
            {
                mainHelp.GetController("c2").selectedIndex = 0;
            }
        });//菜单区域点击
        touchAreaLeft.onClick.Add(() => { PreviousPage(itemIndex,pageCount); });//上一页区域点击
        touchAreaRight.onClick.Add(() => { NextPage(itemIndex, pageCount); }); //下一页区域点击
    }

    private void NextPage(int itemIndex, int pageCount)
    {
        if (pageIndex >= pageCount)
        {
            return;
        }
        PageIndex = pageIndex + 1;
        string resName = String.Format("hp{0}-{1}",itemIndex,pageIndex);
        loader.icon = UIPackage.GetItemURL("Main", resName);
    }

    private void PreviousPage(int itemIndex, int pageCount)
    {
        if (pageIndex <= 1)
        {
            return;
        }
        PageIndex = pageIndex - 1;
        string resName = String.Format("hp{0}-{1}", itemIndex, pageIndex);
        loader.icon = UIPackage.GetItemURL("Main", resName);
    }

}