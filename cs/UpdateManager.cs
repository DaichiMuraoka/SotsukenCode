using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UpdateManager : Manager
{
    [SerializeField] private float horAdjust = 100;

    public static Device updateDevice = null;

    [SerializeField] private Toggle labToggle = null;

    [SerializeField] private TMP_InputField urlIF = null;

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    public IEnumerator Initialize()
    {
        Filter.onlyEffMax = false;

        labToggle.isOn = updateDevice.lab;

        foreach (var data in updateDevice.datas)
        {
            paramOrder.Add(new Device.Param() { param = data.param.param, tag = data.param.tag });
        }

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
        while (dataUIs.Count > 1)
        {
            DataUI dui = dataUIs[0];
            dataUIs.Remove(dui);
            Destroy(dui.gameObject);
        }
        dataUIs[0].SetData(this, updateDevice, paramOrder, title.SizeDeltaX + horAdjust);

        urlIF.text = dataUIs[0].Device.url;
    }

    public void OnUpdateButtonClicked()
    {
        dataUIs[0].SetURL(urlIF.text);
        StartCoroutine(Network.Edit.Update(dataUIs[0].Device));
    }

    public void OnCancelButtonClicked()
    {
        if (renamed) Network.Cache.isHavingData = false;
        SceneManager.LoadScene("Device");
    }

    public void OnRemoveButtonClicked()
    {
        CheckManager.Open(Network.Edit.Remove(updateDevice), "削除しますか?");
    }

    public void OnLabToggleValueChanged()
    {
        if (dataUIs.Count == 0) return;
        dataUIs[0].SetLab(labToggle.isOn);
    }
}
