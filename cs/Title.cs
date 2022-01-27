using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : InputCaller
{
    [SerializeField] protected TitleText titleText = null;
    public List<TitleText> titleTexts = new List<TitleText>();

    [SerializeField] protected Transform texts = null;

    protected Manager manager = null;

    public virtual void Set(List<Device.Param> paramList, Manager _manager)
    {
        manager = _manager;
        while(titleTexts.Count < paramList.Count)
        {
            TitleText tbtn = Instantiate(titleText, texts);
            titleTexts.Add(tbtn);
        }
        while (titleTexts.Count > paramList.Count)
        {
            TitleText tbtn = titleTexts[0];
            titleTexts.Remove(tbtn);
            Destroy(tbtn.gameObject);
        }

        for(int i = 0; i < titleTexts.Count; i++)
        {
            titleTexts[i].SetParam(paramList[i], _manager, this);
        }
    }

    public float SizeDeltaX
    {
        get
        {
            RectTransform rt = GetComponent<RectTransform>();
            return rt.sizeDelta.x;
        }
    }

    public void SetActiveOfAllTab(bool value, TitleButton exception = null)
    {
        foreach(var ttxt in titleTexts)
        {
            TitleButton tbtn = (TitleButton)ttxt;
            if (tbtn == exception) continue;
            tbtn.SetActiveOfTab(value);
        }
    }

    public void OnAddButtonClicked()
    {
        InputManager.Open(this, InputManager.MODE.EXTRA, "名称を入力");
    }

    protected override void OnInputClosed()
    {
        foreach(var tt in titleTexts)
        {
            var param = tt.GetParam();
            if (param.param == Device.PARAM.EXTRA && param.tag == text) return;
        }
        manager.AddParam(new Device.Param() { param = Device.PARAM.EXTRA, tag = text });
    }
}
