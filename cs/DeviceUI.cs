using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceUI : DataUI
{
    [SerializeField] private DataButton dataButton = null;

    private List<DataButton> dataButtons = new List<DataButton>();

    public override void SetData(Manager _manager, Device device, List<Device.Param> paramList, float sizeDelta_x = 0f)
    {
        manager = _manager;
        Device = device;

        linkButton.SetActive(device.url != "");

        List<Device.Param> buttonList = paramList.FindAll(p => p.param == Device.PARAM.AUTHOR || p.param == Device.PARAM.CLASS);

        while (dataButtons.Count < buttonList.Count)
        {
            DataButton dbtn = Instantiate(dataButton, datas);
            dataButtons.Add(dbtn);
        }
        while (dataButtons.Count > buttonList.Count)
        {
            DataButton dbtn = dataButtons[0];
            dataButtons.Remove(dbtn);
            Destroy(dbtn.gameObject);
        }

        while (dataTexts.Count < paramList.Count - buttonList.Count)
        {
            DataText dtxt = Instantiate(dataText, datas);
            dataTexts.Add(dtxt);
        }
        while (dataTexts.Count > paramList.Count - buttonList.Count)
        {
            DataText dtxt = dataTexts[0];
            dataTexts.Remove(dtxt);
            Destroy(dtxt.gameObject);
        }

        int dbtnCount = 0;
        for (int i = 0; i < paramList.Count; i++)
        {
            bool space = true;
            var param = paramList[i].param;

            foreach (var data in device.datas)
            {
                if ((param == data.param.param) && (paramList[i].tag == data.param.tag))
                {
                    space = false;
                    if(param == Device.PARAM.AUTHOR || param == Device.PARAM.CLASS)
                    {
                        dataButtons[dbtnCount].SetData(data);
                        dataButtons[dbtnCount].transform.SetSiblingIndex(i);
                        dbtnCount++;
                    }
                    else
                    {
                        dataTexts[i - dbtnCount].SetData(data);
                        dataTexts[i - dbtnCount].transform.SetSiblingIndex(i);
                    }
                }
            }

            if (space)
            {
                if (param == Device.PARAM.AUTHOR || param == Device.PARAM.CLASS)
                {
                    dataButtons[dbtnCount].SetData(new Device.Data() { param = paramList[i] }, true);
                    dataButtons[dbtnCount].transform.SetSiblingIndex(i);
                }
                else
                {
                    dataTexts[i - dbtnCount].SetData(new Device.Data() { param = paramList[i] }, true);
                    dataTexts[i - dbtnCount].transform.SetSiblingIndex(i);
                }
            }
        }
    }
}
