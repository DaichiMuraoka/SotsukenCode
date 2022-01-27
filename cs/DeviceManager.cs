using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeviceManager : Manager
{
    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        if (!Network.Cache.isHavingData) yield return StartCoroutine(Network.Get.AllData());
        Filter.onlyEffMax = true;
        Filter.Clear();
        devices = Filter.GetDevice();

        paramOrder.Add(new Device.Param() { param = Device.PARAM.CLASS });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.EFFICIENCY });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.FF });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.V_OC });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.J_SC });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.AUTHOR });

        title.Set(paramOrder, this);

        SetList();
    }

    protected override void SetList()
    {
        while (dataUIs.Count < devices.Count)
        {
            DataUI dui = Instantiate(dataUI, content);
            dataUIs.Add(dui);
        }
        while (dataUIs.Count > devices.Count)
        {
            DataUI dui = dataUIs[0];
            dataUIs.Remove(dui);
            Destroy(dui.gameObject);
        }

        for (int i = 0; i < devices.Count; i++)
        {
            DeviceUI dui = (DeviceUI)dataUIs[i];
            dui.SetData(this, devices[i], paramOrder);
        }
    }

    public void InvertOnlyLab()
    {
        Filter.onlyLab = !Filter.onlyLab;
        devices = Filter.GetDevice();
        SetList();
    }

    public void InvertOnlyEffMax()
    {
        Filter.onlyEffMax = !Filter.onlyEffMax;
        devices = Filter.GetDevice();
        SetList();
    }

    public void OnAddDeviceButtonClicked()
    {
        SceneManager.LoadScene("Insert");
    }
}
