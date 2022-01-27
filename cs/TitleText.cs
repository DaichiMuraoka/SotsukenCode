using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleText : InputCaller
{
    [SerializeField] protected TextMeshProUGUI tmp = null;

    protected Device.Param param = null;

    protected Manager manager = null;

    protected Title title = null;

    public Device.Param GetParam()
    {
        return param;
    }

    public void SetParam(Device.Param _param, Manager _manager, Title _title)
    {
        param = _param;
        tmp.text = _param.GetTitle();
        manager = _manager;
        title = _title;
    }
}
