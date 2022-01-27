using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtraManager : Manager
{
    [SerializeField] private float horAdjust = 200;

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    public IEnumerator Initialize()
    {
        Filter.onlyEffMax = false;

        devices = Filter.GetDevice();

        paramOrder.Add(new Device.Param() { param = Device.PARAM.CLASS });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.EFFICIENCY } );
        paramOrder.Add(new Device.Param() { param = Device.PARAM.FF });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.V_OC });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.J_SC });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.ALD });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.SULFURIZATION });

        foreach(var device in devices)
        {
            foreach(var data in device.datas)
            {
                bool isNewParam = data.param.param == Device.PARAM.EXTRA;
                foreach(var param in paramOrder)
                {
                    if ((param.tag == data.param.tag) && (param.param == data.param.param)) isNewParam = false;
                }
                if (isNewParam) paramOrder.Add(data.param);
            }
        }

        paramOrder.Add(new Device.Param() { param = Device.PARAM.CLASS_DETAIL });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.AUTHOR });


        title.Set(paramOrder, this);

        yield return null;

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
            DataUI dui = dataUIs[i];
            dui.SetData(this, devices[i], paramOrder, title.SizeDeltaX + horAdjust);
        }
    }
}
