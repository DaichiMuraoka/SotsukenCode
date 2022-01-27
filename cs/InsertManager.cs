using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InsertManager : Manager
{
    [SerializeField] private float horAdjust = 100;

    [SerializeField] private Toggle labToggle = null;

    [SerializeField] private TMP_InputField urlIF = null;

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    public IEnumerator Initialize()
    {
        paramOrder.Add(new Device.Param() { param = Device.PARAM.CLASS });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.CLASS_DETAIL });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.ALD });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.SULFURIZATION });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.EFFICIENCY });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.FF });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.V_OC });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.J_SC });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.AUTHOR });

        title.Set(paramOrder, this);

        yield return null;

        SetList();
    }

    protected override void SetList()
    {
        while (dataUIs.Count < 1)
        {
            DataUI dui = Instantiate(dataUI, content);
            dataUIs.Add(dui);
        }
        foreach(var dui in dataUIs)
        {
            dui.SetData(this, dui.Device, paramOrder, title.SizeDeltaX + horAdjust);
        }
        OnLabToggleValueChanged();
    }

    public void OnAddDeviceButtonClicked()
    {
        DataUI dui = Instantiate(dataUI, content);
        dui.SetData(this, dataUIs[dataUIs.Count - 1].Device, paramOrder, title.SizeDeltaX + horAdjust);
        dataUIs.Add(dui);
        SetList();
    }

    public void OnInsertButtonClicked()
    {
        devices = new List<Device>();
        foreach (var dui in dataUIs)
        {
            dui.SetURL(urlIF.text);
            devices.Add(dui.Device);
        }
        StartCoroutine(Network.Edit.Insert(devices));
    }

    public void OnCancelButtonClicked()
    {
        SceneManager.LoadScene("Device");
    }

    public void OnLabToggleValueChanged()
    {
        foreach (var dui in dataUIs)
        {
            dui.SetLab(labToggle.isOn);
        }
    }
}
