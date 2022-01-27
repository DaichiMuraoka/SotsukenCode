using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleButton : TitleText
{
    [SerializeField] protected GameObject tab = null;

    private void Start()
    {
        tab.SetActive(false);
    }

    public void SetActiveOfTab(bool value)
    {
        tab.SetActive(value);
    }

    public void OnClick()
    {
        if (!tab.activeSelf) title.SetActiveOfAllTab(false, this);
        tab.SetActive(!tab.activeSelf);
    }

    public void OnClickASC()
    {
        tab.SetActive(false);
        manager.SetParam(param, Manager.MENU.ASC);
    }

    public void OnClickDESC()
    {
        tab.SetActive(false);
        manager.SetParam(param, Manager.MENU.DESC);
    }

    public void OnClickFirstPos()
    {
        tab.SetActive(false);
        manager.SetParam(param, Manager.MENU.FIRST_POS);
    }

    public void OnRenameButtonClicked()
    {
        tab.SetActive(false);
        if (param.param != Device.PARAM.EXTRA) return;
        InputManager.Open(this, InputManager.MODE.EXTRA, "名称を入力", tmp.text);
    }

    protected override void OnInputClosed()
    {
        foreach (var tt in title.titleTexts)
        {
            var param = tt.GetParam();
            if (param.param == Device.PARAM.EXTRA && param.tag == text) return;
        }
        manager.RenameParam(param, text);
    }

    public void OnDeleteButtonClicked()
    {
        tab.SetActive(false);
        if (param.param != Device.PARAM.EXTRA) return;
        manager.RemoveParam(param);
    }
}
