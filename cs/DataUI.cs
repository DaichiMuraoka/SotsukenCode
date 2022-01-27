using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataUI : MonoBehaviour
{
    [SerializeField] protected DataText dataText = null;
    protected List<DataText> dataTexts = new List<DataText>();

    [SerializeField] protected Transform datas = null;

    protected Manager manager = null;

    private Device device = null;

    [SerializeField] protected GameObject linkButton = null;

    public Device Device
    {
        get
        {
            if (device == null)
            {
                Debug.Log("device is null");
                return new Device();
            }
            var ret = new Device() { id = device.id, lab = device.lab, url = device.url };
            //Debug.Log(dataTexts.Count);
            foreach(var dt in dataTexts)
            {
                ret.datas.Add(dt.GetData());
            }
            return ret;
        }
        protected set { device = value; }
    }

    public virtual void SetData(Manager _manager, Device _device, List<Device.Param> paramList, float sizeDelta_x)
    {
        manager = _manager;
        Device = _device;

        if(linkButton != null) linkButton.SetActive(device.url != "");

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(sizeDelta_x, rectTransform.sizeDelta.y);
        
        while (dataTexts.Count < paramList.Count)
        {
            DataText dtxt = Instantiate(dataText, datas);
            dataTexts.Add(dtxt);
        }
        while (dataTexts.Count > paramList.Count)
        {
            DataText dtxt = dataTexts[0];
            dataTexts.Remove(dtxt);
            Destroy(dtxt.gameObject);
        }

        for(int i = 0; i < paramList.Count; i++)
        {
            bool space = true;

            foreach (var data in _device.datas)
            {
                if ((paramList[i].param == data.param.param) && (paramList[i].tag == data.param.tag))
                {
                    space = false;
                    dataTexts[i].SetData(data);
                    break;
                }
            }

            if (space)
            {
                dataTexts[i].SetData(new Device.Data() { param = paramList[i] }, true);
            }
        }
    }

    public void SetLab(bool value)
    {
        if (device == null) return;
        device.lab = value;
    }

    public void SetURL(string url)
    {
        if (device == null) return;
        device.url = url;
    }

    public void OnEditButtonClicked()
    {
        UpdateManager.updateDevice = device;
        SceneManager.LoadScene("Update");
    }

    public void OnRemoveButtonClicked()
    {
        manager.RemoveDataUI(this);
    }

    public void OnLinkButtonClicked()
    {
        Application.OpenURL(Device.url);
    }
}
